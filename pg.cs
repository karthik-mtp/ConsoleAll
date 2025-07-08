 using Newtonsoft.Json;
using MsgKit;
using MsgKit.Enums;
using System.Text;

public class EmailData
{
    [JsonProperty("from")] public string From { get; set; } = string.Empty;
    [JsonProperty("to")] public List<string> To { get; set; } = new List<string>();
    [JsonProperty("cc")] public List<string>? Cc { get; set; }
    [JsonProperty("bcc")] public List<string>? Bcc { get; set; }
    [JsonProperty("subject")] public string Subject { get; set; } = string.Empty;
    [JsonProperty("body")] public string Body { get; set; } = string.Empty;
    [JsonProperty("attachments")] public List<string>? Attachments { get; set; }
    [JsonProperty("replyHistory")] public List<EmailHistoryItem>? ReplyHistory { get; set; }
    [JsonProperty("destination")] public string? Destination { get; set; }
}

public class EmailHistoryItem
{
    [JsonProperty("from")] public string From { get; set; } = string.Empty;
    [JsonProperty("date")] public DateTime Date { get; set; }
    [JsonProperty("subject")] public string Subject { get; set; } = string.Empty;
    [JsonProperty("body")] public string Body { get; set; } = string.Empty;
    [JsonProperty("to")] public List<string>? To { get; set; }
    [JsonProperty("cc")] public List<string>? Cc { get; set; }
}

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        try
        {
            string jsonFilePath = args.Length > 0 ? args[0] : "sample-data.json";
            
            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"❌ Error: File '{jsonFilePath}' not found!");
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
            var emailData = JsonConvert.DeserializeObject<EmailData>(jsonContent);

            if (emailData == null || string.IsNullOrEmpty(emailData.From) || !emailData.To.Any())
            {
                Console.WriteLine("❌ Error: Invalid email data!");
                return;
            }

            // Validate input size for cloud environments
            if (emailData.Attachments != null && emailData.Attachments.Count > 10)
            {
                Console.WriteLine("❌ Error: Too many attachments (max 10 allowed)!");
                return;
            }

            string outputPath = !string.IsNullOrEmpty(emailData.Destination) 
                ? Path.Combine(emailData.Destination, GenerateFileName(emailData.Subject))
                : GenerateFileName(emailData.Subject);

            await CreateOutlookTemplate(emailData, outputPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates an Outlook Template (.oft) file that opens in compose mode
    /// </summary>
    static async System.Threading.Tasks.Task CreateOutlookTemplate(EmailData emailData, string outputPath)
    {
        try
        {
            // Ensure we have .oft extension
            var oftPath = Path.ChangeExtension(outputPath, ".oft");
            
            var directory = Path.GetDirectoryName(oftPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            
            // Create sender
            var sender = new Sender(emailData.From, emailData.From);
            
            // Create email
            using var email = new Email(sender, emailData.Subject);
            
            // Add recipients
            foreach (var recipient in emailData.To)
            {
                email.Recipients.AddTo(recipient, recipient);
            }
                
            if (emailData.Cc != null)
                foreach (var recipient in emailData.Cc)
                {
                    email.Recipients.AddCc(recipient, recipient);
                }
                    
            if (emailData.Bcc != null)
                foreach (var recipient in emailData.Bcc)
                {
                    email.Recipients.AddBcc(recipient, recipient);
                }

            // Set HTML body with reply history
            email.BodyHtml = BuildEmailBody(emailData);
            email.Importance = MessageImportance.IMPORTANCE_NORMAL;

            // Add attachments
            if (emailData.Attachments != null && emailData.Attachments.Any())
            {
                await AddAttachments(email, emailData.Attachments);
            }

            // Save as OFT template file
            using (var fs = new FileStream(oftPath, FileMode.Create))
            {
                email.Save(fs);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error creating template: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Builds the email body with reply history formatting
    /// </summary>
    public static string BuildEmailBody(EmailData emailData)
    {
        var body = new StringBuilder(emailData.Body);
        
        if (emailData.ReplyHistory != null && emailData.ReplyHistory.Any())
        {
            body.AppendLine("<br/><hr/>");
            body.AppendLine("<div style='color: #666; font-size: 11px;'>");
            body.AppendLine("<strong>Reply History:</strong>");
            body.AppendLine("</div>");
            
            foreach (var item in emailData.ReplyHistory)
            {
                body.AppendLine("<div style='border-left: 3px solid #0078d4; padding-left: 15px; margin: 15px 0; background-color: #f8f9fa;'>");
                body.AppendLine($"<div style='font-weight: bold; color: #323130;'>From: {item.From}</div>");
                body.AppendLine($"<div style='color: #605e5c; font-size: 12px;'>Sent: {item.Date:dddd, MMMM dd, yyyy h:mm tt}</div>");
                
                if (item.To?.Any() == true) 
                    body.AppendLine($"<div style='color: #605e5c; font-size: 12px;'>To: {string.Join("; ", item.To)}</div>");
                    
                if (item.Cc?.Any() == true) 
                    body.AppendLine($"<div style='color: #605e5c; font-size: 12px;'>Cc: {string.Join("; ", item.Cc)}</div>");
                
                body.AppendLine($"<div style='font-weight: bold; color: #323130; margin-top: 10px;'>Subject: {item.Subject}</div>");
                body.AppendLine("<br/>");
                body.AppendLine($"<div style='color: #323130;'>{item.Body}</div>");
                body.AppendLine("</div>");
            }
        }
        return body.ToString();
    }

    /// <summary>
    /// Downloads and adds attachments from URLs or local files with memory-efficient handling
    /// </summary>
    static async System.Threading.Tasks.Task AddAttachments(Email email, List<string> attachmentPaths)
    {
        // Cloud-friendly: Add timeout and size limits
        using var httpClient = new HttpClient() 
        { 
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        const int maxAttachmentSize = 25 * 1024 * 1024; // 25MB limit for cloud environments
        
        foreach (var path in attachmentPaths)
        {
            try
            {
                byte[] fileBytes;
                string fileName;

                if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && (uri.Scheme == "http" || uri.Scheme == "https"))
                {
                    // Download from URL with proper disposal
                    using var response = await httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        // Check content size before downloading
                        if (response.Content.Headers.ContentLength > maxAttachmentSize)
                            continue;

                        fileBytes = await response.Content.ReadAsByteArrayAsync();
                        fileName = Path.GetFileName(uri.LocalPath);
                        if (string.IsNullOrEmpty(fileName))
                        {
                            // Extract filename from Content-Disposition header if available
                            var contentDisposition = response.Content.Headers.ContentDisposition?.FileName?.Trim('"');
                            fileName = !string.IsNullOrEmpty(contentDisposition) 
                                ? contentDisposition 
                                : $"download_{Guid.NewGuid():N}.bin";
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (File.Exists(path))
                {
                    // Check file size before loading
                    var fileInfo = new FileInfo(path);
                    if (fileInfo.Length > maxAttachmentSize)
                        continue;

                    fileBytes = await File.ReadAllBytesAsync(path);
                    fileName = Path.GetFileName(path);
                }
                else
                {
                    continue;
                }

                if (string.IsNullOrEmpty(fileName))
                    fileName = $"attachment_{Guid.NewGuid():N}.bin";
                
                // MsgKit will take ownership of the stream and dispose it when Email is disposed
                var stream = new MemoryStream(fileBytes);
                email.Attachments.Add(stream, fileName);
            }
            catch (Exception)
            {
                // Silently continue on attachment errors
            }
        }
    }

    /// <summary>
    /// Generates a safe filename for the .oft template
    /// </summary>
    public static string GenerateFileName(string subject)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeSubject = string.Join("_", subject.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        if (safeSubject.Length > 50) 
            safeSubject = safeSubject.Substring(0, 50);
        
        return $"{safeSubject}_{DateTime.Now:yyyyMMdd_HHmmss}.oft";
    }

    /// <summary>
    /// Creates template in memory for API scenarios (Azure App Service, etc.)
    /// Returns the OFT file as byte array for download/response
    /// </summary>
    public static async Task<byte[]> CreateTemplateInMemory(EmailData emailData)
    {
        var sender = new Sender(emailData.From, emailData.From);
        using var email = new Email(sender, emailData.Subject);
        
        // Add recipients
        foreach (var recipient in emailData.To)
            email.Recipients.AddTo(recipient, recipient);
            
        if (emailData.Cc != null)
            foreach (var recipient in emailData.Cc)
                email.Recipients.AddCc(recipient, recipient);
                
        if (emailData.Bcc != null)
            foreach (var recipient in emailData.Bcc)
                email.Recipients.AddBcc(recipient, recipient);

        // Set body and importance
        email.BodyHtml = BuildEmailBody(emailData);
        email.Importance = MessageImportance.IMPORTANCE_NORMAL;

        // Add attachments with size limits
        if (emailData.Attachments != null && emailData.Attachments.Any())
            await AddAttachments(email, emailData.Attachments);

        // Save to memory stream and return bytes
        using var memoryStream = new MemoryStream();
        email.Save(memoryStream);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Cloud-friendly version that returns the file path and validates before processing
    /// </summary>
    static async Task<string?> CreateTemplateWithValidation(EmailData emailData, string outputPath)
    {
        // Validate input data
        if (emailData == null || string.IsNullOrEmpty(emailData.From) || !emailData.To.Any())
            return null;

        // Check total estimated memory usage for attachments
        const long maxTotalAttachmentSize = 100 * 1024 * 1024; // 100MB total limit
        long estimatedSize = 0;

        if (emailData.Attachments != null)
        {
            using var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) };
            
            foreach (var path in emailData.Attachments.Take(10)) // Limit to 10 attachments max
            {
                try
                {
                    if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && (uri.Scheme == "http" || uri.Scheme == "https"))
                    {
                        using var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, uri));
                        if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength.HasValue)
                        {
                            estimatedSize += response.Content.Headers.ContentLength.Value;
                        }
                    }
                    else if (File.Exists(path))
                    {
                        estimatedSize += new FileInfo(path).Length;
                    }

                    if (estimatedSize > maxTotalAttachmentSize)
                        return null; // Too large, reject
                }
                catch
                {
                    // Skip problematic attachments in size calculation
                }
            }
        }

        // Proceed with creation if size is acceptable
        await CreateOutlookTemplate(emailData, outputPath);
        return Path.ChangeExtension(outputPath, ".oft");
    }
}

/// <summary>
/// Cloud-friendly service for creating Outlook templates
/// </summary>
public static class EmailTemplateService
{
    /// <summary>
    /// Creates an OFT template and returns it as byte array for web APIs
    /// Memory-efficient and cloud-optimized
    /// </summary>
    public static async Task<(byte[]? FileBytes, string? FileName, string? ErrorMessage)> CreateTemplateAsync(EmailData emailData)
    {
        try
        {
            // Validate input
            if (emailData == null || string.IsNullOrEmpty(emailData.From) || !emailData.To.Any())
                return (null, null, "Invalid email data: from and to fields are required");

            // Cloud limits
            if (emailData.Attachments?.Count > 10)
                return (null, null, "Too many attachments (max 10 allowed)");

            if (emailData.Body?.Length > 1_000_000) // 1MB body limit
                return (null, null, "Email body too large (max 1MB)");

            // Pre-validate attachments size
            var sizeCheck = await ValidateAttachmentSizes(emailData.Attachments);
            if (!sizeCheck.IsValid)
                return (null, null, sizeCheck.ErrorMessage);

            // Create template in memory
            var fileBytes = await Program.CreateTemplateInMemory(emailData);
            var fileName = Program.GenerateFileName(emailData.Subject);
            
            return (fileBytes, fileName, null);
        }
        catch (Exception ex)
        {
            return (null, null, $"Error creating template: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates attachment sizes before downloading to prevent memory issues
    /// </summary>
    private static async Task<(bool IsValid, string? ErrorMessage)> ValidateAttachmentSizes(List<string>? attachmentPaths)
    {
        if (attachmentPaths == null || !attachmentPaths.Any())
            return (true, null);

        const long maxTotalSize = 100 * 1024 * 1024; // 100MB total
        const long maxSingleSize = 25 * 1024 * 1024;  // 25MB per file
        long totalSize = 0;

        using var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) };

        foreach (var path in attachmentPaths.Take(10))
        {
            try
            {
                long fileSize = 0;

                if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && (uri.Scheme == "http" || uri.Scheme == "https"))
                {
                    using var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, uri));
                    if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength.HasValue)
                    {
                        fileSize = response.Content.Headers.ContentLength.Value;
                    }
                }
                else if (File.Exists(path))
                {
                    fileSize = new FileInfo(path).Length;
                }

                if (fileSize > maxSingleSize)
                    return (false, $"Attachment too large: {Path.GetFileName(path)} ({fileSize:N0} bytes, max {maxSingleSize:N0})");

                totalSize += fileSize;
                if (totalSize > maxTotalSize)
                    return (false, $"Total attachments too large ({totalSize:N0} bytes, max {maxTotalSize:N0})");
            }
            catch
            {
                // Skip validation for problematic URLs/files
                continue;
            }
        }

        return (true, null);
    }
}

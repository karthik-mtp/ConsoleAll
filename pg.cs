using Newtonsoft.Json;
using MsgKit;
using MsgKit.Enums;
using System.Text;
using SystemTask = System.Threading.Tasks.Task;

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
        Console.WriteLine("Outlook MSG Generator");
        
        try
        {
            string jsonFilePath = args.Length > 0 ? args[0] : "sample-data.json";
            
            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"Error: File '{jsonFilePath}' not found!");
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
            var emailData = JsonConvert.DeserializeObject<EmailData>(jsonContent);

            if (emailData == null || string.IsNullOrEmpty(emailData.From) || !emailData.To.Any())
            {
                Console.WriteLine("Error: Invalid email data!");
                return;
            }

            string outputPath = !string.IsNullOrEmpty(emailData.Destination) 
                ? Path.Combine(emailData.Destination, GenerateFileName(emailData.Subject))
                : GenerateFileName(emailData.Subject);

            await GenerateMsgFile(emailData, outputPath);
            Console.WriteLine($"MSG file created: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async System.Threading.Tasks.Task GenerateMsgFile(EmailData emailData, string outputPath)
    {
        using var email = new Email(new Sender(emailData.From, emailData.From), emailData.Subject);
        
        foreach (var recipient in emailData.To)
            email.Recipients.AddTo(recipient, recipient);
            
        if (emailData.Cc != null)
            foreach (var recipient in emailData.Cc)
                email.Recipients.AddCc(recipient, recipient);
                
        if (emailData.Bcc != null)
            foreach (var recipient in emailData.Bcc)
                email.Recipients.AddBcc(recipient, recipient);

        email.BodyHtml = BuildEmailBody(emailData);
        email.Importance = MessageImportance.IMPORTANCE_NORMAL;

        if (emailData.Attachments != null)
            await AddAttachments(email, emailData.Attachments);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
        email.Save(outputPath);
    }

    static string BuildEmailBody(EmailData emailData)
    {
        var body = new StringBuilder(emailData.Body);
        
        if (emailData.ReplyHistory != null && emailData.ReplyHistory.Any())
        {
            body.AppendLine("<br/><hr/>");
            foreach (var item in emailData.ReplyHistory)
            {
                body.AppendLine("<div style='border-left: 3px solid #ccc; padding-left: 10px; margin: 10px 0;'>");
                body.AppendLine($"<strong>From:</strong> {item.From}<br/>");
                body.AppendLine($"<strong>Sent:</strong> {item.Date:dddd, MMMM dd, yyyy h:mm tt}<br/>");
                if (item.To?.Any() == true) body.AppendLine($"<strong>To:</strong> {string.Join("; ", item.To)}<br/>");
                if (item.Cc?.Any() == true) body.AppendLine($"<strong>Cc:</strong> {string.Join("; ", item.Cc)}<br/>");
                body.AppendLine($"<strong>Subject:</strong> {item.Subject}<br/><br/>");
                body.AppendLine(item.Body);
                body.AppendLine("</div><br/>");
            }
        }
        return body.ToString();
    }

    static async System.Threading.Tasks.Task AddAttachments(Email email, List<string> attachmentUrls)
    {
        using var httpClient = new HttpClient();
        foreach (var url in attachmentUrls)
        {
            try
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    var response = await httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        var fileBytes = await response.Content.ReadAsByteArrayAsync();
                        var fileName = Path.GetFileName(uri.LocalPath);
                        if (string.IsNullOrEmpty(fileName))
                            fileName = $"attachment_{Guid.NewGuid():N}.bin";
                        
                        var stream = new MemoryStream(fileBytes);
                        email.Attachments.Add(stream, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add attachment {url}: {ex.Message}");
            }
        }
    }

    static string GenerateFileName(string subject)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeSubject = string.Join("_", subject.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        if (safeSubject.Length > 50) safeSubject = safeSubject.Substring(0, 50);
        return $"{safeSubject}_{DateTime.Now:yyyyMMdd_HHmmss}.msg";
    }
}



{
  "from": "john.doe@company.com",
  "to": [
    "jane.smith@company.com",
    "mike.wilson@company.com"
  ],
  "cc": [
    "manager@company.com"
  ],
  "subject": "Re: Project Update - Q2 Review",
  "body": "<html><body><div style='font-family: Calibri, Arial, sans-serif; font-size: 11pt;'><p>Hi team,</p><p>Thank you for the detailed update on the Q2 project review. I've reviewed the documents and have the following feedback:</p><ul><li><strong>Budget Analysis:</strong> The numbers look good overall, but we need to address the variance in the marketing budget.</li><li><strong>Timeline:</strong> The proposed timeline seems realistic, but let's add a 2-week buffer for testing.</li><li><strong>Resource Allocation:</strong> We might need additional developer resources in Q3.</li></ul><p>Please schedule a follow-up meeting to discuss these points in detail.</p><p>Best regards,<br/>John Doe<br/>Project Manager</p></div></body></html>",
  "attachments": [
    "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf"
  ],
  "destination": "C:\\karthik\\projects\\poc\\outlook-new\\MSG\\msgfiles",
  "replyHistory": [
    {
      "from": "jane.smith@company.com",
      "date": "2025-07-07T14:30:00Z",
      "subject": "Project Update - Q2 Review",
      "body": "<div style='font-family: Calibri, Arial, sans-serif; font-size: 11pt;'><p>Hi John,</p><p>I'm sending the Q2 project review as requested. Please find the key highlights below:</p><ul><li>Budget utilization: 78% of allocated funds</li><li>Timeline: On track with minor delays in testing phase</li><li>Team performance: Excellent across all departments</li></ul><p>The detailed report is attached for your review.</p><p>Looking forward to your feedback.</p><p>Best regards,<br/>Jane Smith<br/>Senior Analyst</p></div>",
      "to": [
        "john.doe@company.com"
      ],
      "cc": [
        "manager@company.com"
      ]
    },
    {
      "from": "manager@company.com",
      "date": "2025-07-06T09:15:00Z",
      "subject": "Project Update - Q2 Review",
      "body": "<div style='font-family: Calibri, Arial, sans-serif; font-size: 11pt;'><p>Team,</p><p>We need to prepare the Q2 review by end of this week. Please ensure all departments submit their reports by Thursday.</p><p>Key areas to focus on:</p><ul><li>Budget analysis</li><li>Timeline adherence</li><li>Resource utilization</li><li>Risk assessment</li></ul><p>Let me know if you need any clarification.</p><p>Thanks,<br/>Sarah Johnson<br/>Department Manager</p></div>",
      "to": [
        "john.doe@company.com",
        "jane.smith@company.com",
        "mike.wilson@company.com"
      ]
    }
  ]
}

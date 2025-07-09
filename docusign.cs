using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using System;
using System.Collections.Generic;
using System.IO;

public async Task<string> SendEnvelopeWithXYSignature()
{
    // Step 1: Setup the API Client
    var apiClient = new ApiClient("https://demo.docusign.net/restapi");
    apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer YOUR_ACCESS_TOKEN");

    var accountId = "YOUR_ACCOUNT_ID";

    // Step 2: Prepare Document
    byte[] fileBytes = File.ReadAllBytes("sample.pdf");
    var docBase64 = Convert.ToBase64String(fileBytes);

    var document = new Document
    {
        DocumentBase64 = docBase64,
        Name = "Document for Signing",
        FileExtension = "pdf",
        DocumentId = "1"
    };

    // Step 3: Define the SignHere tab (X,Y coordinates)
    var signHere = new SignHere
    {
        DocumentId = "1",
        PageNumber = "1",
        XPosition = "200",   // X from left
        YPosition = "300"    // Y from top
    };

    // Step 4: Create the signer with embedded signing
    var signer = new Signer
    {
        Email = "signer@example.com",
        Name = "John Doe",
        RecipientId = "1",
        ClientUserId = "1234", // required for embedded signing
        Tabs = new Tabs
        {
            SignHereTabs = new List<SignHere> { signHere }
        }
    };

    // Step 5: Create the envelope
    var envelopeDefinition = new EnvelopeDefinition
    {
        EmailSubject = "Please sign this document",
        Documents = new List<Document> { document },
        Recipients = new Recipients { Signers = new List<Signer> { signer } },
        Status = "sent" // "sent" to actually send, or "created" to save as draft
    };

    // Step 6: Create Envelope
    var envelopesApi = new EnvelopesApi(apiClient.Configuration);
    EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);
    string envelopeId = results.EnvelopeId;

    // Step 7: Generate embedded signing URL
    var recipientViewRequest = new RecipientViewRequest
    {
        ReturnUrl = "https://your-app.com/return-url", // redirect after signing
        ClientUserId = "1234",
        AuthenticationMethod = "none",
        Email = "signer@example.com",
        UserName = "John Doe"
    };

    ViewUrl recipientView = envelopesApi.CreateRecipientView(accountId, envelopeId, recipientViewRequest);
    return recipientView.Url; // <- This is the URL to open in an iframe or popup
}

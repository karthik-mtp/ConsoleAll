// Sample JSON data structure
const sampleMailData = {
    from: "sender@company.com",
    to: "recipient@example.com; another@example.com",
    cc: "manager@company.com",
    bcc: "",
    subject: "RE: Project Update - Q1 Results",
    body: "Hi Team,\n\nThank you for the comprehensive update. I have a few follow-up questions:\n\n1. What's the timeline for Phase 2 implementation?\n2. Do we need additional resources for the next quarter?\n3. Can we schedule a stakeholder presentation?\n\nLooking forward to your response.\n\nBest regards,\nJohn Smith",
    emailHistory: [
        {
            from: "project.manager@company.com",
            to: "recipient@example.com; another@example.com",
            cc: "manager@company.com",
            date: "Mon, Jul 7, 2025 at 2:30 PM",
            subject: "Project Update - Q1 Results",
            body: "Dear Team,\n\nI hope this email finds you well. I wanted to provide you with an update on our Q1 project results.\n\nKey achievements:\n• Successfully completed Phase 1 of the project\n• Exceeded target metrics by 15%\n• Received positive feedback from stakeholders\n\nNext steps:\n• Begin Phase 2 implementation\n• Schedule team review meeting\n• Prepare detailed report for management\n\nPlease let me know if you have any questions or concerns.\n\nBest regards,\nProject Manager"
        },
        {
            from: "manager@company.com",
            to: "project.manager@company.com; recipient@example.com; another@example.com",
            cc: "",
            date: "Mon, Jul 7, 2025 at 1:15 PM",
            subject: "RE: Project Update - Q1 Results",
            body: "Great work team! The results look impressive. \n\nI'd like to add that we should also consider:\n• Budget allocation for Q2\n• Team capacity planning\n• Risk assessment for Phase 2\n\nLet's discuss this in our next meeting.\n\nRegards,\nSarah Johnson\nProject Director"
        }
    ],
    attachments: [
        {
            name: "Q1_Report.pdf",
            size: "2.4 MB",
            type: "application/pdf"
        },
        {
            name: "Project_Timeline.xlsx",
            size: "456 KB",
            type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        },
        {
            name: "Team_Photo.jpg",
            size: "1.2 MB",
            type: "image/jpeg"
        }
    ]
};

// Display sample JSON data
function displaySampleJson() {
    document.getElementById('jsonData').textContent = JSON.stringify(sampleMailData, null, 2);
}

// Open compose modal
function openComposeModal() {
    const modal = document.getElementById('composeModal');
    const overlay = document.getElementById('overlay');
    
    modal.classList.add('active');
    overlay.classList.add('active');
    
    // Populate fields with sample data
    populateFields(sampleMailData);
}

// Close compose modal
function closeComposeModal() {
    const modal = document.getElementById('composeModal');
    const overlay = document.getElementById('overlay');
    
    modal.classList.remove('active');
    overlay.classList.remove('active');
}

// Populate fields with JSON data
function populateFields(data) {
    document.getElementById('toField').value = data.to || '';
    document.getElementById('ccField').value = data.cc || '';
    document.getElementById('bccField').value = data.bcc || '';
    document.getElementById('subjectField').value = data.subject || '';
    
    // Format body content with line breaks and add signature
    const bodyEditor = document.getElementById('bodyEditor');
    let bodyContent = '';
    
    if (data.body) {
        bodyContent = data.body.replace(/\n/g, '<br>');
    }
    
    // Add signature as part of body content
    const signatureHtml = `
        <br><br>
        <div style="border-top: 1px solid #edebe9; padding-top: 12px; margin-top: 20px;">
            <div>
                <img src="https://www.signwell.com/assets/vip-signatures/clint-eastwood-signature-e5a46a2363ef513d4fc0a45d8c0340943082ce60229084e3a12b82539321094b.png" 
                     alt="Signature" 
                     style="max-width: 200px; max-height: 80px; height: auto; border-radius: 4px; box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);">
            </div>
        </div>
    `;
    
    // Add email history if available
    let emailHistoryHtml = '';
    if (data.emailHistory && data.emailHistory.length > 0) {
        emailHistoryHtml = '<br><br><div style="border-top: 2px solid #edebe9; margin-top: 30px; padding-top: 20px;">';
        
        data.emailHistory.forEach((email, index) => {
            emailHistoryHtml += `
                <div style="margin-bottom: 25px; border-left: 3px solid #106ebe; padding-left: 15px;">
                    <div style="font-size: 13px; color: #605e5c; margin-bottom: 8px; font-weight: 600;">
                        <strong>From:</strong> ${email.from}<br>
                        <strong>To:</strong> ${email.to}${email.cc ? '<br><strong>Cc:</strong> ' + email.cc : ''}<br>
                        <strong>Date:</strong> ${email.date}<br>
                        <strong>Subject:</strong> ${email.subject}
                    </div>
                    <div style="font-size: 14px; line-height: 1.5; color: #323130; background: #faf9f8; padding: 12px; border-radius: 4px;">
                        ${email.body.replace(/\n/g, '<br>')}
                    </div>
                </div>
            `;
        });
        
        emailHistoryHtml += '</div>';
    }
    
    bodyEditor.innerHTML = bodyContent + signatureHtml + emailHistoryHtml;
    
    // Show CC field if there's CC data
    if (data.cc) {
        toggleCc();
    }
    
    // Show BCC field if there's BCC data
    if (data.bcc) {
        toggleBcc();
    }
    
    // Populate attachments
    if (data.attachments && data.attachments.length > 0) {
        populateAttachments(data.attachments);
    }
}

// Populate attachments
function populateAttachments(attachments) {
    const attachmentsSection = document.getElementById('attachmentsSection');
    const attachmentsList = document.getElementById('attachmentsList');
    
    // Clear existing attachments
    attachmentsList.innerHTML = '';
    
    attachments.forEach((attachment, index) => {
        const attachmentItem = createAttachmentElement(attachment, index);
        attachmentsList.appendChild(attachmentItem);
    });
    
    // Show attachments section
    attachmentsSection.classList.add('active');
}

// Create attachment element
function createAttachmentElement(attachment, index) {
    const attachmentDiv = document.createElement('div');
    attachmentDiv.className = 'attachment-item';
    
    // Get appropriate icon based on file type
    const icon = getFileIcon(attachment.type, attachment.name);
    
    attachmentDiv.innerHTML = `
        <i class="${icon} attachment-icon"></i>
        <div class="attachment-info">
            <div class="attachment-name" title="${attachment.name}">${attachment.name}</div>
            <div class="attachment-size">${attachment.size}</div>
        </div>
        <button class="attachment-remove" onclick="removeAttachment(${index})" title="Remove attachment">
            <i class="fas fa-times"></i>
        </button>
    `;
    
    return attachmentDiv;
}

// Get file icon based on type
function getFileIcon(type, filename) {
    const extension = filename.split('.').pop().toLowerCase();
    
    if (type.includes('pdf') || extension === 'pdf') {
        return 'fas fa-file-pdf';
    } else if (type.includes('image') || ['jpg', 'jpeg', 'png', 'gif', 'bmp'].includes(extension)) {
        return 'fas fa-file-image';
    } else if (type.includes('spreadsheet') || ['xlsx', 'xls', 'csv'].includes(extension)) {
        return 'fas fa-file-excel';
    } else if (type.includes('document') || ['docx', 'doc'].includes(extension)) {
        return 'fas fa-file-word';
    } else if (type.includes('text') || extension === 'txt') {
        return 'fas fa-file-alt';
    } else if (['zip', 'rar', '7z'].includes(extension)) {
        return 'fas fa-file-archive';
    } else {
        return 'fas fa-file';
    }
}

// Remove attachment
function removeAttachment(index) {
    const attachmentsList = document.getElementById('attachmentsList');
    const attachmentItems = attachmentsList.querySelectorAll('.attachment-item');
    
    if (attachmentItems[index]) {
        attachmentItems[index].remove();
    }
    
    // Hide attachments section if no attachments left
    if (attachmentsList.children.length === 0) {
        document.getElementById('attachmentsSection').classList.remove('active');
    }
}

// Toggle CC field
function toggleCc() {
    const ccRow = document.getElementById('ccRow');
    if (ccRow.style.display === 'none' || ccRow.style.display === '') {
        ccRow.style.display = 'flex';
        document.getElementById('ccField').focus();
    } else {
        ccRow.style.display = 'none';
        document.getElementById('ccField').value = '';
    }
}

// Toggle BCC field
function toggleBcc() {
    const bccRow = document.getElementById('bccRow');
    if (bccRow.style.display === 'none' || bccRow.style.display === '') {
        bccRow.style.display = 'flex';
        document.getElementById('bccField').focus();
    } else {
        bccRow.style.display = 'none';
        document.getElementById('bccField').value = '';
    }
}

// Send mail function
function sendMail() {
    const mailData = {
        to: document.getElementById('toField').value,
        cc: document.getElementById('ccField').value,
        bcc: document.getElementById('bccField').value,
        subject: document.getElementById('subjectField').value,
        body: document.getElementById('bodyEditor').innerHTML,
        attachments: getAttachmentsData()
    };
    
    // Validate required fields
    if (!mailData.to.trim()) {
        alert('Please enter at least one recipient in the "To" field.');
        document.getElementById('toField').focus();
        return;
    }
    
    if (!mailData.subject.trim()) {
        alert('Please enter a subject for your email.');
        document.getElementById('subjectField').focus();
        return;
    }
    
    // Here you would typically send the data to your backend
    console.log('Sending email with data:', mailData);
    
    // Show success message
    alert('Email sent successfully!');
    
    // Close modal
    closeComposeModal();
    
    // Clear form
    clearForm();
}

// Get attachments data
function getAttachmentsData() {
    const attachmentItems = document.querySelectorAll('.attachment-item');
    const attachments = [];
    
    attachmentItems.forEach(item => {
        const name = item.querySelector('.attachment-name').textContent;
        const size = item.querySelector('.attachment-size').textContent;
        attachments.push({ name, size });
    });
    
    return attachments;
}

// Clear form
function clearForm() {
    document.getElementById('toField').value = '';
    document.getElementById('ccField').value = '';
    document.getElementById('bccField').value = '';
    document.getElementById('subjectField').value = '';
    document.getElementById('bodyEditor').innerHTML = '';
    
    // Hide CC and BCC fields
    document.getElementById('ccRow').style.display = 'none';
    document.getElementById('bccRow').style.display = 'none';
    
    // Clear attachments
    document.getElementById('attachmentsList').innerHTML = '';
    document.getElementById('attachmentsSection').classList.remove('active');
}

// Handle keyboard shortcuts
document.addEventListener('keydown', function(e) {
    // Close modal with Escape key
    if (e.key === 'Escape') {
        closeComposeModal();
    }
    
    // Send email with Ctrl+Enter
    if (e.ctrlKey && e.key === 'Enter') {
        const modal = document.getElementById('composeModal');
        if (modal.classList.contains('active')) {
            sendMail();
        }
    }
});

// Close modal when clicking overlay
document.getElementById('overlay').addEventListener('click', closeComposeModal);

// Prevent modal from closing when clicking inside it
document.getElementById('composeModal').addEventListener('click', function(e) {
    e.stopPropagation();
});

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    displaySampleJson();
    
    // Add some formatting toolbar functionality
    setupFormattingButtons();
});

// Setup formatting buttons
function setupFormattingButtons() {
    const boldBtn = document.querySelector('[title="Bold"]');
    const italicBtn = document.querySelector('[title="Italic"]');
    const underlineBtn = document.querySelector('[title="Underline"]');
    
    boldBtn.addEventListener('click', () => {
        document.execCommand('bold', false, null);
        document.getElementById('bodyEditor').focus();
    });
    
    italicBtn.addEventListener('click', () => {
        document.execCommand('italic', false, null);
        document.getElementById('bodyEditor').focus();
    });
    
    underlineBtn.addEventListener('click', () => {
        document.execCommand('underline', false, null);
        document.getElementById('bodyEditor').focus();
    });
}

// Auto-resize body editor
function autoResize() {
    const bodyEditor = document.getElementById('bodyEditor');
    bodyEditor.style.height = 'auto';
    bodyEditor.style.height = bodyEditor.scrollHeight + 'px';
}

// Add input event listener for auto-resize
document.getElementById('bodyEditor').addEventListener('input', autoResize);

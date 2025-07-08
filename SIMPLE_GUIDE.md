# PDF Signature Processor - Clean & Simple Version

## Overview

This is a simplified, high-quality PDF signature processor that uses original signature images without any processing for maximum clarity and quality preservation.

## Key Features

- **Zero Quality Loss**: Uses original signature images directly
- **No Image Processing**: No resizing, recompression, or format conversion
- **Simple Configuration**: Minimal setup required
- **Automatic Sizing**: Automatically detects and uses original image dimensions
- **Clean Code**: No unnecessary dependencies or complex logic

## Installation

Only three dependencies are needed:

```bash
pip install PyMuPDF pdfplumber Pillow
```

Or use the requirements file:
```bash
pip install -r requirements.txt
```

## Usage

### Basic Configuration

```python
from pdf_signature_processor import PDFSignatureProcessor, SignatureConfig

config = SignatureConfig(
    working_folder="C:\\your\\path",
    input_pdf_filename="document.pdf",
    signature_filename="signature.png",
    keywords=["By:", "Sign here"],
    signature_position="right"  # "top", "bottom", "left", "right"
)

processor = PDFSignatureProcessor()
results = processor.process_documents([config])
```

### JSON Configuration (for C# integration)

```json
{
    "workingFolder": "C:\\your\\path",
    "inputPdfFilename": "document.pdf",
    "signatureFilename": "signature.png",
    "keywords": ["By:"],
    "signaturePosition": "right",
    "skipNonEmpty": false
}
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `working_folder` | `str` | Required | Path to folder containing PDF and signature files |
| `input_pdf_filename` | `str` | Required | Name of the PDF file to sign |
| `signature_filename` | `str` | Required | Name of the signature image file |
| `keywords` | `List[str]` | `None` | Keywords to search for signature placement |
| `signature_position` | `str` | `"top"` | Position relative to keyword: "top", "bottom", "left", "right" |
| `x_coord` | `float` | `None` | Manual X coordinate for signature placement |
| `y_coord` | `float` | `None` | Manual Y coordinate for signature placement |
| `skip_non_empty` | `bool` | `False` | Skip keywords that already have content after them |
| `signature_size` | `tuple` | Auto-detected | Automatically set to original image dimensions |

## How It Works

1. **Original Image Detection**: Reads the signature image and detects its dimensions
2. **Direct Byte Reading**: Reads the original image file as raw bytes
3. **No Processing**: Image is used exactly as provided - no modifications
4. **PDF Insertion**: Inserts the original image bytes directly into the PDF
5. **Quality Preservation**: Saves PDF without image compression

## Expected Log Output

When processing, you should see:

```
Using original signature image without any processing for maximum quality
Original signature size: 300 x 120
Signature prepared using original file - no quality loss
Inserted signature using image bytes for maximum quality
PDF saved with original image quality to: signed_document.pdf
```

## Best Practices

### For Maximum Quality:
1. **Use PNG format** for signature images (supports transparency)
2. **High resolution**: Use images with 300+ pixels width
3. **Clean background**: Transparent or white background works best
4. **Proper size**: Make sure your signature image is the size you want in the PDF

### File Organization:
```
your_project/
├── document.pdf          # PDF to sign
├── signature.png         # Your signature image
└── pdf_signature_processor.py
```

## Troubleshooting

### Common Issues:

1. **File not found errors**: Ensure all files are in the correct working folder
2. **Large signatures**: If signature appears too large, resize your image file before processing
3. **Quality issues**: Make sure your original signature image is high quality

### Supported Image Formats:
- PNG (recommended for transparency)
- JPG/JPEG (for opaque signatures)

## Example Output

The processor will create a new file named `signed_[original_filename].pdf` with your signature placed at the keyword locations with perfect quality preservation.

## Dependencies

- **PyMuPDF**: PDF manipulation and signature insertion
- **pdfplumber**: PDF text extraction and keyword searching  
- **Pillow (PIL)**: Basic image dimension detection only

No complex image processing libraries needed!

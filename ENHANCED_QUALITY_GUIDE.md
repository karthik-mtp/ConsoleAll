# PDF Signature Processor - Enhanced Quality Edition

## Features

This enhanced version of the PDF signature processor uses advanced image processing techniques to ensure maximum quality when placing signatures in PDFs, eliminating blur and quality loss.

## Key Improvements

### 1. Advanced Image Processing
- **OpenCV Integration**: Optional OpenCV support for superior image resizing algorithms
- **Multi-Algorithm Resizing**: Uses optimal algorithms based on scaling direction
- **Quality Preservation**: Maintains original image quality when no resizing is needed
- **Smart Sharpening**: Applies sharpening filters for very small signatures

### 2. Direct PDF Integration
- **Byte-Stream Insertion**: Images are inserted as byte streams for maximum quality
- **No File I/O During Insertion**: Eliminates quality loss from file operations
- **Optimal PDF Settings**: Maximum quality PDF output settings

### 3. Flexible Configuration
- **signature_size=None**: Uses original image size for best quality
- **use_advanced_processing**: Enables OpenCV and advanced algorithms
- **high_quality**: Maximum quality processing mode
- **auto_resize**: Smart resizing only when needed

## Installation

### Required Dependencies
```bash
pip install PyMuPDF pdfplumber Pillow
```

### Optional (for maximum quality)
```bash
pip install opencv-python numpy
```

Or install all at once:
```bash
pip install -r requirements.txt
```

## Usage Examples

### Maximum Quality (Recommended)
```python
config = SignatureConfig(
    working_folder="C:\\your\\path",
    input_pdf_filename="document.pdf",
    signature_filename="signature.png",
    keywords=["By:"],
    
    # For maximum quality - use original size
    signature_size=None,
    
    # Enable all quality features
    high_quality=True,
    use_advanced_processing=True,
    auto_resize=True,
    preserve_aspect_ratio=True,
    
    signature_position="right"
)
```

### Custom Size with Quality Preservation
```python
config = SignatureConfig(
    working_folder="C:\\your\\path",
    input_pdf_filename="document.pdf",
    signature_filename="signature.png",
    keywords=["Sign here"],
    
    # Specific size but maintain quality
    signature_size=(100, 40),
    
    # Quality settings
    high_quality=True,
    use_advanced_processing=True,
    preserve_aspect_ratio=True,
    
    signature_position="bottom"
)
```

### JSON Configuration (for C# integration)
```json
{
    "workingFolder": "C:\\your\\path",
    "inputPdfFilename": "document.pdf",
    "signatureFilename": "signature.png",
    "keywords": ["By:"],
    "signatureSize": null,
    "highQuality": true,
    "useAdvancedProcessing": true,
    "autoResize": true,
    "preserveAspectRatio": true,
    "signaturePosition": "right"
}
```

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `signature_size` | `tuple` or `None` | `None` | Target size in pixels. `None` uses original size |
| `high_quality` | `bool` | `True` | Enable maximum quality processing |
| `use_advanced_processing` | `bool` | `True` | Use OpenCV and advanced algorithms |
| `auto_resize` | `bool` | `True` | Automatically resize if image is too large |
| `preserve_aspect_ratio` | `bool` | `True` | Maintain image proportions |
| `signature_position` | `str` | `"top"` | Position relative to keyword: "top", "bottom", "left", "right" |

## Quality Features Explained

### Why signature_size=None Prevents Blur

When `signature_size=None`:
1. The original image is analyzed for its natural dimensions
2. If no resizing is needed, the original image bytes are used directly
3. No recompression or quality loss occurs
4. The image is inserted into the PDF at its native resolution

### Advanced Processing Benefits

With `use_advanced_processing=True`:
1. **OpenCV Resizing**: Uses superior interpolation algorithms
2. **Smart Algorithm Selection**: INTER_AREA for downscaling, INTER_CUBIC for upscaling
3. **Sharpening**: Automatic sharpening for small signatures
4. **Quality Fallback**: Graceful fallback to PIL if OpenCV unavailable

### Direct Byte Insertion

The processor:
1. Processes the image in memory
2. Converts to optimized byte stream
3. Inserts directly into PDF without temporary files
4. Preserves maximum quality throughout the process

## Troubleshooting

### Still Getting Blur?

1. **Check Original Image**: Ensure your signature image is high resolution
2. **Use signature_size=None**: Let the system use original dimensions
3. **Install OpenCV**: `pip install opencv-python numpy`
4. **Verify high_quality=True**: Ensure quality mode is enabled

### Performance Considerations

- OpenCV processing may be slower but provides better quality
- Set `use_advanced_processing=False` for faster processing
- Large images with `auto_resize=False` may create large PDFs

## Example Output

With these settings, you should see log messages like:
```
Original signature size: 300 x 120
Using original image without resizing for maximum quality
Inserted signature using image bytes for maximum quality
PDF saved with maximum quality to: signed_document.pdf
```

This indicates optimal quality processing was used.

import os
import logging
import fitz  # PyMuPDF
import pdfplumber
from PIL import Image
from typing import List, Dict, Optional, Union
from dataclasses import dataclass
from pathlib import Path

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

@dataclass
class SignatureConfig:
    working_folder: str
    input_pdf_filename: str
    item_id: str
    signature_filename: str
    keywords: Optional[List[str]] = None
    signature_size: Optional[tuple[float, float]] = None  # This will be automatically set to original image size
    output_path: Optional[str] = None
    x_coord: Optional[float] = None
    y_coord: Optional[float] = None
    page_numbers: Optional[List[int]] = None
    skip_non_empty: bool = False  # If True, skip matches that have content after the keyword
    signature_position: str = "top"  # Options: "top", "bottom", "left", "right"

@dataclass
class SignatureResult:
    input_pdf_path: str
    item_id: str
    output_pdf_path: str
    success: bool
    error_message: Optional[str] = None

class PDFSignatureProcessor:
    def __init__(self):
        """Initialize the PDF Signature Processor."""
        logger.info("Initializing PDF Signature Processor")

    def _validate_input(self, config: SignatureConfig) -> bool:
        """
        Validate the input parameters.
        
        Args:
            config: SignatureConfig object containing the input parameters
            
        Returns:
            bool: True if validation passes, False otherwise
        """
        try:
            # Build full paths
            pdf_path = os.path.join(config.working_folder, config.input_pdf_filename)
            signature_image_path = os.path.join(config.working_folder, config.signature_filename)
            
            if not os.path.exists(pdf_path):
                raise ValueError(f"PDF file not found:: {pdf_path}")
            
            if not os.path.exists(signature_image_path):
                raise ValueError(f"Signature image not found: {signature_image_path}")
            
            # Validate file extensions
            if not config.input_pdf_filename.lower().endswith('.pdf'):
                raise ValueError(f"Invalid PDF file: {config.input_pdf_filename}")
            
            if not any(config.signature_filename.lower().endswith(ext) 
                      for ext in ['.png', '.jpg', '.jpeg']):
                raise ValueError(f"Invalid signature image format: {config.signature_filename}")
            
            return True
            
        except Exception as e:
            logger.error(f"Validation error: {str(e)}")
            return False

    def _find_keyword_locations(self, pdf_path: str, keywords: List[str], config: SignatureConfig) -> List[tuple[int, float, float]]:
        """
        Find the locations of keywords in the PDF and calculate signature placement.
        
        Args:
            pdf_path: Path to the PDF file
            keywords: List of keywords to search for
            config: SignatureConfig object containing placement preferences
            
        Returns:
            List of tuples containing (page_number, x_coord, y_coord)
        """
        locations = []
        try:
            with pdfplumber.open(pdf_path) as pdf:
                for page_num, page in enumerate(pdf.pages):
                    text = page.extract_text()
                    if text:
                        logger.info(f"Page {page_num + 1} text preview: {text[:200]}...")
                        
                        # Search for keywords in the full text of the page
                        for keyword in keywords:
                            # Handle multiline keywords (like "By:\nName:")
                            if '\n' in keyword:
                                # Split the keyword into parts
                                keyword_parts = keyword.split('\n')
                                if len(keyword_parts) == 2:
                                    first_part = keyword_parts[0].strip()  # "By:"
                                    second_part = keyword_parts[1].strip()  # "Name:"
                                    
                                    # Split the page text into lines
                                    lines = text.split('\n')
                                    logger.info(f"Page {page_num + 1} has {len(lines)} lines")
                                    
                                    # Look for consecutive lines that match the pattern
                                    for i in range(len(lines) - 1):
                                        current_line = lines[i].strip()
                                        next_line = lines[i + 1].strip()
                                        
                                        logger.info(f"Checking line {i}: '{current_line}' and line {i+1}: '{next_line}'")
                                        
                                        # Check if we have a "By:" line followed by a "Name:" line
                                        if ("by:" in current_line.lower() and "name:" in next_line.lower()):
                                            logger.info(f"Found By:/Name: pattern at lines {i} and {i+1}")
                                            
                                            # Check if "By:" line is blank (only contains "By:" and optional whitespace)
                                            by_content = current_line.lower().replace("by:", "").strip()
                                            name_content = next_line.lower().replace("name:", "").strip()
                                            
                                            logger.info(f"By: content: '{by_content}', Name: content: '{name_content}'")
                                            
                                            # Apply skip_non_empty logic if enabled
                                            should_skip = False
                                            if config.skip_non_empty:
                                                if by_content != "" or name_content != "":
                                                    should_skip = True
                                                    logger.info(f"Skipping due to skip_non_empty=True - By: or Name: has content")
                                            
                                            # Only proceed if we're not skipping due to content
                                            if not should_skip:
                                                logger.info("Processing this By:/Name: pattern - looking for word positions")
                                                
                                                # Find the position of "By:" in the PDF words
                                                words = page.extract_words()
                                                for word in words:
                                                    if "by:" in word['text'].lower():
                                                        logger.info(f"Found 'By:' word at ({word['x0']}, {word['top']}): '{word['text']}'")
                                                        
                                                        # Get signature size for positioning calculation
                                                        if config.signature_size is not None:
                                                            actual_signature_size = config.signature_size
                                                        else:
                                                            # For positioning, calculate proper PDF points from image
                                                            signature_image_path = os.path.join(config.working_folder, config.signature_filename)
                                                            with Image.open(signature_image_path) as sig_img:
                                                                img_width_px, img_height_px = sig_img.size
                                                                # Get DPI - default to 72 if not available
                                                                dpi = getattr(sig_img, 'info', {}).get('dpi', (72, 72))
                                                                if isinstance(dpi, tuple):
                                                                    dpi_x, dpi_y = dpi
                                                                else:
                                                                    dpi_x = dpi_y = dpi
                                                                # Convert to PDF points
                                                                img_width_points = (img_width_px * 72) / dpi_x
                                                                img_height_points = (img_height_px * 72) / dpi_y
                                                                actual_signature_size = (img_width_points, img_height_points)
                                                        
                                                        # Calculate signature position based on config
                                                        signature_x, signature_y = self._calculate_signature_position(
                                                            word, actual_signature_size, config.signature_position
                                                        )
                                                        
                                                        locations.append((
                                                            page_num,
                                                            signature_x,
                                                            signature_y
                                                        ))
                                                        logger.info(f"Added signature location at ({signature_x}, {signature_y}) - position: {config.signature_position}")
                                                        break
                                            else:
                                                logger.info(f"Skipping - By='{by_content}', Name='{name_content}' (skip_non_empty={config.skip_non_empty})")
                            else:
                                # Single line keyword - find all occurrences
                                if keyword.lower() in text.lower():
                                    words = page.extract_words()
                                    keyword_words = keyword.lower().split()
                                    
                                    # Split the page text into lines for checking content
                                    lines = text.split('\n')
                                    
                                    # Look for ALL occurrences of the keyword phrase
                                    for i, word in enumerate(words):
                                        if keyword_words[0] in word['text'].lower():
                                            # Check if this is the start of our keyword phrase
                                            match_found = True
                                            if len(keyword_words) > 1:
                                                # For multi-word keywords, check subsequent words
                                                for j, kw in enumerate(keyword_words[1:], 1):
                                                    if (i + j < len(words) and 
                                                        kw in words[i + j]['text'].lower()):
                                                        continue
                                                    else:
                                                        match_found = False
                                                        break
                                            
                                            if match_found:
                                                # Apply skip_non_empty logic for single keywords
                                                should_skip = False
                                                if config.skip_non_empty:
                                                    # Find the next word horizontally after this keyword to check for content
                                                    word_y = word['top']
                                                    word_x_end = word['x1']  # Right edge of the current word
                                                    
                                                    # Get all words on the same line (within 5 pixels vertically)
                                                    all_words = page.extract_words()
                                                    same_line_words = []
                                                    for w in all_words:
                                                        if abs(w['top'] - word_y) <= 5:  # Same line tolerance
                                                            same_line_words.append(w)
                                                    
                                                    # Sort by horizontal position
                                                    same_line_words.sort(key=lambda w: w['x0'])
                                                    
                                                    # Find words that come immediately after this keyword word
                                                    content_after_keyword = []
                                                    for w in same_line_words:
                                                        if w['x0'] > word_x_end:  # Word is to the right of current keyword
                                                            # Check if this word is close enough to be considered part of the same field
                                                            # (within reasonable distance, but stop at the next "By:" if present)
                                                            if w['text'].lower().strip() == 'by:':
                                                                # Stop if we encounter another "By:" field
                                                                break
                                                            elif w['x0'] - word_x_end <= 100:  # Within 100 pixels (reasonable gap)
                                                                content_after_keyword.append(w['text'])
                                                            else:
                                                                # Large gap, probably end of this field
                                                                break
                                                    
                                                    # Check if there's meaningful content after the keyword
                                                    content_text = ' '.join(content_after_keyword).strip()
                                                    if content_text and content_text.lower() not in ['', 'by:', 'name:']:
                                                        should_skip = True
                                                        logger.info(f"Skipping '{keyword}' at ({word['x0']}, {word['top']}) due to skip_non_empty=True")
                                                        logger.info(f"Content after keyword: '{content_text}'")
                                                    else:
                                                        logger.info(f"No content after '{keyword}' at ({word['x0']}, {word['top']}) - will place signature")
                                                        logger.info(f"Content found: '{content_text}'")
                                                
                                                if not should_skip:
                                                    # Get signature size for positioning calculation
                                                    if config.signature_size is not None:
                                                        actual_signature_size = config.signature_size
                                                    else:
                                                        # For positioning, calculate proper PDF points from image
                                                        signature_image_path = os.path.join(config.working_folder, config.signature_filename)
                                                        with Image.open(signature_image_path) as sig_img:
                                                            img_width_px, img_height_px = sig_img.size
                                                            # Get DPI - default to 72 if not available
                                                            dpi = getattr(sig_img, 'info', {}).get('dpi', (72, 72))
                                                            if isinstance(dpi, tuple):
                                                                dpi_x, dpi_y = dpi
                                                            else:
                                                                dpi_x = dpi_y = dpi
                                                            # Convert to PDF points
                                                            img_width_points = (img_width_px * 72) / dpi_x
                                                            img_height_points = (img_height_px * 72) / dpi_y
                                                            actual_signature_size = (img_width_points, img_height_points)
                                                    
                                                    # Calculate signature position based on config
                                                    signature_x, signature_y = self._calculate_signature_position(
                                                        word, actual_signature_size, config.signature_position
                                                    )
                                                    
                                                    locations.append((
                                                        page_num,
                                                        signature_x,
                                                        signature_y
                                                    ))
                                                    logger.info(f"Found keyword '{keyword}' on page {page_num + 1} at position ({word['x0']}, {word['top']})")
                                                    logger.info(f"Placed signature at ({signature_x}, {signature_y}) - position: {config.signature_position}")
            
            logger.info(f"Total keyword locations found: {len(locations)}")
            return locations
        except Exception as e:
            logger.error(f"Error finding keyword locations: {str(e)}")
            return []

    def _calculate_signature_position(self, word: dict, signature_size: tuple[float, float], position: str) -> tuple[float, float]:
        """
        Calculate the signature position relative to the keyword word.
        
        Args:
            word: Word dictionary containing position information (x0, x1, top, bottom)
            signature_size: Tuple of (width, height) for the signature
            position: Position relative to keyword ("top", "bottom", "left", "right")
            
        Returns:
            Tuple of (x_coord, y_coord) for signature placement
        """
        word_x0 = float(word['x0'])
        word_x1 = float(word['x1']) 
        word_top = float(word['top'])
        word_bottom = float(word['bottom'])
        
        sig_width, sig_height = signature_size
        padding = 10  # Padding between keyword and signature
        
        if position.lower() == "top":
            # Place signature above the keyword
            signature_x = word_x0
            signature_y = word_top - sig_height - padding
        elif position.lower() == "bottom":
            # Place signature below the keyword
            signature_x = word_x0
            signature_y = word_bottom + padding
        elif position.lower() == "left":
            # Place signature to the left of the keyword
            signature_x = word_x0 - sig_width - padding
            signature_y = word_top
        elif position.lower() == "right":
            # Place signature to the right of the keyword
            signature_x = word_x1 + padding
            signature_y = word_top
        else:
            # Default to top if invalid position specified
            logger.warning(f"Invalid signature position '{position}', defaulting to 'top'")
            signature_x = word_x0
            signature_y = word_top - sig_height - padding
        
        # Ensure coordinates are not negative
        signature_x = max(0, signature_x)
        signature_y = max(0, signature_y)
        
        return signature_x, signature_y

    def _prepare_signature_for_pdf(self, config: SignatureConfig) -> tuple[str, bytes]:
        """
        Prepare signature image for PDF insertion - uses original image as-is for maximum quality.
        
        Args:
            config: SignatureConfig object
            
        Returns:
            tuple: (image_path, image_bytes) for direct PDF insertion
        """
        try:
            signature_image_path = os.path.join(config.working_folder, config.signature_filename)
            
            # Always use the original image without any processing
            logger.info("Using original signature image without any processing for maximum quality")
            
            # When signature_size is None, we use the image completely as-is
            if config.signature_size is None:
                logger.info("signature_size=None, will use image as-is with its natural size")
            else:
                logger.info(f"Using specified signature_size: {config.signature_size}")
            
            # Read the original image bytes
            with open(signature_image_path, 'rb') as f:
                image_bytes = f.read()
            
            logger.info("Signature prepared using original file - no quality loss")
            return signature_image_path, image_bytes
                
        except Exception as e:
            logger.error(f"Error preparing signature: {str(e)}")
            raise

    def _add_signature_to_pdf(self, config: SignatureConfig) -> str:
        """
        Add signature to PDF using original image for maximum quality.
        
        Args:
            config: SignatureConfig object containing the configuration
            
        Returns:
            str: Path to the output PDF file
        """
        try:
            # Build full paths
            pdf_path = os.path.join(config.working_folder, config.input_pdf_filename)
            
            # Open the PDF
            pdf_document = fitz.open(pdf_path)
            
            # Prepare signature image (uses original image as-is)
            signature_path, signature_bytes = self._prepare_signature_for_pdf(config)
            
            logger.info(f"Prepared signature - size mode: {'Natural DPI-based sizing' if config.signature_size is None else f'Specified size: {config.signature_size}'}")
            
            # Find signature placement locations
            locations = []
            if config.keywords:
                logger.info(f"Searching for keywords: {config.keywords}")
                locations = self._find_keyword_locations(pdf_path, config.keywords, config)
                logger.info(f"Found {len(locations)} keyword locations")
            
            if not locations and config.x_coord is not None and config.y_coord is not None:
                logger.info("Using explicit coordinates")
                pages = config.page_numbers if config.page_numbers else [0]
                for page_num in pages:
                    locations.append((page_num, config.x_coord, config.y_coord))
            
            # Remove duplicates
            unique_locations = list(set(locations))
            logger.info(f"Adding signatures to {len(unique_locations)} locations")
            
            # Insert signatures using direct image bytes for maximum quality
            for page_num, x, y in unique_locations:
                page = pdf_document[page_num]
                
                if config.signature_size is None:
                    # When signature_size=None, use image as-is with natural size
                    logger.info(f"Adding signature to page {page_num + 1} at ({x}, {y}) using image's natural size")
                    
                    # Get the image dimensions in pixels to convert to PDF points properly
                    signature_image_path = os.path.join(config.working_folder, config.signature_filename)
                    with Image.open(signature_image_path) as sig_img:
                        img_width_px, img_height_px = sig_img.size
                        
                        # Get DPI - default to 72 if not available (PDF standard)
                        dpi = getattr(sig_img, 'info', {}).get('dpi', (72, 72))
                        if isinstance(dpi, tuple):
                            dpi_x, dpi_y = dpi
                        else:
                            dpi_x = dpi_y = dpi
                        
                        # Convert pixels to points (1 point = 1/72 inch)
                        img_width_points = (img_width_px * 72) / dpi_x
                        img_height_points = (img_height_px * 72) / dpi_y
                        
                        logger.info(f"Image: {img_width_px}x{img_height_px}px at {dpi_x}x{dpi_y} DPI = {img_width_points:.1f}x{img_height_points:.1f} points")
                    
                    # Create rectangle using proper PDF points
                    rect = fitz.Rect(x, y, x + img_width_points, y + img_height_points)
                    
                    # Insert image with exact sizing
                    try:
                        page.insert_image(rect, stream=signature_bytes, keep_proportion=False)
                        logger.info(f"Inserted signature using image bytes at natural size ({img_width_points:.1f}x{img_height_points:.1f} points)")
                    except Exception as img_error:
                        logger.warning(f"Bytes insertion failed, using file fallback: {img_error}")
                        page.insert_image(rect, filename=signature_path, keep_proportion=False)
                else:
                    # When signature_size is specified, use the specified dimensions
                    sig_width, sig_height = config.signature_size
                    logger.info(f"Adding signature to page {page_num + 1} at ({x}, {y}) with specified size {config.signature_size}")
                    
                    # Create precise rectangle using specified signature size
                    rect = fitz.Rect(x, y, x + sig_width, y + sig_height)
                    
                    # Insert image using bytes for maximum quality
                    try:
                        # Use image bytes directly for best quality
                        page.insert_image(rect, stream=signature_bytes, keep_proportion=False)
                        logger.info(f"Inserted signature using image bytes with specified dimensions")
                    except Exception as img_error:
                        # Fallback to file-based insertion
                        logger.warning(f"Bytes insertion failed, using file fallback: {img_error}")
                        page.insert_image(rect, filename=signature_path, keep_proportion=False)
            
            # Save PDF with optimal quality settings
            output_filename = f"signed_{config.input_pdf_filename}"
            output_path = config.output_path or os.path.join(config.working_folder, output_filename)
            
            # Save with no image compression to preserve original quality
            pdf_document.save(output_path, deflate_images=False, deflate=False)
            pdf_document.close()
            
            logger.info(f"PDF saved with original image quality to: {output_path}")
            return output_path
            
        except Exception as e:
            logger.error(f"Error adding signature: {str(e)}")
            raise

    def process_documents(self, configs: List[SignatureConfig]) -> List[SignatureResult]:
        """
        Process multiple PDF documents with signatures.
        
        Args:
            configs: List of SignatureConfig objects
            
        Returns:
            List[SignatureResult]: Results of the processing
        """
        results = []
        
        for config in configs:
            try:
                if not self._validate_input(config):
                    raise ValueError("Invalid input configuration")
                
                output_path = self._add_signature_to_pdf(config)
                
                results.append(SignatureResult(
                    input_pdf_path=os.path.join(config.working_folder, config.input_pdf_filename),
                    item_id=config.item_id,
                    output_pdf_path=output_path,
                    success=True
                ))
                
                logger.info(f"Successfully processed document: {config.input_pdf_filename}")
                
            except Exception as e:
                logger.error(f"Error processing document {config.input_pdf_filename}: {str(e)}")
                results.append(SignatureResult(
                    input_pdf_path=os.path.join(config.working_folder, config.input_pdf_filename),
                    item_id=config.item_id,
                    output_pdf_path="",
                    success=False,
                    error_message=str(e)
                ))
        
        return results

    def _get_actual_signature_size(self, config: SignatureConfig) -> tuple[float, float]:
        """
        Get the actual size the signature will be when placed in the PDF.
        Only used when signature_size is explicitly specified.
        
        Args:
            config: SignatureConfig object
            
        Returns:
            Tuple of (width, height) in points for the signature
        """
        if config.signature_size is not None:
            logger.info(f"Using specified signature_size: {config.signature_size}")
            return config.signature_size
        else:
            # This method should not be called when signature_size is None
            raise ValueError("_get_actual_signature_size called when signature_size is None")

if __name__ == "__main__":
    import sys
    import json
    
    # Check if command line arguments are provided (for C# integration)
    if len(sys.argv) > 1:
        try:
            # Get JSON file path from command line argument
            json_file_path = sys.argv[1]
            
            # Read JSON data from file
            with open(json_file_path, 'r', encoding='utf-8') as f:
                json_data = f.read()
            
            configs_data = json.loads(json_data)
            
            # Convert JSON data to SignatureConfig objects
            configs = []
            for config_data in configs_data:
                config = SignatureConfig(
                    working_folder=config_data.get('workingFolder', ''),
                    input_pdf_filename=config_data.get('inputPdfFilename', ''),
                    item_id=config_data.get('itemId', ''),
                    signature_filename=config_data.get('signatureFilename', ''),
                    keywords=config_data.get('keywords', []),
                    signature_size=tuple(config_data['signatureSize']) if config_data.get('signatureSize') else None,
                    output_path=config_data.get('outputPath'),
                    x_coord=config_data.get('xCoord'),
                    y_coord=config_data.get('yCoord'),
                    page_numbers=config_data.get('pageNumbers'),
                    skip_non_empty=config_data.get('skipNonEmpty', False),
                    signature_position=config_data.get('signaturePosition', 'top')
                )
                configs.append(config)
            
            # Process the documents
            processor = PDFSignatureProcessor()
            results = processor.process_documents(configs)
            
            # Convert results to JSON format for C# consumption
            json_results = []
            for result in results:
                json_results.append({
                    'inputPdfPath': result.input_pdf_path,
                    'itemId': result.item_id,
                    'outputPdfPath': result.output_pdf_path,
                    'success': result.success,
                    'errorMessage': result.error_message
                })
            
            # Print JSON results to stdout for C# to read
            print(json.dumps(json_results))
            
        except Exception as e:
            # Print error in JSON format
            error_result = [{
                'inputPdfPath': '',
                'itemId': '',
                'outputPdfPath': '',
                'success': False,
                'errorMessage': str(e)
            }]
            print(json.dumps(error_result))
            sys.exit(1)
    else:
        # Original example usage when run directly
        processor = PDFSignatureProcessor()
        config = SignatureConfig(
            working_folder="C:\\Users\\KarthikMahalingam\\OneDrive - C-Mart Solutions Ltd\\Documents",
            input_pdf_filename="sign.pdf",
            item_id="test_signature",
            signature_filename="signature.png",
            keywords=["by:"],  # Simple keyword search
            
            # SIZE OPTIONS:
            # When signature_size=None: Uses image as-is with proper DPI conversion
            # This preserves the image's natural size and quality in the PDF
            signature_size=None,  # Uses original image with proper DPI conversion
            
            x_coord=None,
            y_coord=None,
            skip_non_empty=False,
            signature_position="top"  # Options: "top", "bottom", "left", "right"
        )
        # config1 = SignatureConfig(
        #     working_folder="C:\\Users\\Gomathi\\Downloads",
        #     input_pdf_filename="ip1.pdf",
        #     item_id="test_signature",
        #     signature_filename="signature.png",
        #     keywords=["AUTHORIZED SIGNATURE", "sign here"],
        #     signature_size=(120, 40),  # Size is in pixels (px)
        #     x_coord=None,  # Will default to bottom-right if no keywords found
        #     y_coord=None   # Will default to bottom-right if no keywords found
        # )
        # config2 = SignatureConfig(
        #     working_folder="C:\\Users\\Gomathi\\Downloads",
        #     input_pdf_filename="ip2.pdf",
        #     item_id="test_signature",
        #     signature_filename="signature.png",
        #     keywords=["AUTHORIZED SIGNATURE", "sign here"],
        #     signature_size=(120, 40),
        #     x_coord=None,
        #     y_coord=None
        # )
        # config3 = SignatureConfig(
        #     working_folder="C:\\Users\\Gomathi\\Downloads",
        #     input_pdf_filename="ip3.pdf",
        #     item_id="test_signature",
        #     signature_filename="signature.png",
        #     keywords=["AUTHORIZED SIGNATURE", "sign here"],
        #     signature_size=(120, 40),
        #     x_coord=None,
        #     y_coord=None
        # )
        # config4 = SignatureConfig(
        #     working_folder="C:\\Users\\Gomathi\\Downloads",
        #     input_pdf_filename="ip4.pdf",
        #     item_id="test_signature",
        #     signature_filename="signature.png",
        #     keywords=["AUTHORIZED SIGNATURE", "sign here"],
        #     signature_size=(120, 40),
        #     x_coord=None,
        #     y_coord=None
        # )
        # config5 = SignatureConfig(
        #     working_folder="C:\\Users\\Gomathi\\Downloads",
        #     input_pdf_filename="ip5.pdf",
        #     item_id="test_signature",
        #     signature_filename="signature.png",
        #     keywords=["AUTHORIZED SIGNATURE", "sign here"],
        #     signature_size=(120, 40),
        #     x_coord=None,
        #     y_coord=None
        # )
        
        results = processor.process_documents([config]);#,config1, config2, config3, config4, config5])
        for result in results:
            print(f"Processed {result.input_pdf_path}: {'Success' if result.success else f'Failed - {result.error_message}'}")

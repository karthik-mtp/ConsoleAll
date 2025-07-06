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
    signature_size: Optional[tuple[float, float]] = None
    output_path: Optional[str] = None
    x_coord: Optional[float] = None
    y_coord: Optional[float] = None
    page_numbers: Optional[List[int]] = None

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

    def _find_keyword_locations(self, pdf_path: str, keywords: List[str]) -> List[tuple[int, float, float]]:
        """
        Find the locations of keywords in the PDF.
        
        Args:
            pdf_path: Path to the PDF file
            keywords: List of keywords to search for
            
        Returns:
            List of tuples containing (page_number, x_coord, y_coord)
        """
        locations = []
        try:
            with pdfplumber.open(pdf_path) as pdf:
                for page_num, page in enumerate(pdf.pages):
                    text = page.extract_text()
                    if text:
                        # Search for keywords in the full text of the page
                        for keyword in keywords:
                            if keyword.lower() in text.lower():
                                # Find the position of the keyword using character-level search
                                words = page.extract_words()
                                keyword_words = keyword.lower().split()
                                
                                # Look for the first word of the keyword phrase
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
                                            # Place signature above the keyword
                                            signature_y = float(word['top']) - config.signature_size[1] - 10  # 10 pixels above
                                            locations.append((
                                                page_num,
                                                float(word['x0']),
                                                signature_y
                                            ))
                                            logger.info(f"Found keyword '{keyword}' on page {page_num + 1} at position ({word['x0']}, {word['top']}), placing signature above at ({word['x0']}, {signature_y})")
            
            logger.info(f"Total keyword locations found: {len(locations)}")
            return locations
        except Exception as e:
            logger.error(f"Error finding keyword locations: {str(e)}")
            return []

    def _add_signature_to_pdf(self, config: SignatureConfig) -> str:
        """
        Add signature to the PDF at specified locations.
        
        Args:
            config: SignatureConfig object containing the configuration
            
        Returns:
            str: Path to the output PDF file
        """
        try:
            # Build full paths
            pdf_path = os.path.join(config.working_folder, config.input_pdf_filename)
            signature_image_path = os.path.join(config.working_folder, config.signature_filename)
            
            # Open the PDF
            pdf_document = fitz.open(pdf_path)
            
            # Prepare the signature image
            signature_image = Image.open(signature_image_path)
            
            # Default signature size if not specified (200x100 pixels)
            if not config.signature_size:
                config.signature_size = (200, 100)
            
            signature_image = signature_image.resize(
                (int(config.signature_size[0]), int(config.signature_size[1])),
                Image.Resampling.LANCZOS
            )
            
            # Save the resized image temporarily with a unique path
            temp_signature_path = os.path.join(config.working_folder, f"temp_signature_{config.item_id}.png")
            signature_image.save(temp_signature_path, "PNG")
            
            # Determine signature placement
            locations = []
            if config.keywords:
                # Find all keyword locations in the PDF (all pages)
                logger.info(f"Searching for keywords: {config.keywords}")
                locations = self._find_keyword_locations(pdf_path, config.keywords)
                logger.info(f"Found {len(locations)} keyword locations")
            
            if not locations:  # If no keywords found or no keywords specified
                logger.info("No keyword locations found, using default placement")
                pages = config.page_numbers if config.page_numbers else [0]  # Default to first page
                for page_num in pages:
                    page = pdf_document[page_num]
                    # Default to bottom-right corner with some padding
                    x = config.x_coord if config.x_coord is not None else page.rect.width - config.signature_size[0] - 50
                    y = config.y_coord if config.y_coord is not None else page.rect.height - config.signature_size[1] - 50
                    locations.append((page_num, x, y))
            
            # Add signature to each location (all keyword matches on all pages)
            logger.info(f"Adding signatures to {len(locations)} locations")
            for page_num, x, y in locations:
                logger.info(f"Adding signature to page {page_num + 1} at position ({x}, {y})")
                page = pdf_document[page_num]
                # Create rectangle for image placement
                rect = fitz.Rect(
                    x, y,
                    x + config.signature_size[0],
                    y + config.signature_size[1]
                )
                # Insert image with alpha channel support
                page.insert_image(rect, filename=temp_signature_path, keep_proportion=True)
            
            # Save the result with signed_ prefix
            output_filename = f"signed_{config.input_pdf_filename}"
            output_path = config.output_path or os.path.join(config.working_folder, output_filename)
            pdf_document.save(output_path)
            pdf_document.close()
            
            # Clean up temporary file
            if os.path.exists(temp_signature_path):
                os.remove(temp_signature_path)
            
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
                    page_numbers=config_data.get('pageNumbers')
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
            working_folder="C:\\Users\\Gomathi\\Downloads",
            input_pdf_filename="INPUT.pdf",
            item_id="test_signature",
            signature_filename="signature.png",
            keywords=["AUTHORIZED SIGNATURE", "sign here"],
            signature_size=(120, 40),  # Size is in pixels (px)
            x_coord=None,  # Will default to bottom-right if no keywords found
            y_coord=None   # Will default to bottom-right if no keywords found
        )
        config1 = SignatureConfig(
            working_folder="C:\\Users\\Gomathi\\Downloads",
            input_pdf_filename="ip1.pdf",
            item_id="test_signature",
            signature_filename="signature.png",
            keywords=["AUTHORIZED SIGNATURE", "sign here"],
            signature_size=(120, 40),  # Size is in pixels (px)
            x_coord=None,  # Will default to bottom-right if no keywords found
            y_coord=None   # Will default to bottom-right if no keywords found
        )
        config2 = SignatureConfig(
            working_folder="C:\\Users\\Gomathi\\Downloads",
            input_pdf_filename="ip2.pdf",
            item_id="test_signature",
            signature_filename="signature.png",
            keywords=["AUTHORIZED SIGNATURE", "sign here"],
            signature_size=(120, 40),
            x_coord=None,
            y_coord=None
        )
        config3 = SignatureConfig(
            working_folder="C:\\Users\\Gomathi\\Downloads",
            input_pdf_filename="ip3.pdf",
            item_id="test_signature",
            signature_filename="signature.png",
            keywords=["AUTHORIZED SIGNATURE", "sign here"],
            signature_size=(120, 40),
            x_coord=None,
            y_coord=None
        )
        config4 = SignatureConfig(
            working_folder="C:\\Users\\Gomathi\\Downloads",
            input_pdf_filename="ip4.pdf",
            item_id="test_signature",
            signature_filename="signature.png",
            keywords=["AUTHORIZED SIGNATURE", "sign here"],
            signature_size=(120, 40),
            x_coord=None,
            y_coord=None
        )
        config5 = SignatureConfig(
            working_folder="C:\\Users\\Gomathi\\Downloads",
            input_pdf_filename="ip5.pdf",
            item_id="test_signature",
            signature_filename="signature.png",
            keywords=["AUTHORIZED SIGNATURE", "sign here"],
            signature_size=(120, 40),
            x_coord=None,
            y_coord=None
        )
        
        results = processor.process_documents([config,config1, config2, config3, config4, config5])
        for result in results:
            print(f"Processed {result.input_pdf_path}: {'Success' if result.success else f'Failed - {result.error_message}'}")

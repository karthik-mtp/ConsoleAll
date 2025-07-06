#!/usr/bin/env python3

# Test script to verify the skip_non_empty logic for single-line keywords

def test_skip_logic():
    test_lines = [
        "By:",
        "By: ",
        "By: karthik",
        "By: John Doe",
        "By:Smith",
        "Name: By:",
        "By: \n",
        "By: \t",
        "By:\t\n",
        "By:  \t  ",
        "By: Something here"
    ]
    
    keyword = "By:"
    
    print(f"Testing skip_non_empty logic for keyword: '{keyword}'")
    print("=" * 50)
    
    for line in test_lines:
        # Simulate the logic from the PDF processor
        keyword_index = line.lower().find(keyword.lower())
        if keyword_index >= 0:
            # Get everything after the keyword
            after_keyword = line[keyword_index + len(keyword):].strip()
            should_skip = after_keyword != ""
            
            print(f"Line: '{line.replace(chr(10), '\\n').replace(chr(9), '\\t')}'")
            print(f"  After keyword: '{after_keyword}'")
            print(f"  Should skip: {should_skip}")
            print()

if __name__ == "__main__":
    test_skip_logic()

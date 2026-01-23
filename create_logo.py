
import os
from PIL import Image, ImageDraw

def create_logo():
    # Create a simple 100x100 logo
    img = Image.new('RGBA', (100, 100), color=(0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    
    # Draw a blue circle
    d.ellipse([10, 10, 90, 90], fill=(0, 122, 204), outline=None)
    
    # Draw a white 'L'
    d.rectangle([35, 30, 45, 70], fill="white")
    d.rectangle([35, 60, 65, 70], fill="white")
    
    # Ensure directory exists
    os.makedirs("Images", exist_ok=True)
    
    img.save("Images/logo.png")
    print("Logo created at Images/logo.png")

if __name__ == "__main__":
    create_logo()

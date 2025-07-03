# ocr_wrapper.py
import sys
from manga_ocr import MangaOcr

mocr = MangaOcr()
image_path = sys.argv[1]
print(mocr(image_path))

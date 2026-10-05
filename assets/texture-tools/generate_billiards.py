"""Generate deterministic cloth, walnut, and spherical numbered-ball textures."""
from pathlib import Path
import math, random
from PIL import Image, ImageDraw, ImageFont

output = Path(__file__).resolve().parents[2] / 'RayTracer.Core' / 'Textures' / 'Billiards'
output.mkdir(parents=True, exist_ok=True)
rng = random.Random(24)
size = 1024
cloth = Image.new('RGB', (size, size))
pixels = cloth.load()
for y in range(size):
    for x in range(size):
        grain = rng.gauss(0, 3) + (1.2 if (x + y) % 3 == 0 else -0.6)
        pixels[x, y] = tuple(max(0, min(255, int(c + grain))) for c in (19, 103, 76))
cloth.save(output / 'felt.png')
wood = Image.new('RGB', (1024, 256))
pixels = wood.load()
for y in range(256):
    for x in range(1024):
        grain = 8 * math.sin(y * .55 + 4 * math.sin(x * .008)) + 4 * math.sin(y * 1.7 + x * .006) + rng.gauss(0, 1.5)
        pixels[x, y] = tuple(max(0, min(255, int(c + grain))) for c in (88, 43, 23))
wood.save(output / 'walnut.png')
font = ImageFont.truetype('/System/Library/Fonts/Supplemental/Arial Bold.ttf', 145)
colors = [(236,185,20), (22,63,165), (188,27,27), (86,36,126), (225,88,14), (17,101,61), (113,22,31), (18,19,23)]
width, height = 1024, 512
for number in range(1,16):
    color = colors[(number-1)%8]
    label = Image.new('RGB',(256,256),(242,239,225))
    draw = ImageDraw.Draw(label)
    draw.text((128,128),str(number),fill=(14,14,18),font=font,anchor='mm')
    image = Image.new('RGB',(width,height))
    pixels=image.load()
    # Number medallions on opposite sides; front one tilts upward toward the camera.
    centers=[(0,.5,-math.sqrt(.75)),(0,-.5,math.sqrt(.75))]
    for y in range(height):
        latitude=math.pi*(.5-y/height)
        py=math.sin(latitude)
        for x in range(width):
            longitude=2*math.pi*(x/width-.5)
            px,pz=math.cos(latitude)*math.cos(longitude),math.cos(latitude)*math.sin(longitude)
            base=color if number<=8 or abs(py)<.43 else (242,239,225)
            for cx,cy,cz in centers:
                dot=py*cy+pz*cz
                if dot>math.cos(.42):
                    horizontal=px if cz>0 else -px
                    vertical=py*(-cz)+pz*cy
                    lx=max(0,min(255,int(128+horizontal/.41*128)))
                    ly=max(0,min(255,int(128-vertical/.41*128)))
                    base=label.getpixel((lx,ly))
                    break
            pixels[x,y]=base
    image.save(output / f'ball-{number:02}.png')
print(output)

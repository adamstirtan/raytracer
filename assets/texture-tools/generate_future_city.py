"""Generate deterministic architectural and landscape maps for Future City."""
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw

output = Path(__file__).resolve().parents[2] / 'RayTracer.Core' / 'Textures' / 'FutureCity'
output.mkdir(parents=True, exist_ok=True)
rng = np.random.default_rng(42)

for name, base in [('alloy', (51, 75, 103)), ('ceramic', (177, 194, 199)),
                   ('dark', (26, 37, 51)), ('gold', (181, 104, 35))]:
    image = Image.new('RGB', (512, 512), base)
    draw = ImageDraw.Draw(image)
    for y in range(0, 512, 128):
        for x in range(0, 512, 128):
            variation = int(rng.integers(-8, 9))
            fill = tuple(max(0, c + variation) for c in base)
            draw.rectangle((x+2,y+2,x+125,y+125), fill=fill, outline=(36,48,61), width=2)
            draw.line((x+7,y+8,x+119,y+8), fill=tuple(min(255,c+16) for c in base))
            for offset in (10, 115):
                draw.ellipse((x+offset,y+115,x+offset+3,y+118), fill=(120,135,145))
            if name == 'alloy':
                for offset in range(80,111,5):
                    draw.line((x+80,y+offset,x+110,y+offset), fill=(18,27,37), width=2)
    image.save(output / f'{name}.png')

image = Image.new('RGB',(512,512),(18,31,46))
draw = ImageDraw.Draw(image)
for y in range(0,512,64):
    for x in range(0,512,64):
        fill = (20,48,70) if rng.random() < .45 else ((72,143,169) if rng.random() < .75 else (192,141,75))
        draw.rectangle((x+5,y+6,x+57,y+54), fill=fill, outline=(51,69,81), width=2)
        draw.line((x+31,y+6,x+31,y+54), fill=(20,34,47), width=2)
        draw.line((x+6,y+12,x+55,y+12), fill=tuple(min(255,c+20) for c in fill))
        draw.rectangle((x,y+59,x+63,y+63), fill=(64,75,86))
image.save(output/'windows.png')

image = Image.new('RGB',(1024,1024),(38,49,63))
draw = ImageDraw.Draw(image)
for y in range(0,1024,128):
    for x in range(0,1024,128):
        shade = int(rng.integers(-4,5))
        draw.rectangle((x+3,y+3,x+124,y+124),fill=(48+shade,61+shade,76+shade),outline=(76,88,98),width=2)
        draw.line((x+9,y+9,x+114,y+9),fill=(90,101,110))
        for offset in range(30,100,8):
            draw.line((x+102,y+offset,x+115,y+offset),fill=(25,35,45),width=2)
image.save(output/'paving.png')

width,height=1024,512
u,v=np.meshgrid(np.arange(width)/width,np.arange(height)/height)
grain=np.zeros_like(u)
for octave in range(7):
    f=2**octave
    grain+=np.sin(2*np.pi*(f*3*u+f*2*v)+rng.random()*6.28)/(2**(octave*.65))
mineral=np.zeros_like(u)
for resolution, strength in [(8,20),(24,14),(64,8),(192,4)]:
    samples=Image.fromarray(rng.integers(0,256,(resolution,resolution),dtype=np.uint8))
    smooth=np.asarray(samples.resize((width,height),Image.Resampling.BICUBIC)).astype(float)
    mineral+=(smooth/127.5-1)*strength
rock=np.array([88,64,75])+mineral[...,None]
strata=np.sin(v*200+mineral*.2)
rock+=strata[...,None]*2
Image.fromarray(np.uint8(np.clip(rock,0,255))).save(output/'rock.png')

# Latitude-longitude glazing with metallic ribs for the curved domes.
glass=np.zeros((height,width,3))+np.array([30,72,104])
glass+=(np.sin(u*2*np.pi*32)*3)[...,None]
ribs=(u*32 % 1 < .035) | (v*16 % 1 < .045)
glass[ribs]=[120,144,154]
Image.fromarray(np.uint8(glass)).save(output/'dome.png')

bands=.5+.5*np.sin(v*50+np.sin(u*25)*.5+grain*.25)
planet=np.array([100,56,45])+bands[...,None]*np.array([95,75,50])
Image.fromarray(np.uint8(np.clip(planet,0,255))).save(output/'planet.png')

# An emissive distant sky preserves the gradient without receiving scene shadows.
sky=np.array([9,17,35])+(v**2)[...,None]*np.array([69,37,34])
Image.fromarray(np.uint8(sky)).save(output/'sky.png')
print(output)

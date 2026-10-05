"""Create the Synthwave scene's sky, sun, and luminous grid textures."""
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw

output = Path(__file__).resolve().parents[2] / 'RayTracer.Core' / 'Textures' / 'Synthwave'
output.mkdir(parents=True, exist_ok=True)
rng = np.random.default_rng(1984)
u,v=np.meshgrid(np.arange(2048)/2048,np.arange(1024)/1024)
stops=np.array([[13,3,42],[47,3,71],[152,0,115],[251,13,128],[255,137,80]])
t=np.clip(v*4,0,3.999)
i=t.astype(int)
sky=stops[i]*(1-(t-i)[...,None])+stops[i+1]*(t-i)[...,None]
glow=np.exp(-(((u-.5)/.12)**2+((v-.55)/.2)**2))
sky+=glow[...,None]*np.array([48,5,22])
sky=np.clip(sky,0,255)
image=Image.fromarray(np.uint8(sky))
draw=ImageDraw.Draw(image)
for _ in range(1400):
    x,y=int(rng.integers(2048)),int(rng.integers(1024))
    light=int(rng.integers(100,235))
    draw.point((x,y),fill=(light,light,min(255,light+15)))
for _ in range(24):
    x,y=int(rng.integers(2048)),int(rng.integers(750))
    draw.line((x-3,y,x+3,y),fill=(130,100,166))
    draw.line((x,y-3,x,y+3),fill=(130,100,166))
    draw.point((x,y),fill=(255,235,255))
image.save(output/'sky.png')

u,v=np.meshgrid(np.arange(1024)/1024,np.arange(1024)/1024)
sun=np.array([255,12,111])+(v**1.8)[...,None]*np.array([0,91,57])
center=np.exp(-(((u-.5)/.35)**2+((v-.42)/.5)**2))
sun+=center[...,None]*np.array([0,19,23])
Image.fromarray(np.uint8(np.clip(sun,0,255))).save(output/'sun.png')

u,v=np.meshgrid(np.arange(1024)/1024,np.arange(1024)/1024)
d=np.minimum(np.minimum(u,1-u),np.minimum(v,1-v))
core=np.exp(-(d/.008)**2)
halo=np.exp(-(d/.032)**2)
grid=np.array([21,3,48])+core[...,None]*np.array([0,113,210])+halo[...,None]*np.array([3,25,48])
Image.fromarray(np.uint8(np.clip(grid,0,255))).save(output/'grid.png')
print(output)

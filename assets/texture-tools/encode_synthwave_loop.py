"""Encode a periodic 30 fps sequence into a 1080p looping MP4.

Usage: python encode_synthwave_loop.py FRAMES_DIRECTORY OUTPUT.mp4
Requires ffmpeg on PATH or the imageio-ffmpeg Python package.
Frames are encoded directly with no fades or duplicate endpoint frames.
"""
import shutil
import subprocess
import sys
from pathlib import Path

ffmpeg = shutil.which('ffmpeg')
if not ffmpeg:
    import imageio_ffmpeg
    ffmpeg = imageio_ffmpeg.get_ffmpeg_exe()

frames = Path(sys.argv[1])
output = Path(sys.argv[2])
subprocess.run([ffmpeg, '-y', '-framerate', '30', '-i', str(frames/'frame-%05d.png'),
                '-vf', 'setsar=1,format=yuv420p', '-an', '-c:v', 'libx264',
                '-preset', 'slow', '-crf', '16', '-r', '30', '-video_track_timescale', '30000',
                '-movflags', '+faststart', str(output)], check=True)

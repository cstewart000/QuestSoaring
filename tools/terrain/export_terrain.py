#!/usr/bin/env python3
"""Export terrain heightmap PNG + OBJ mesh (no Unity). Matches HeightNoise params."""
import math
import struct
import sys
import zlib

SCALE, OCTAVES, MAX_H = 680.0, 5, 460.0
RES, CHUNK = 64, 256


def smooth(t, a, b):
    t = max(0.0, min(1.0, (t - a) / (b - a)))
    return t


def valley_mask(x, z):
    macro = perlin(x * 0.00085, z * 0.00085)
    return smooth(macro, 0.42, 0.58) * (1 - smooth(macro, 0.68, 0.82))


def river_mask(x, z):
    def ch(ox, oz):
        c = perlin(x * 0.0024 + ox, z * 0.0024 + oz)
        return max(0.0, min(1.0, 1 - abs(c - 0.5) * 9))
    r = max(ch(12, 8), ch(140, 95), ch(260, 180))
    return r * r


def sample_height(x, z):
    h, amp, freq, amp_sum = 0.0, 1.0, 1.0 / SCALE, 0.0
    for _ in range(OCTAVES):
        p = perlin(x * freq, z * freq)
        ridged = 1 - abs(p * 2 - 1)
        n = ridged * 0.55 + p * 0.45
        h += n * amp
        amp_sum += amp
        amp *= 0.52
        freq *= 2.05
    base = (max(0.0, min(1.0, h / amp_sum)) ** 1.04) * MAX_H
    valley = valley_mask(x, z)
    river = river_mask(x, z)
    carve = valley * 75 + river * max(60, 130 - base * 0.12)
    return max(1.2, base - carve)


def fade(t):
    return t * t * t * (t * (t * 6 - 15) + 10)


def lerp(a, b, t):
    return a + t * (b - a)


def grad(h, x, y):
    return ((h & 1) and -x or x) + ((h & 2) and -y or y)


perm = list(range(256))
import random
random.seed(42)
random.shuffle(perm)
perm = perm + perm


def hash2(x, y):
    return perm[perm[x & 255] + (y & 255)] & 15


def perlin(x, y):
    xi, yi = int(math.floor(x)) & 255, int(math.floor(y)) & 255
    xf, yf = x - math.floor(x), y - math.floor(y)
    u, v = fade(xf), fade(yf)
    aa = grad(hash2(xi, yi), xf, yf)
    ba = grad(hash2(xi + 1, yi), xf - 1, yf)
    ab = grad(hash2(xi, yi + 1), xf, yf - 1)
    bb = grad(hash2(xi + 1, yi + 1), xf - 1, yf - 1)
    return lerp(lerp(aa, ba, u), lerp(ab, bb, u), v) * 0.5 + 0.5


def write_png(path, w, h, gray_bytes):
    def chunk(tag, data):
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data) & 0xFFFFFFFF)

    raw = b"".join(b"\x00" + gray_bytes[y * w:(y + 1) * w] for y in range(h))
    ihdr = struct.pack(">IIBBBBB", w, h, 8, 0, 0, 0, 0)
    with open(path, "wb") as f:
        f.write(b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", ihdr) + chunk(b"IDAT", zlib.compress(raw, 9)) + chunk(b"IEND", b""))
    print(f"[export] PNG {path} ({w}x{h})")


def export_region(out_dir, cx=0, cz=0):
    import os
    os.makedirs(out_dir, exist_ok=True)
    ox, oz = cx * CHUNK, cz * CHUNK
    heights = []
    for z in range(RES):
        for x in range(RES):
            wx = ox + x * CHUNK / (RES - 1)
            wz = oz + z * CHUNK / (RES - 1)
            heights.append(sample_height(wx, wz))

    mx = max(heights) or 1.0
    gray = bytes(int(h / mx * 255) for h in heights)
    write_png(f"{out_dir}/heightmap_c{cx}_{cz}.png", RES, RES, gray)

    obj_path = f"{out_dir}/terrain_c{cx}_{cz}.obj"
    with open(obj_path, "w") as f:
        for z in range(RES):
            for x in range(RES):
                i = z * RES + x
                wx = ox + x * CHUNK / (RES - 1)
                wz = oz + z * CHUNK / (RES - 1)
                f.write(f"v {wx:.2f} {heights[i]:.2f} {wz:.2f}\n")
        for z in range(RES - 1):
            for x in range(RES - 1):
                i = z * RES + x + 1
                f.write(f"f {i} {i + RES} {i + 1}\n")
                f.write(f"f {i + 1} {i + RES} {i + RES + 1}\n")
    print(f"[export] OBJ {obj_path} ({RES}x{RES} verts)")


if __name__ == "__main__":
    out = sys.argv[1] if len(sys.argv) > 1 else "tools/terrain/export"
    export_region(out, 0, 0)
    print("[export] Done — open OBJ in Blender or heightmap in any viewer")

/** Height + biomes — matches concept_art/image (1).jpg */
export const MAX_H = 460;
export const SCALE = 680;
export const OCTAVES = 5;

export function sampleHeight(x, z) {
  return applyValleysRivers(baseHeight(x, z), x, z);
}

function baseHeight(x, z) {
  let h = 0, amp = 1, freq = 1 / SCALE, ampSum = 0;
  for (let i = 0; i < OCTAVES; i++) {
    const p = perlin(x * freq, z * freq);
    const ridged = 1 - Math.abs(p * 2 - 1);
    const n = ridged * 0.68 + p * 0.32;
    h += n * amp;
    ampSum += amp;
    amp *= 0.52;
    freq *= 2.08;
  }
  return Math.pow(Math.min(1, h / ampSum), 1.1) * MAX_H;
}

export function forestStipple(x, z, y) {
  if (y > 230 || y < 35) return false;
  const patch = perlin(x * 0.022, z * 0.022);
  const fine = perlin(x * 0.19 + 41, z * 0.19 + 17);
  return patch > 0.46 && fine > 0.58;
}

export function valleyMask(x, z) {
  const macro = perlin(x * 0.00085, z * 0.00085);
  return smooth(macro, 0.42, 0.58) * (1 - smooth(macro, 0.68, 0.82));
}

export function riverMask(x, z) {
  const ch = (ox, oz) => {
    const c = perlin(x * 0.0024 + ox, z * 0.0024 + oz);
    return clamp(1 - Math.abs(c - 0.5) * 9);
  };
  return Math.pow(Math.max(ch(12, 8), ch(140, 95), ch(260, 180)), 2);
}

function applyValleysRivers(baseH, x, z) {
  const carve = valleyMask(x, z) * 75 + riverMask(x, z) * Math.max(60, 130 - baseH * 0.12);
  return Math.max(1.2, baseH - carve);
}

export function biomeColor(x, z, y, slope) {
  const speck = perlin(x * 0.11, z * 0.11);
  if (riverMask(x, z) > 0.28 && y < 155) return gray(0.99);
  if (forestStipple(x, z, y)) return gray(0.24 + speck * 0.1);
  if (y < 125 && slope < 0.32) return gray(0.94);
  if (y > 270) return gray(0.97 + speck * 0.03);
  if (slope > 0.38) return gray(0.9 - slope * 0.12);
  return gray(0.965);
}

const gray = v => { v = clamp(v); return [v, v, v]; };
const clamp = v => Math.max(0, Math.min(1, v));
const smooth = (t, a, b) => clamp((t - a) / (b - a));

function perlin(x, y) {
  const xi = Math.floor(x) & 255, yi = Math.floor(y) & 255;
  const xf = x - Math.floor(x), yf = y - Math.floor(y);
  const u = fade(xf), v = fade(yf);
  return lerp(lerp(grad(hash(xi,yi),xf,yf), grad(hash(xi+1,yi),xf-1,yf), u),
    lerp(grad(hash(xi,yi+1),xf,yf-1), grad(hash(xi+1,yi+1),xf-1,yf-1), v), v) * 0.5 + 0.5;
}

const perm = new Uint8Array(512);
for (let i = 0; i < 256; i++) perm[i] = perm[i + 256] = i;
for (let i = 255; i > 0; i--) { const j = Math.floor(Math.random() * (i + 1)); [perm[i], perm[j]] = [perm[j], perm[i]]; }
const fade = t => t * t * t * (t * (t * 6 - 15) + 10);
const lerp = (a, b, t) => a + t * (b - a);
const hash = (x, y) => perm[perm[x & 255] + (y & 255)] & 15;
const grad = (h, x, y) => ((h & 1) ? -x : x) + ((h & 2) ? -y : y);

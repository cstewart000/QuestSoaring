/** Matches QuestSoaring.Terrain.HeightNoise (octaves, scale 800, *400). */
export function sampleHeight(x, z, scale = 800, octaves = 4) {
  let h = 0, amp = 1, freq = 1 / scale;
  for (let i = 0; i < octaves; i++) {
    h += perlin(x * freq, z * freq) * amp;
    amp *= 0.5;
    freq *= 2;
  }
  return h * 400;
}

function perlin(x, y) {
  const xi = Math.floor(x) & 255, yi = Math.floor(y) & 255;
  const xf = x - Math.floor(x), yf = y - Math.floor(y);
  const u = fade(xf), v = fade(yf);
  const aa = grad(hash(xi, yi), xf, yf);
  const ba = grad(hash(xi + 1, yi), xf - 1, yf);
  const ab = grad(hash(xi, yi + 1), xf, yf - 1);
  const bb = grad(hash(xi + 1, yi + 1), xf - 1, yf - 1);
  return lerp(lerp(aa, ba, u), lerp(ab, bb, u), v) * 0.5 + 0.5;
}

const perm = new Uint8Array(512);
for (let i = 0; i < 256; i++) perm[i] = perm[i + 256] = i;
for (let i = 255; i > 0; i--) {
  const j = Math.floor(Math.random() * (i + 1));
  [perm[i], perm[j]] = [perm[j], perm[i]];
}
const fade = t => t * t * t * (t * (t * 6 - 15) + 10);
const lerp = (a, b, t) => a + t * (b - a);
const hash = (x, y) => perm[perm[x] + y] & 15;
const grad = (h, x, y) => ((h & 1) ? -x : x) + ((h & 2) ? -y : y);

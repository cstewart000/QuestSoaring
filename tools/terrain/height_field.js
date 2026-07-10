import { uplift } from './erosion_uplift.js';
import { erodeGrid, flowAccum } from './erosion_sim.js';
import { perlin } from './erosion_uplift.js';

export const GRID = 33;
export const CHUNK = 256;
export const MAX_H = 420;

const cache = new Map();

export function getChunk(cx, cz) {
  const k = `${cx},${cz}`;
  if (cache.has(k)) return cache.get(k);
  if (cache.size > 20) cache.clear();
  const data = buildChunk(cx, cz);
  cache.set(k, data);
  return data;
}

function buildChunk(cx, cz) {
  const cell = CHUNK / (GRID - 1);
  const ox = cx * CHUNK, oz = cz * CHUNK;
  const h = Array.from({ length: GRID }, () => new Float32Array(GRID));
  for (let z = 0; z < GRID; z++)
    for (let x = 0; x < GRID; x++)
      h[x][z] = uplift(ox + x * cell, oz + z * cell);
  const seed = (cx * 73856093) ^ (cz * 19349663);
  erodeGrid(h, GRID, GRID, seed);
  const flow = flowAccum(h, GRID, GRID);
  console.log(`[TerrainHeightField] Eroded chunk (${cx},${cz})`);
  return { h, flow, ox, oz, cell };
}

export function sampleHeight(x, z) {
  const cx = Math.floor(x / CHUNK), cz = Math.floor(z / CHUNK);
  return bilinear(getChunk(cx, cz).h, x, z);
}

export function sampleFlow(x, z) {
  const cx = Math.floor(x / CHUNK), cz = Math.floor(z / CHUNK);
  return bilinear(getChunk(cx, cz).flow, x, z);
}

function bilinear(grid, wx, wz) {
  const c = getChunk(Math.floor(wx / CHUNK), Math.floor(wz / CHUNK));
  let lx = (wx - c.ox) / c.cell, lz = (wz - c.oz) / c.cell;
  lx = Math.max(0, Math.min(GRID - 1.001, lx));
  lz = Math.max(0, Math.min(GRID - 1.001, lz));
  const x0 = Math.floor(lx), z0 = Math.floor(lz);
  const tx = lx - x0, tz = lz - z0;
  const a = grid[x0][z0], b = grid[Math.min(x0 + 1, GRID - 1)][z0];
  const c0 = grid[x0][Math.min(z0 + 1, GRID - 1)], d = grid[Math.min(x0 + 1, GRID - 1)][Math.min(z0 + 1, GRID - 1)];
  return (1 - tx) * (1 - tz) * a + tx * (1 - tz) * b + (1 - tx) * tz * c0 + tx * tz * d;
}

export function biomeColor(x, z, y, slope, flow) {
  const speck = perlin(x * 0.09, z * 0.09);
  if (flow > 10 && y < 200) return gray(0.84);
  if (y > 220 || y < 40 ? false : perlin(x * 0.018, z * 0.018) > 0.5 && flow < 6) return gray(0.44 + speck * 0.08);
  if (y < 110 && slope < 0.28) return gray(0.76);
  if (y > 260) return gray(0.82 + speck * 0.04);
  if (slope > 0.42) return gray(0.72 - slope * 0.05);
  return gray(0.8);
}

const gray = v => [Math.max(0, Math.min(1, v)), Math.max(0, Math.min(1, v)), Math.max(0, Math.min(1, v))];

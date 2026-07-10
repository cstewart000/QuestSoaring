import { sampleHeight } from './height_noise.js';

const RES = 16;
const CHUNK = 256;

export function buildChunkMesh(coordX, coordZ) {
  const originX = coordX * CHUNK, originZ = coordZ * CHUNK;
  const verts = [], tris = [], colors = [];
  for (let z = 0; z < RES; z++) {
    for (let x = 0; x < RES; x++) {
      const wx = originX + (x * CHUNK) / (RES - 1);
      const wz = originZ + (z * CHUNK) / (RES - 1);
      const wy = sampleHeight(wx, wz);
      verts.push(wx, wy, wz);
      const g = 0.45 + (wy / 400) * 0.35;
      colors.push(g, g, g + 0.02);
    }
  }
  for (let z = 0; z < RES - 1; z++) {
    for (let x = 0; x < RES - 1; x++) {
      const i = z * RES + x;
      tris.push(i, i + RES, i + 1, i + 1, i + RES, i + RES + 1);
    }
  }
  console.log(`[TerrainChunk] Built chunk (${coordX}, ${coordZ})`);
  return { verts, tris, colors, key: `${coordX},${coordZ}` };
}

export function chunkCoords(camX, camZ, radius = 2) {
  const cx = Math.floor(camX / CHUNK), cz = Math.floor(camZ / CHUNK);
  const out = [];
  for (let dz = -radius; dz <= radius; dz++)
    for (let dx = -radius; dx <= radius; dx++)
      out.push([cx + dx, cz + dz]);
  return out;
}

export { CHUNK, RES };

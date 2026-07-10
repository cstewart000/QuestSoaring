import { sampleHeight, heightToGray, MAX_H } from './height_noise.js';

const RES = 16;
const CHUNK = 256;
export { CHUNK, MAX_H };

function pos(originX, originZ, x, z, heights, res) {
  const wx = originX + x * CHUNK / (res - 1);
  const wz = originZ + z * CHUNK / (res - 1);
  return [wx, heights[z * res + x], wz];
}

export function buildChunkMesh(coordX, coordZ) {
  const originX = coordX * CHUNK, originZ = coordZ * CHUNK;
  const heights = new Float32Array(RES * RES);
  for (let z = 0; z < RES; z++)
    for (let x = 0; x < RES; x++) {
      const wx = originX + x * CHUNK / (RES - 1);
      const wz = originZ + z * CHUNK / (RES - 1);
      heights[z * RES + x] = sampleHeight(wx, wz);
    }

  const verts = [], colors = [], tris = [];
  function addTri(x0, z0, x1, z1, x2, z2) {
    const a = pos(originX, originZ, x0, z0, heights, RES);
    const b = pos(originX, originZ, x1, z1, heights, RES);
    const c = pos(originX, originZ, x2, z2, heights, RES);
    const ab = [b[0]-a[0], b[1]-a[1], b[2]-a[2]];
    const ac = [c[0]-a[0], c[1]-a[1], c[2]-a[2]];
    const nx = ab[1]*ac[2]-ab[2]*ac[1], ny = ab[2]*ac[0]-ab[0]*ac[2], nz = ab[0]*ac[1]-ab[1]*ac[0];
    const len = Math.hypot(nx, ny, nz) || 1;
    const slope = 1 - ny / len;
    const avgY = (a[1] + b[1] + c[1]) / 3;
    const col = heightToGray(avgY, slope);
    const i = verts.length / 3;
    verts.push(...a, ...b, ...c);
    colors.push(...col, ...col, ...col);
    tris.push(i, i + 1, i + 2);
  }
  for (let z = 0; z < RES - 1; z++)
    for (let x = 0; x < RES - 1; x++) {
      addTri(x, z, x, z + 1, x + 1, z);
      addTri(x + 1, z, x, z + 1, x + 1, z + 1);
    }
  console.log(`[TerrainChunk] Flat chunk (${coordX}, ${coordZ})`);
  return { verts, tris, colors, key: `${coordX},${coordZ}` };
}

export function chunkCoords(camX, camZ, radius = 2) {
  const cx = Math.floor(camX / CHUNK), cz = Math.floor(camZ / CHUNK);
  const out = [];
  for (let dz = -radius; dz <= radius; dz++)
    for (let dx = -radius; dx <= radius; dx++) out.push([cx + dx, cz + dz]);
  return out;
}

export const RES_EXPORT = RES;

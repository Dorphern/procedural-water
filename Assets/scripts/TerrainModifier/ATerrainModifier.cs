using UnityEngine;
using System.Collections;

public struct ErosionOptions {
	public float 	rainAmount,
					solubility,
					evaporation,
					sedimentCapacity;
	public int 		generations,
					erosionsPerGeneration;
}


public abstract class ATerrainModifier {
	
	protected int width;
	protected int height;

	protected ATerrainGenerator terrainGenerator;

	protected Heightmap terrainHeightmap;		// Final terrain heightmap (initial terrain + erosion)
	protected Heightmap waterHeightmap; 		// Final water heightmap (waterflow + terrain)

	protected Heightmap waterflowMap;			// Water amount on different points (for water)
	protected Heightmap erosionMap;				// Difference in original terrain on points
	
	protected ulong[,] accumulatedHeights;		// Dynamic table containing the accumulated heighs across the terrain, used to find average heights.
	protected Heightmap accHeights;

	protected int totalSize = 0;

	protected readonly ulong PRECISION = 10000;



	public ATerrainModifier(ATerrainGenerator terrainGenerator) {
		this.terrainGenerator = terrainGenerator;
	}

	public void setSize(int width, int height) {
		this.width 	= width;
		this.height = height;
		totalSize = width;
	}
	
	public void setOffset(int x, int y) {
		terrainGenerator.setOffset(x, y);
	}

	public void setScale(float s) {
		terrainGenerator.setScale(s);
	}

	/** Generate the the whole terrain */
	public abstract void generate(ErosionOptions? erosionOptions, int time, float waterAmount);


	public Heightmap getTerrainHeightmap() {
		return terrainHeightmap;
	}

	public Heightmap getWaterHeightmap() {
		return waterHeightmap;
	}

	public Heightmap getWaterflowMap() {
		return waterflowMap;
	}

	public Heightmap getErosionMap() {
		return erosionMap;
	}

	public float GetAverageAreaHeight(int x, int y, int w, int h) {
		w--;
		h--;

		ulong horizontal = 0;
		ulong vertical = 0;
		ulong b = 0;

		if (y > 0) horizontal = accumulatedHeights[x + w, y - 1];
		if (x > 0) vertical = accumulatedHeights[x - 1, y + h];
		if (x > 0 && y > 0) b = accumulatedHeights[x - 1, y - 1];

		ulong acc = accumulatedHeights[x + w, y + h];
		ulong area = ((ulong)w + 1) * ((ulong)h + 1);

		float accu = (float)((acc - horizontal - vertical + b) / area) / PRECISION;

		return accu;
	}


	protected void createAccumulatedMap() {
		// Set accumulated heigthmap
		accumulatedHeights = new ulong[totalSize, totalSize];
		for (int x = 0; x < this.totalSize; x++) {
			for (int y = 0; y < this.totalSize; y++) {

				accumulatedHeights[x, y] = (ulong)(terrainHeightmap.getHeight(x, y) * PRECISION);
				if (x > 0) accumulatedHeights[x, y] += accumulatedHeights[x - 1, y];
				if (y > 0) accumulatedHeights[x, y] += accumulatedHeights[x, y - 1];
				if (x > 0 && y > 0) accumulatedHeights[x, y] -= accumulatedHeights[x - 1, y - 1];
			}
		}
	}
}
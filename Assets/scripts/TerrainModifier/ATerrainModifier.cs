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

	protected readonly ulong PRECISION = 10000;


	public ATerrainModifier(ATerrainGenerator terrainGenerator) {
		this.terrainGenerator = terrainGenerator;
	}

	public void setSize(int width, int height) {
		this.width 	= width;
		this.height = height;
	}
	
	public void setOffset(int x, int y) {
		terrainGenerator.setOffset(x, y);
	}

	public void setScale(float s) {
		//this.scale = s;
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
		/*float numb = 0;
		for (int i = x; i <= w + x; i++){
			for (int j = y; j <= h + y; j++){
				numb += terrainHeightmap.getHeight(i, j);
			}
		}

		numb /= ((w + 1) * (h + 1));*/
		
		/*float horizental = 0; 
		float vertical = 0; 
		float b = 0;

		if (y > 0) horizental = accHeights.getHeight (x + w, y - 1);
		if (x > 0) vertical = accHeights.getHeight (x - 1, y + h);
		if (x > 0 && y > 0) b = accHeights.getHeight (x - 1, y - 1);

		float acc = accHeights.getHeight (x + w, y + h);

		float accu = (acc - horizental - vertical + b) / ((w + 1) * (h + 1));
		*/

		ulong horizontal = 0;
		ulong vertical = 0;
		ulong b = 0;

		if (y > 0) horizontal = accumulatedHeights[x + w, y - 1];
		if (x > 0) vertical = accumulatedHeights[x - 1, y + h];
		if (x > 0 && y > 0) b = accumulatedHeights[x - 1, y - 1];

		ulong acc = accumulatedHeights[x + w, y + h];
		ulong area = ((ulong)w + 1) * ((ulong)h + 1);

		float accu = (float)((acc - horizontal - vertical + b) / area) / PRECISION;
		//Debug.Log ("real: " + numb + " accu: " + accu);
		//Debug.Log ("value: " + acc);

		return accu;
	}


	protected void createAccumulatedMap() {
		// Set accumulated heigthmap
		//accHeights = new Heightmap(width, height, 0f);
		accumulatedHeights = new ulong[width, height];

		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {

				accumulatedHeights[x, y] = (ulong)(terrainHeightmap.getHeight(x, y) * PRECISION);
				if (x > 0) accumulatedHeights[x, y] += accumulatedHeights[x - 1, y];
				if (y > 0) accumulatedHeights[x, y] += accumulatedHeights[x, y - 1];
				if (x > 0 && y > 0) accumulatedHeights[x, y] -= accumulatedHeights[x - 1, y - 1];

				/*
				accHeights.setHeight(x, y, terrainHeightmap.getHeight(x, y));// + erosionMap.getHeight(x, y);
				if (x > 0) accHeights.addHeight(x, y, accHeights.getHeight(x-1, y));
                if (y > 0) accHeights.addHeight(x, y, accHeights.getHeight(x, y-1));		
                if (x > 0 && y > 0) accHeights.addHeight(x, y, -accHeights.getHeight(x-1, y-1));	
                */
			}
		}
	}

	/** @return heightmap of the water in the terrain */
	//public abstract Heightmap modifiedWater();

	/** @return heightmap of the final modified terrain */
	//public abstract Heightmap modifiedTerrain(ErosionOptions erosionOptions);

	/** @return "heightmap" of the errosion (amount of errosion on tiles) */
	//public abstract Heightmap erosionMap();

	/** @return "heightmap" of the waterflow (water amount on different tiles */
	//public abstract Heightmap waterflowMap();


}
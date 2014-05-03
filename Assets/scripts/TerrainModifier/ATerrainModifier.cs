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


	/** @return heightmap of the water in the terrain */
	//public abstract Heightmap modifiedWater();

	/** @return heightmap of the final modified terrain */
	//public abstract Heightmap modifiedTerrain(ErosionOptions erosionOptions);

	/** @return "heightmap" of the errosion (amount of errosion on tiles) */
	//public abstract Heightmap erosionMap();

	/** @return "heightmap" of the waterflow (water amount on different tiles */
	//public abstract Heightmap waterflowMap();


}
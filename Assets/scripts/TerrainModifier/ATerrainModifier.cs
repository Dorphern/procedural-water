using UnityEngine;
using System.Collections;


public abstract class ATerrainModifier {

	protected int offsetX;
	protected int offsetY;
	protected int width;
	protected int height;

	protected ATerrainGenerator terrainGenerator;

	public ATerrainModifier(ATerrainGenerator terrainGenerator) {
		this.terrainGenerator = terrainGenerator;
	}

	public void setSize(int width, int height) {
		this.width 	= width;
		this.height = height;
	}
	
	public void setOffset(int x, int y) {
		this.offsetX = x;
		this.offsetY = y;
	}
	
	public void setScale(float s) {
		//this.scale = s;
	}

	/** @return heightmap of the water in the terrain */
	public abstract Heightmap modifiedWater();

	/** @return heightmap of the final modified terrain */
	public abstract Heightmap modifiedTerrain();

	/** @return "heightmap" of the slopes (degree of declination on tiles) */
	public abstract Heightmap slopeMap();

	/** @return "heightmap" of the errosion (amount of errosion on tiles) */
	public abstract Heightmap errosionMap();

	/** @return "heightmap" of the waterflow (water amount on different tiles */
	public abstract Heightmap waterflowMap();


}
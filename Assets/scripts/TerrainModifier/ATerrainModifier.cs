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
		this.scale = s;
	}

	public abstract Heightmap modifiedWater();
	
	public abstract Heightmap modifiedTerrain();

}
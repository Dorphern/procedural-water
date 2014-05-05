using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public abstract class ATerrainGenerator
{
	// Terrain generation parameters
	protected double frequency = 0.5f;
	protected double lacunarity = 1f;
	protected double persistence = 0.5d;
	protected int octaves = 1;
	protected QualityMode quality = QualityMode.High;
	protected double scale = 2d;
	protected float heightScale = 1f;
	protected Perlin perlin;

	protected int seed;

	protected int offsetX = 0;
	protected int offsetY = 0;

	public ATerrainGenerator(int seed) {
		this.seed = seed;
	}

	protected void setupGenerator() {
		perlin = new Perlin(
			this.frequency, 
			this.lacunarity, 
			this.persistence,
			this.octaves, 
			this.seed, 
			this.quality
		);
	}
	protected virtual float TerrainValue(float x, float y) {
		float v = (float) this.perlin.GetValue(x * this.scale, y * this.scale, 0) * this.heightScale;
		v = (v + 1f) / 2f;
		return v;
	}
	
	public float GetHeight(float x, float y) {
		return TerrainValue(x + offsetX, y + offsetY);
	}


	public virtual float GetRoughHeight(float x, float y, int zoomLevel) {
		float v = TerrainValue(x + offsetX, y + offsetY);
		int usedX = Mathf.FloorToInt((x + offsetX) * Mathf.Pow (0.5f, (float)zoomLevel));
		int usedY = Mathf.FloorToInt((y + offsetY) * Mathf.Pow (0.5f, (float)zoomLevel));
		float v = TerrainValue(usedX, usedY);

		return v;
	}

	public void setOffset(int x, int y) {
		offsetX = x;
		offsetY = y;
	}

	public int getOffsetX() {
		return offsetX;
	}

	public int getOffsetY() {
		return offsetY;
	}
}


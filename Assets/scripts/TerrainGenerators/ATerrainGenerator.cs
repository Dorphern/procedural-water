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

	protected virtual float TerrainValue(int x, int y) {
		float v = (float) this.perlin.GetValue(x * this.scale, y * this.scale, 0) * this.heightScale;
		v = (v + 1f) / 2f;
		return v;
	}
	
	public float GetHeight(int x, int y) {
		this.perlin.OctaveCount = this.octaves;
		return TerrainValue(x, y);
	}

	public float GetRoughHeight(int x, int y, int octaves) {
		this.perlin.OctaveCount = octaves;
		return TerrainValue(x, y);
	}
}


using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public abstract class ATerrainGenerator
{
	// Terrain generation parameters
	protected double frequency;
	protected double lacunarity;
	protected int octaves;
	protected QualityMode quality = QualityMode.High;
	protected double scale;
	protected RidgedMultifractal ridgedMultiFractal;

	protected int seed;

	public ATerrainGenerator(int seed) {
		this.seed = seed;
		ridgedMultiFractal = new RidgedMultifractal(
			this.frequency, 
			this.lacunarity, 
			this.octaves, 
			this.seed, 
			this.quality
		);
	}


	protected float TerrainValue(int x, int y) {
		return (float) this.ridgedMultiFractal.GetValue(x * this.scale, y * this.scale, 0);
	}

	public float GetHeight(int x, int y) {
		this.ridgedMultiFractal.OctaveCount = this.octaves;
		return TerrainValue(x, y);
	}

	public float GetRoughHeight(int x, int y, int octaves) {
		this.ridgedMultiFractal.OctaveCount = octaves;
		return TerrainValue(x, y);
	}
}


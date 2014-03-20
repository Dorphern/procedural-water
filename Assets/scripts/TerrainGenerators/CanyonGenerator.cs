using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class CanyonGenerator : ATerrainGenerator
{
	// Terrain generation parameters
	protected double frequency;
	protected double lacunarity;
	protected int octaves;
	protected QualityMode quality;
	protected double scale;
	protected float heightScale;

	public CanyonGenerator(int seed) : base(seed) {
		this.frequency = 0.04d;
		this.lacunarity = 1.5d;
		this.octaves = 10;
		this.scale = 1.5d;
		this.heightScale = 0.25f;
	}
}


using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class MountainGenerator : ATerrainGenerator
{
	// Terrain generation parameters
	protected double frequency;
	protected double lacunarity;
	protected int octaves;
	protected QualityMode quality;
	protected double scale;
	protected float heightScale;

	public MountainGenerator(int seed) : base(seed) {
		this.frequency = 0.02d;
		this.lacunarity = 1.5d;
		this.octaves = 4;
		this.scale = 2d;
		this.heightScale = 0.25f;
	}
}


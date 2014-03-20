using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class PillarGenerator : ATerrainGenerator
{
	// Terrain generation parameters
	protected double frequency;
	protected double lacunarity;
	protected int octaves;
	protected QualityMode quality;
	protected double scale;
	protected float heightScale;

	public PillarGenerator(int seed) : base(seed) {
		this.frequency = 0.02d;
		this.lacunarity = 1.5d;
		this.octaves = 3;
		this.scale = 2d;
		this.heightScale = 0.5f;


	}
}


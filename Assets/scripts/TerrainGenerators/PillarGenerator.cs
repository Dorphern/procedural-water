using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class PillarGenerator : ATerrainGenerator
{
	public PillarGenerator(int seed) : base(seed) {
		this.frequency = 0.02d;
		this.lacunarity = 1.5d;
		this.octaves = 3;
		this.scale = 2d;
		this.heightScale = 0.5f;

		this.setupGenerator();
	}
}


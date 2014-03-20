using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class CanyonGenerator : ATerrainGenerator
{

	public CanyonGenerator(int seed) : base(seed) {
		this.frequency = 0.04d;
		this.lacunarity = 1.5d;
		this.octaves = 10;
		this.scale = 1.5d;
		this.heightScale = 0.25f;

		this.setupGenerator();
	}
}


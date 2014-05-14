using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class MountainGenerator : ATerrainGenerator
{

	public MountainGenerator(int seed) : base(seed) {
		this.frequency = 0.015d;
		this.persistence = 0.5d;
		this.lacunarity = 1.4d;
		this.scale = 1d;
		this.heightScale = 0.5f;

		this.setupGenerator();
	}

	protected override float TerrainValue (float x, float y)
	{
		this.octaves = 4;
		float tVal = - Mathf.Abs(0.5f - base.TerrainValue(x, y));
		this.octaves = 2;
		float vVal = 0.5f - Mathf.Abs(0.5f - base.TerrainValue(-x * .3f, -y * 0.3f)) * 2f;
		return (vVal - 0.3f) * tVal * 4f + vVal * 2f;
	}
}


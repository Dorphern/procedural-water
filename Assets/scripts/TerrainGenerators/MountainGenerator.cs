using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class MountainGenerator : ATerrainGenerator
{

	public MountainGenerator(int seed) : base(seed) {
		this.frequency = 0.02d;
		this.persistence = 0.8d;
		this.lacunarity = 1.5d;
		this.octaves = 4;
		this.scale = 2d;
		this.heightScale = 0.5f;

		this.setupGenerator();
	}

	protected override float TerrainValue (int x, int y)
	{
		float tVal = base.TerrainValue(x, y);
		tVal = Mathf.Abs(0.5f - tVal) * 2f + 0.1f;
		return tVal;
	}
}


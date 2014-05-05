using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class MountainGenerator : ATerrainGenerator
{

	public MountainGenerator(int seed) : base(seed) {
		this.frequency = 0.01d;
		this.persistence = 0.5d;
		this.lacunarity = 1.5d;
		this.octaves = 2;
		this.scale = 0.5d;
		this.heightScale = 0.5f;

		this.setupGenerator();
	}

	protected override float TerrainValue (int x, int y)
	{
		float tVal = base.TerrainValue(x, y);
		tVal = Mathf.Abs(0.5f - tVal) * 2f;
		return tVal;
	}
}


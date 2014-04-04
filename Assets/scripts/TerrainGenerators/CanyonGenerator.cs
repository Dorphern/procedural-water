using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class CanyonGenerator : ATerrainGenerator
{

	public CanyonGenerator(int seed) : base(seed) {
		this.frequency = 0.1d;
		this.persistence = 0.7d;
		this.octaves = 2;
		this.scale = 0.2d;

		this.setupGenerator();
	}

	protected override float TerrainValue (int x, int y)
	{
		float tVal = base.TerrainValue(x, y) * 2f - 1f;
		tVal = Mathf.Round(tVal / 0.5f) * 0.5f;
		return Mathf.Clamp(tVal, 0.1f, 1f);
	}
}


using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class CanyonGenerator : ATerrainGenerator
{

	public CanyonGenerator(int seed) : base(seed) {
		this.frequency = 0.02d;
		this.persistence = 0.7d;
		this.octaves = 3;
		this.scale = 1d;

		this.setupGenerator();
	}

	protected override float TerrainValue (float x, float y)
	{
		float tVal = base.TerrainValue(x, y);
		tVal = Mathf.Round(tVal / 0.25f) * 0.25f;
		return Mathf.Clamp(tVal, 0.0f, 0.75f);
	}
}


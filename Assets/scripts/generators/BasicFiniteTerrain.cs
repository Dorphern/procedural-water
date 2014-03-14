using UnityEngine;
using System.Collections;

public class BasicFiniteTerrain : FiniteTerrainGenerator {

	public BasicFiniteTerrain(int seed) : base(seed) { }

	public override Heightmap generateWater() {
		Heightmap heightmap = new Heightmap(this.width, this.height);
		
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				heightmap.setHeight(x, y, 0.5f);
			}
		}
		
		return heightmap;
	}

	public override Heightmap generateTerrain() {
		Heightmap heightmap = new Heightmap(this.width, this.height);

		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				heightmap.setHeight(x, y, Random.Range(0f, 1f));
			}
		}

		return heightmap;
	}
}

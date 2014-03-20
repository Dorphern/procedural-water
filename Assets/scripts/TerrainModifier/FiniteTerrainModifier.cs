using UnityEngine;
using System.Collections;

public class FiniteTerrainModifier : ATerrainModifier {

	public FiniteTerrainModifier(ATerrainGenerator tg) : base(tg) {

	}

	public override Heightmap modifiedWater() {
		Heightmap heightmap = new Heightmap(this.width, this.height);
		
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				heightmap.setHeight(x, y, 0.01f);
			}
		}
		
		return heightmap;
	}
	
	public override Heightmap modifiedTerrain() {
		Heightmap heightmap = new Heightmap(this.width, this.height);

		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				heightmap.setHeight(x, y, this.terrainGenerator.GetHeight(x, y));
			}
		}
		
		return heightmap;
	}

}

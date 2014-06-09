using UnityEngine;
using System.Collections;

public class InfiniteTerrainModifier : ATerrainModifier {

	FiniteTerrainModifier finiteTerrainModifier;

	public InfiniteTerrainModifier(ATerrainGenerator tg) : base(tg) {
		finiteTerrainModifier = new FiniteTerrainModifier(tg);
	}

	public override void generate (ErosionOptions? erosionOptions, int time, float waterAmount) {
		int padding = time;
		if (erosionOptions.HasValue && erosionOptions.Value.generations > padding) 
			padding = erosionOptions.Value.generations;

		finiteTerrainModifier.setSize(width + padding * 2, height + padding * 2);

		finiteTerrainModifier.setOffset(terrainGenerator.getOffsetX() - padding, 
		                                terrainGenerator.getOffsetY() - padding);
		finiteTerrainModifier.generate(erosionOptions, time, waterAmount);

		terrainHeightmap 	= finiteTerrainModifier.getTerrainHeightmap().crop(padding, padding, width, height);
		waterHeightmap   	= finiteTerrainModifier.getWaterHeightmap().crop(padding, padding, width, height);
		waterflowMap 		= finiteTerrainModifier.getWaterflowMap().crop(padding, padding, width, height);
		erosionMap 			= finiteTerrainModifier.getErosionMap().crop(padding, padding, width, height);
	}

}

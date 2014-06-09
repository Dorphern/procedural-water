using UnityEngine;
using System.Collections;

public class OptimizedInfiniteModifier : ATerrainModifier {

	private int mapPadding = 0;

	OptimizedFiniteModifier optimizedFiniteModifier;


	public OptimizedInfiniteModifier(ATerrainGenerator tg) : base(tg) { 
		optimizedFiniteModifier = new OptimizedFiniteModifier(tg);
	}


	public override void generate (ErosionOptions? erosionOptions, int time, float waterAmount) {
		mapPadding = (int)Mathf.Pow (2f, time);
		
		optimizedFiniteModifier.setSize(width + mapPadding * 2, height + mapPadding * 2);
		
		optimizedFiniteModifier.setOffset(terrainGenerator.getOffsetX() - mapPadding, 
		                                  terrainGenerator.getOffsetY() - mapPadding);
		optimizedFiniteModifier.generate(erosionOptions, time, waterAmount);
		
		terrainHeightmap 	= optimizedFiniteModifier.getTerrainHeightmap().crop(mapPadding, mapPadding, width, height);
		waterHeightmap   	= optimizedFiniteModifier.getWaterHeightmap().crop(mapPadding, mapPadding, width, height);
		waterflowMap 		= optimizedFiniteModifier.getWaterflowMap().crop(mapPadding, mapPadding, width, height);
		erosionMap 			= optimizedFiniteModifier.getErosionMap().crop(mapPadding, mapPadding, width, height);
	}

}

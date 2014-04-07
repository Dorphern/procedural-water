using UnityEngine;
using System.Collections;
using System.Text;

public class TestControls : MonoBehaviour {

	private Heightmap groundHeightmap;
	private Heightmap waterHeightmap;
	private Heightmap slopeMap;
	private Heightmap errosionMap;
	private Heightmap waterflowMap;

	private string generationSeed = "";
	private float simulationSize = 1f;
	private int terrainGenerator = 0;
	private bool useInfiniteModifier = false;
	private bool visualizeHeight = false;
	[SerializeField] private float simSizeMin = 5f;
	[SerializeField] private float simSizeMax = 10f;
	[SerializeField] private Terrain groundTerrain;
	[SerializeField] private Terrain waterTerrain;
	[SerializeField] private float rainAmount = 0.1f;
	[SerializeField] private float solubility = 0.01f;
	[SerializeField] private float evaporation = 0.1f;
	[SerializeField] private float sedimentCapacity = 0.03f;
	[SerializeField] private int erosionGenerations = 7;
	[SerializeField] private int erosionsPerGeneration = 7;


	// Use this for initialization
	void Start () {
		this.simulationSize = this.simSizeMin;
		generateTerrain();
	}

	void OnGUI () {
		GUILayout.BeginArea(new Rect(0, 0, 200, 500));
		GUILayout.BeginVertical("box");

		//GUILayout.Box("walla", GUILayout.Height(200));
		this.generationSeed = GUILayout.TextField(this.generationSeed);
		this.simulationSize = Mathf.Round(GUILayout.HorizontalSlider(this.simulationSize, this.simSizeMin, this.simSizeMax));

		this.useInfiniteModifier = GUILayout.Toggle(this.useInfiniteModifier, " Infinite terrain");

		bool oldVisHeight = this.visualizeHeight;
		this.visualizeHeight = GUILayout.Toggle(this.visualizeHeight, " Visualize height");

		if (oldVisHeight != this.visualizeHeight) { this.updateTerrainVisualization(); }

		this.terrainGenerator = (int)Mathf.Round(GUILayout.HorizontalSlider((float)this.terrainGenerator, 0, 2));

		if (GUILayout.Button("Generate!")) { this.generateTerrain(); }

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	int numberFromString(string str) {
		int d = 0, i = 0;
		foreach (char c in str)
		{
			d += ((int) Mathf.Pow(10, i)) * (int) c;
			i++;
		}
		return d;
	}

	void setTerrainObjects() {
		TerrainData groundTerrainData 	= this.groundTerrain.terrainData;
		TerrainData waterTerrainData 	= this.waterTerrain.terrainData;

		int res = (int)(Mathf.Pow(2, this.simulationSize) + 1f);
		int size = res * 2;
		
		int height = 30;
		
		groundTerrainData.alphamapResolution 	= waterTerrainData.alphamapResolution 	= res;
		groundTerrainData.heightmapResolution 	= waterTerrainData.heightmapResolution 	= res;
		groundTerrainData.SetDetailResolution(res, 8);
		waterTerrainData.SetDetailResolution(res, 8);
		
		groundTerrainData.size = waterTerrainData.size = new Vector3(size, height, size);
		
		this.groundTerrain.gameObject.transform.position = 
			this.waterTerrain.gameObject.transform.position =
				new Vector3(-groundTerrainData.size.x / 2f, 0f, -groundTerrainData.size.z / 2f);
	}

	ATerrainGenerator getTerrainGenerator(int num, int seed) {
		switch (num) {
			case 0: return new MountainGenerator(seed);
			case 1: return new CanyonGenerator(seed);
			case 2: return new PillarGenerator(seed);
		}
		return null;
	}
	
	void generateTerrain() {
		TerrainData groundTerrainData 	= this.groundTerrain.terrainData;
		TerrainData waterTerrainData 	= this.waterTerrain.terrainData;

		setTerrainObjects();

		int res = (int)(Mathf.Pow(2, this.simulationSize) + 1f);
		int size = res * 2;

		ATerrainGenerator generator = this.getTerrainGenerator(this.terrainGenerator, 
		                                                       numberFromString(this.generationSeed));
		ATerrainModifier modifier;

		if (this.useInfiniteModifier) {
			modifier = new InfiniteTerrainModifier(generator);
		} else {
			modifier = new FiniteTerrainModifier(generator);
		}

		modifier.setSize(res, res);
		this.groundHeightmap = modifier.modifiedTerrain();
		this.waterHeightmap = modifier.modifiedWater();

		Erosion erosionController = new Erosion(this.groundHeightmap, this.rainAmount, this.solubility, this.evaporation, this.sedimentCapacity);
		erosionController.ErodeTerrain(erosionGenerations, erosionsPerGeneration);

		this.slopeMap = modifier.slopeMap();
		this.errosionMap = modifier.errosionMap();
		this.waterflowMap = modifier.waterflowMap();

		this.updateTerrainVisualization();
	}

	void updateTerrainVisualization() {
		TerrainData groundTerrainData 	= this.groundTerrain.terrainData;
		TerrainData waterTerrainData 	= this.waterTerrain.terrainData;

		Heightmap groundHm, waterHm;

		if (this.visualizeHeight) {
			groundHm = this.groundHeightmap;
			waterHm = this.waterHeightmap;
		} else {
			int size = this.groundHeightmap.getSizeHeight();
			groundHm = new Heightmap(size, size, 0.2f);
			waterHm = new Heightmap(size, size, 0.1f);
		}

		groundTerrainData.SetHeights(0, 0, groundHm.getHeights());
		waterTerrainData.SetHeights(0, 0, waterHm.getHeights());
	}
}

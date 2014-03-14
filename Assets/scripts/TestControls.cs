using UnityEngine;
using System.Collections;
using System.Text;

public class TestControls : MonoBehaviour {
	
	[SerializeField] private float simSizeMin = 32f;
	[SerializeField] private float simSizeMax = 256f;

	private string generationSeed = "";
	private float simulationSize = 1f;

	[SerializeField] private Terrain groundTerrain;
	[SerializeField] private Terrain waterTerrain;

	// Use this for initialization
	void Start () {
		this.simulationSize = this.simSizeMin;
		generateTerrain();
	}

	void OnGUI () {
		GUILayout.BeginArea(new Rect(0, 0, 200, 500));
		GUILayout.BeginVertical("box");

		GUILayout.Box("walla", GUILayout.Height(200));
		this.generationSeed = GUILayout.TextField(this.generationSeed);
		this.simulationSize = GUILayout.HorizontalSlider(this.simulationSize, this.simSizeMin, this.simSizeMax);
		if (GUILayout.Button("Generate!")) {
			this.generateTerrain();
		}

		this.simulationSize = Mathf.Pow(2, Mathf.Round(Mathf.Log(this.simulationSize, 2)));

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

	void generateTerrain() {
		TerrainData groundTerrainData 	= this.groundTerrain.terrainData;
		TerrainData waterTerrainData 	= this.waterTerrain.terrainData;

		int res = (int)(this.simulationSize + 1f);
		int size = res * 2;

		int height = 15;

		groundTerrainData.alphamapResolution 	= waterTerrainData.alphamapResolution 	= res;
		groundTerrainData.heightmapResolution 	= waterTerrainData.heightmapResolution 	= res;
		groundTerrainData.SetDetailResolution(res, 8);
		waterTerrainData.SetDetailResolution(res, 8);

		groundTerrainData.size = waterTerrainData.size = new Vector3(size, height, size);


		this.groundTerrain.gameObject.transform.position = 
			this.waterTerrain.gameObject.transform.position =
				new Vector3(-groundTerrainData.size.x / 2f, 0f, -groundTerrainData.size.z / 2f);

		TerrainGenerator generator = new BasicFiniteTerrain(numberFromString(this.generationSeed));
		generator.setSize(res, res);
		Heightmap groundHeightmap = generator.generateTerrain();
		Heightmap waterHeightmap = generator.generateWater();

		groundTerrainData.SetHeights(0, 0, groundHeightmap.getHeights());
		waterTerrainData.SetHeights(0, 0, waterHeightmap.getHeights());
	}
}

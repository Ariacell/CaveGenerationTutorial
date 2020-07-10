using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {


    /* PUBLIC UNITY INTERFACE */
    public Mesh mesh;
    public Material material;
    public GameObject terrainUnit; //Prefab unit to use as the terrain tiling unit
    public int chunkWidth = 10; //Chunksize of terrain to load
    public int blockScale = 1; //Scale of the unit of terrain we are using as blocks
    public float maxMapHeight = 10;
    /* END PUBLIC UNITY INTERFACE */

    private float offset;

    private GameObject[,] chunkMap;
    private FastNoise noise;
    private float[,] heightMap;

    // Start is called before the first frame update
    void Start() {

        chunkMap = new GameObject[chunkWidth, chunkWidth];
        offset = terrainUnit.GetComponent<Renderer>().bounds.size.x;

        noise = new FastNoise(5983);
        noise.SetNoiseType(FastNoise.NoiseType.Perlin);

        heightMap = GetHeightMap(chunkMap, 10);


        for (int x = 0; x < chunkWidth; x++) {
            for (int y = 0; y < chunkWidth; y++) {
                Vector3 terrainUnitPosition = new Vector3(x * offset, heightMap[x, y], y * offset);
                chunkMap[x, y] = Instantiate(terrainUnit, terrainUnitPosition, Quaternion.identity);
                chunkMap[x, y].transform.SetParent(this.transform);
            }
        }
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
    }

    // Update is called once per frame
    void Update() {

    }

    private float[,] GetHeightMap(GameObject[,] targetMap, int octaves) {
        float[,] mapToPopulate = new float[chunkMap.GetLength(0), chunkMap.GetLength(1)];
        for (int oct = 1; oct < octaves+1; oct++) {
            for (int x = 0; x < mapToPopulate.GetLength(0); x++) {
                for (int y = 0; y < mapToPopulate.GetLength(1); y++) {
                    mapToPopulate[x, y] += (20/oct)*(noise.GetPerlin((oct)*x,(oct)*y)+1);
                }
            }
        }

        return mapToPopulate;
    }
}

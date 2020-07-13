using System;
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
    public float mapAmplitude = 20;
    public int octaves = 10;
    public int noiseSeed = 5983;
    /* END PUBLIC UNITY INTERFACE */

    private float offset;

    private GameObject[,] chunkMap;
    private FastNoise noise;
    private float[,] heightMap;

    private List<BouncingMapUnit> bouncingCubes;
    private List<BouncingMapUnit> settlingCubes;

    // Start is called before the first frame update
    void Start() {

        chunkMap = new GameObject[chunkWidth, chunkWidth];
        offset = terrainUnit.GetComponent<Renderer>().bounds.size.x;

        bouncingCubes = new List<BouncingMapUnit>();

        noise = new FastNoise(noiseSeed);
        noise.SetNoiseType(FastNoise.NoiseType.Perlin);

        heightMap = GetHeightMap(chunkMap, octaves);


        for (int x = 0; x < chunkWidth; x++) {
            for (int z = 0; z < chunkWidth; z++) {
                Vector3 terrainUnitPosition = new Vector3(
                    this.transform.position.x + x * offset,
                    (float) Math.Round(heightMap[x, z],1),
                    this.transform.position.z + z * offset);
                chunkMap[x, z] = Instantiate(terrainUnit, terrainUnitPosition, Quaternion.identity);
                chunkMap[x, z].transform.SetParent(this.transform);
            }
        }
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
    }

    // Update is called once per frame
    void Update() {

        List<BouncingMapUnit> nextBounceList = new List<BouncingMapUnit>();
        List<BouncingMapUnit> nextSettlingList = new List<BouncingMapUnit>();


        for (int i = 0; i < bouncingCubes.Count; i++) {
            if (bouncingCubes[i].lifetime < bouncingCubes[i].ttl) {
                bouncingCubes[i].lifetime += Time.deltaTime;
                bouncingCubes[i].bouncePos.y = bouncingCubes[i].initPos.y + (float)(bouncingCubes[i].magnitude * Math.Exp(-bouncingCubes[i].lifetime) * Math.Sin(Math.PI * bouncingCubes[i].lifetime));
                chunkMap[bouncingCubes[i].x, bouncingCubes[i].y].transform.position = bouncingCubes[i].bouncePos;
                nextBounceList.Add(bouncingCubes[i]);
            } else {
                chunkMap[bouncingCubes[i].x, bouncingCubes[i].y].transform.position = bouncingCubes[i].initPos;
            }
        }
        bouncingCubes = nextBounceList;
    }

    // private float DecaySine()

    private float[,] GetHeightMap(GameObject[,] targetMap, int octaves) {
        float[,] mapToPopulate = new float[chunkMap.GetLength(0), chunkMap.GetLength(1)];
        for (int oct = 1; oct < octaves+1; oct++) {
            for (int x = 0; x < mapToPopulate.GetLength(0); x++) {
                for (int y = 0; y < mapToPopulate.GetLength(1); y++) {
                    mapToPopulate[x, y] += (mapAmplitude/oct)*(noise.GetPerlin((oct)*x,(oct)*y));
                }
            }
        }

        return mapToPopulate;
    }

    public void BounceCube(int x, int y, float ttl, float magnitude) {
        Vector3 pos = chunkMap[x, y].transform.position;
        bouncingCubes.Add(new BouncingMapUnit(x, y, ttl, magnitude, pos));
        // chunkMap[x, y].transform.position += new Vector3(pos.x, pos.y + magnitude, pos.z);
    }

    public void HighlightZone(int sourcePositionX, int sourcePositionY, int radius, Material initialHighlightMat) {
        for (int x = sourcePositionX - radius; x <= sourcePositionX + radius; x++) {
            for (int y = sourcePositionY - radius; y <= sourcePositionY + radius; y++) {
                if (IsInMapBounds(x, y, chunkMap)) {
                    chunkMap[x, y].GetComponent<Renderer>().material = initialHighlightMat;
                }
            }
        }
    }
    private bool IsInMapBounds(int x, int y, GameObject[,] map) {
        return (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1));
    }

    class BouncingMapUnit {
        public int x;
        public int y;
        public float birthTime;
        public float ttl;
        public float lifetime;
        public float magnitude;
        public readonly Vector3 initPos;
        public Vector3 bouncePos;

        public BouncingMapUnit(int x, int y, float ttl, float magnitude, Vector3 pos) {
            this.x = x;
            this.y = y;
            this.birthTime = Time.time;
            this.lifetime = 0;
            this.ttl = ttl * (float)(2 * Math.PI);
            this.magnitude = magnitude;
            this.initPos = pos;
            this.bouncePos = pos;
        }
    }
}

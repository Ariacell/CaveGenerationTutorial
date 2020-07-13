using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPerlin : MonoBehaviour {

    public float animationSpeed = 1f;
    public int terrainLength = 50;
    public int terrainWidth = 50;
    public float mapAmplitude = 20;
    public int octaves = 10;
    public int noiseSeed = 23213;

    private float[,] heightMap;
    private Vector2 animationPosition;
    private FastNoise noise;

    private Vector3[,] newVertices;
    // private Vector2[] newUV;
    private int[] newTris;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Start() {
        noise = new FastNoise(noiseSeed);
        noise.SetNoiseType(FastNoise.NoiseType.Perlin);
        animationPosition = new Vector2(0, 0);

        heightMap = new float[terrainWidth, terrainLength];


        meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        newVertices = new Vector3[terrainWidth, terrainLength];
        //Fix the line below so we don't need a factor to be divisble by 2...
        newTris = new int[terrainLength * terrainWidth / 2];
    }

    void Update() {
        animationPosition += new Vector2(animationSpeed * Time.deltaTime, animationSpeed * Time.deltaTime);
        heightMap = GetAnimatedHeightMap(heightMap, noiseSeed, octaves, mapAmplitude, animationPosition);
        for (int x = 0; x < newVertices.GetLength(0); x++) {
            for (int y = 0; y < newVertices.GetLength(1); y++) {
                newVertices[x, y] = new Vector3(x, heightMap[x, y], y);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        if (newVertices != null) {
            for (int x = 0; x < newVertices.GetLength(0); x++) {
                for (int y = 0; y < newVertices.GetLength(1); y++) {
                    Gizmos.DrawSphere(newVertices[x, y], 0.2f);
                }
            }
        }
    }

    private float[,] GetAnimatedHeightMap(float[,] map, int seed, int octaves, float amp, Vector2 animationPosition) {
        float[,] mapToPopulate = new float[map.GetLength(0), map.GetLength(1)];
        for (int oct = 1; oct < octaves + 1; oct++) {
            for (int x = 0; x < mapToPopulate.GetLength(0); x++) {
                for (int y = 0; y < mapToPopulate.GetLength(1); y++) {
                    mapToPopulate[x, y] += (mapAmplitude / oct) * (noise.GetPerlin((oct) * (x + animationPosition.x), (oct) * y + animationPosition.y));
                }
            }
        }

        return mapToPopulate;
    }



}

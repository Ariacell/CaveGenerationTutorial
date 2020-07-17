using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPerlin : MonoBehaviour {

    public float animationSpeed = 1f;
    public int terrainDepth = 50;
    public int terrainWidth = 50;
    public float mapAmplitude = 20;
    public int octaves = 10;
    public int noiseSeed = 23213;

    private float[,] heightMap;
    private Vector2 animationPosition;
    private FastNoise noise;

    private Vector3[] newVertices;
    private Vector2[] newUV;
    private int[] newTris;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void Start() {
        noise = new FastNoise(noiseSeed);
        noise.SetNoiseType(FastNoise.NoiseType.Perlin);
        animationPosition = new Vector2(0, 0);

        heightMap = new float[terrainWidth, terrainDepth];

        meshFilter = this.gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = (Material)Resources.Load("Material");

        meshCollider = this.gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        newVertices = new Vector3[(terrainWidth) * (terrainDepth)];
        newUV = new Vector2[newVertices.Length];
        newTris = new int[(terrainDepth) * (terrainWidth) * 6];

        for (int i = 0, x = 0; x < terrainWidth; x++) {
            for (int y = 0; y < terrainDepth; i++, y++) {
                newVertices[i] = new Vector3(y, 0, x);
                newUV[i] = new Vector2(newVertices[i].x, newVertices[i].z);

            }
        }

        // for (int ti = 0, vi = 0, y = 0; y < terrainDepth; y++, vi++) {
        //     for (int x = 0; x < terrainWidth; x++, ti += 6, vi++) {
        //         newTris[ti] = vi;
        //         newTris[ti + 1] = newTris[ti + 4] = vi + 1;
        //         newTris[ti + 2] = newTris[ti + 3] = vi + terrainWidth;
        //         newTris[ti + 5] = vi + terrainWidth + 1;
        //     }
        // }

        for (int i = 0, ti = 0; i < terrainWidth - 1; i++) {
            for (int j = 0; j < terrainDepth - 1; j++, ti += 6) {
                newTris[ti] = j + terrainWidth * i;
                newTris[ti + 1] = j + 1 + terrainWidth * i;
                newTris[ti + 2] = j + (terrainWidth * (i + 1));

                newTris[ti + 3] = j + 1 + terrainWidth * i;
                newTris[ti + 4] = j + 1 + terrainWidth * (i + 1);
                newTris[ti + 5] = j + terrainWidth * (i + 1);
            }
        }

        mesh.vertices = newVertices;
        mesh.triangles = newTris;
        mesh.uv = newUV;
        mesh.RecalculateNormals();
    }

    void Update() {
        animationPosition += new Vector2(animationSpeed * Time.deltaTime, animationSpeed * Time.deltaTime);
        heightMap = GetAnimatedHeightMap(heightMap, noiseSeed, octaves, mapAmplitude, animationPosition);
        for (int i = 0, x = 0; x < terrainWidth; x++) {
            for (int y = 0; y < terrainDepth; i++, y++) {
                newVertices[i] = new Vector3(x, heightMap[x, y], y);
            }
        }
        mesh.vertices = newVertices;
        mesh.RecalculateNormals();


    }

    // private void OnDrawGizmos() {
    //     Gizmos.color = Color.green;
    //     if (newVertices != null) {
    //         for (int i = 0; i < newVertices.Length; i++) {
    //             Gizmos.color = new Color(i, 0, 0);
    //             Gizmos.DrawSphere(newVertices[i], 0.2f);
    //         }
    //     }
    // }

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

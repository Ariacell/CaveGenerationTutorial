using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CubeAnimatedPerlin : MonoBehaviour {

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
    private Vector3[] squareVertices;
    private Vector2[] newUV;
    private int[] newTris;
    private int[] squareTris;

    private int[] surfaceTris; //Tris with their faces parallel to the Up axis
    private int[] sideTris; //Tris with their faces parallel to the depth axis
    private int[] depthTris; //Tris with their faces parallel to the width axis

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

        newVertices = new Vector3[(terrainWidth) * (terrainDepth)];
        squareVertices = new Vector3[newVertices.Length * 4];
        squareTris = new int[squareVertices.Length * 6];

        surfaceTris = new int[newVertices.Length * 6];
        sideTris = new int[newVertices.Length * 6 - 3 * terrainDepth];
        depthTris = new int[newVertices.Length * 6 - 3 * terrainWidth];

        newUV = new Vector2[squareVertices.Length];
        newTris = new int[(terrainDepth) * (terrainWidth) * 6];

        for (int i = 0, x = 0; x < terrainWidth; x++) {
            for (int y = 0; y < terrainDepth; i++, y++) {
                newVertices[i] = new Vector3(x, 0, y);
            }
        }

        //TODO: Figure out how to tile the UVs correctly
        // for (int i = 0, x = 0; x < terrainWidth; x++) {
        //     for (int y = 0; y < terrainDepth; i+=4, y++) {
        //         newUV[i] = new Vector2(squareVertices[i].x, squareVertices[i].z);
        //         Debug.Log("WE MANAGED TO ASSIGN THE FIRST UV");
        //         newUV[i+1] = new Vector2(squareVertices[i+1].x, newVertices[i+1].z);
        //         newUV[i+2] = new Vector2(squareVertices[i+2].x, newVertices[i+2].z);
        //         newUV[i+3] = new Vector2(squareVertices[i+3].x, newVertices[i+3].z);
        //     }
        // }

        // for (int i = 0, ti = 0; i < (terrainWidth - 1)*2; i++) {
        //     for (int j = 0; j < (terrainDepth - 1)*2; j++, ti += 6) {
        //         squareTris[ti] = j + terrainWidth * i;
        //         squareTris[ti + 1] = j + 1 + terrainWidth * i;
        //         squareTris[ti + 2] = j + (terrainWidth * (i + 1));

        //         squareTris[ti + 3] = j + 1 + terrainWidth * i;
        //         squareTris[ti + 4] = j + 1 + terrainWidth * (i + 1);
        //         squareTris[ti + 5] = j + terrainWidth * (i + 1);
        //     }
        // }

        // squareTris[0] = 0;
        // squareTris[1] = 2;
        // squareTris[2] = 1;

        // squareTris[3] = 0;
        // squareTris[4] = 3;
        // squareTris[5] = 2;


        // squareTris[6] = 0 + 3;
        // squareTris[7] = 0 + 4;
        // squareTris[8] = 0 + 4 + 1;
        // squareTris[9] = 0 + 4 + 1;
        // squareTris[10] = 0 + 2;
        // squareTris[11] = 0 + 3;

        //Surface tris
        for (int x = 0, ti = 0; x < terrainWidth; x++) {
            for (int y = 0; y < terrainDepth; y++, ti +=6) {
                surfaceTris[ti] = y * 4 + x * terrainWidth * 4;
                surfaceTris[ti + 1] = y * 4 + 2 + x * terrainWidth * 4;
                surfaceTris[ti + 2] = y * 4 + 1 + x * terrainWidth * 4;

                surfaceTris[ti+3] = y * 4 + x * terrainWidth * 4;
                surfaceTris[ti + 4] = y * 4 + 3 + x * terrainWidth * 4;
                surfaceTris[ti + 5] = y * 4 + 2 + x * terrainWidth * 4;
            }
        }

        //Side tris
        for (int x = 0, ti = 0; x < terrainWidth-1; x++) {
            for (int y = 0; y < terrainDepth; y++, ti += 6) {
                sideTris[ti] = y * 4 + 1 + x * terrainWidth * 4;
                sideTris[ti+1] = y * 4 + 2 + x * terrainWidth * 4;
                sideTris[ti+2] = y * 4 + 3 + (x+1) * terrainWidth * 4;

                sideTris[ti+3] = y * 4 + 1 + x * terrainWidth * 4;
                sideTris[ti+4] = y * 4 + 3 + (x+1) * terrainWidth * 4;
                sideTris[ti+5] = y * 4 + (x+1) * terrainWidth * 4;
            }
        }

        //Depth tris
        for (int x = 0, ti = 0; x < terrainWidth; x++) {
            for (int y = 0; y < terrainDepth-1; y++, ti += 6) {
                depthTris[ti] = y * 4 + 2 + x * terrainWidth * 4;
                depthTris[ti+1] = y * 4 + 3 + x * terrainWidth * 4;
                depthTris[ti+2] = y * 4 + 3 + 1 + (x * terrainWidth * 4);

                depthTris[ti+3] = y * 4 + 2 + x * terrainWidth * 4;
                depthTris[ti+4] = y * 4 + 3 + 1 + (x * terrainWidth * 4);
                depthTris[ti+5] = y * 4 + 3 + 2 + (x * terrainWidth * 4);
            }
        }

        List<int> triList = new List<int>();
        triList.AddRange(surfaceTris);
        triList.AddRange(sideTris);
        triList.AddRange(depthTris);

        mesh.vertices = squareVertices;
        mesh.triangles = triList.ToArray();
        mesh.uv = newUV;
        mesh.RecalculateNormals();
        Debug.Log("Surface tris length: " + surfaceTris.Length);
        Debug.Log("Side tris length: " + sideTris.Length);
        Debug.Log("DepthTris length: " + depthTris.Length);
        Debug.Log("Triangles length: " + triList.Count);
        // for (int i = 0; i < sideTris.Length; i++) {
        //     Debug.Log("Side triangle index " + i + " : " + sideTris[i]);
        // };
    }

    void Update() {
        animationPosition += new Vector2(animationSpeed * Time.deltaTime, animationSpeed * Time.deltaTime);
        heightMap = GetAnimatedHeightMap(heightMap, noiseSeed, octaves, mapAmplitude, animationPosition);
        for (int i = 0, x = 0; x < terrainWidth; x++) {
            for (int y = 0; y < terrainDepth; i++, y++) {
                newVertices[i] = new Vector3(x, heightMap[x, y], y);
            }
        }
        squareVertices = ConvertVertsToSquaredUnitVerts(newVertices);
        mesh.vertices = squareVertices;
        mesh.RecalculateNormals();




    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        if (squareVertices != null) {
            for (int i = 0; i < squareVertices.Length; i++) {
                Gizmos.color = new Color(i, 0, 0);
                Gizmos.DrawSphere(squareVertices[i], 0.1f);
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

    private Vector3[] ConvertVertsToSquaredUnitVerts(Vector3[] vertices) {
        Vector3[] squaredVerts = new Vector3[vertices.Length * 4];
        for (int i = 0, ci = 0; i < vertices.Length; i++, ci += 4) {
            squaredVerts[ci] = vertices[i] + new Vector3(-0.5f, 0, -0.5f);
            squaredVerts[ci + 1] = vertices[i] + new Vector3(0.5f, 0, -0.5f);
            squaredVerts[ci + 2] = vertices[i] + new Vector3(0.5f, 0, 0.5f);
            squaredVerts[ci + 3] = vertices[i] + new Vector3(-0.5f, 0, 0.5f);
        }
        return squaredVerts;
    }



}

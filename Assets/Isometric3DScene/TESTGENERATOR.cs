using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTGENERATOR : MonoBehaviour {
    public Transform cubePrefab;
    public int rows, columns, depth;
    public float padding;
    // Use this for initialization
    void Start() {
        for (int i = 0; i < depth; i++) {
            for (int j = 0; j < columns; j++) {
                for (int k = 0; k < rows; k++) {
                    Vector3 pos = new Vector3(k * padding, j * padding, i * padding);
                    //print(pos);
                    Transform foo = (Transform)Instantiate(cubePrefab, pos, Quaternion.identity);
                }
            }
        }
    }
}

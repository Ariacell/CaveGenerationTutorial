using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {

    public Mesh mesh;
    public Material material;
    public float childScale;
    public float emergenceSpeed = 0.1f;
    public int maxDepth;
    private int depth;
    private bool hasSpawnedChildren = false;
    private Vector3 initialPosition = new Vector3(0, 0, 0);
    private Vector3 finalPosition = new Vector3(0,0,0);
    private float lerpFraction = 1;
    private float time = 0.0f;
    public float interpolationPeriod;

    private void Start() {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        // if (depth < maxDepth) {
        //     new GameObject("Fractal Child").
        //         AddComponent<Fractal>().Initialize(this);
        // }
    }

    private void Update() {
        time += Time.deltaTime;
        if (lerpFraction < 1) {
            lerpFraction += time * emergenceSpeed;
            transform.localPosition = Vector3.Lerp(Vector3.zero, finalPosition, lerpFraction);
        }

        if (
            time >= interpolationPeriod && 
                depth < maxDepth &&
                 hasSpawnedChildren == false &&
                 lerpFraction >= 1) {

            time = 0.0f;
            new GameObject("Fractal Child").
                AddComponent<Fractal>().Initialize(this, Vector3.forward);
            new GameObject("Fractal Child").
                AddComponent<Fractal>().Initialize(this, Vector3.down);
            new GameObject("Fractal Child").
                AddComponent<Fractal>().Initialize(this, Vector3.left);
            new GameObject("Fractal Child").
                AddComponent<Fractal>().Initialize(this, Vector3.right);
            new GameObject("Fractal Child").
                AddComponent<Fractal>().Initialize(this, Vector3.up);
            new GameObject("Fractal Child").
                AddComponent<Fractal>().Initialize(this, Vector3.back);
            hasSpawnedChildren = true;
        }

    }

    private void Initialize(Fractal parent, Vector3 direction) {
        mesh = parent.mesh;
        material = parent.material;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        emergenceSpeed = parent.emergenceSpeed;
        hasSpawnedChildren = false;
        childScale = parent.childScale;
        interpolationPeriod = parent.interpolationPeriod;
        // time = parent.time;
        lerpFraction = 0;
        finalPosition = direction * (0.5f + 0.5f * childScale);
        initialPosition = parent.transform.localPosition;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
    }
}
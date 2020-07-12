using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastColour : MonoBehaviour {

    public TerrainGenerator terrain;
    public float bounceCooldown = 2f;

    private Transform parent;
    private Camera playerCam;

    private float timeSinceBounce;

    [SerializeField] private Material raycastHitConfirmMaterial;

    // Start is called before the first frame update
    void Start() {
        parent = transform.parent;
        playerCam = gameObject.GetComponentInChildren<Camera>();

        timeSinceBounce = 0f;
    }

    // Update is called once per frame
    void Update() {
        timeSinceBounce += Time.deltaTime;
        RaycastHit hit;
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit)) {
            Renderer hitRenderer = hit.transform.GetComponent<Renderer>();
            if (hitRenderer) {
                hitRenderer.material = raycastHitConfirmMaterial;
                //Make this better by finding just passing the coordinates and figuring out the correct cube of origin in the Terrain class.
                // hit.transform.parent.SendMessage("HighlightZone", ((int)hit.transform.position.x, (int)hit.transform.position.y, 5,  raycastHitConfirmMaterial));
                terrain.HighlightZone((int)hit.transform.position.x, (int)hit.transform.position.z, 1, raycastHitConfirmMaterial);
                if (timeSinceBounce > bounceCooldown) {
                    terrain.BounceCube((int)hit.transform.position.x, (int)hit.transform.position.z, 1f, 1.0f);
                }
            }
        }
    }
}

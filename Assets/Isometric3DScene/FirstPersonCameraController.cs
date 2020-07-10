using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraController : MonoBehaviour {

    public float mouseSensitivity = 10;

    private Vector2 mouseDirection;
    private Transform parentBody;

    // Start is called before the first frame update
    void Start() {
        parentBody = this.transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        Vector2 deltaMouse = new Vector2(
            Input.GetAxisRaw("Mouse X") * mouseSensitivity,
            Input.GetAxisRaw("Mouse Y") * mouseSensitivity
            );
        mouseDirection += deltaMouse;
        mouseDirection.y = Mathf.Clamp(mouseDirection.y, -90f, 90f);

        transform.localRotation = Quaternion.AngleAxis(mouseDirection.y, Vector3.left);
        parentBody.localRotation = Quaternion.AngleAxis(mouseDirection.x, Vector3.up);
    }
}

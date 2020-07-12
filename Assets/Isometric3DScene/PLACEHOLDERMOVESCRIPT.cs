using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLACEHOLDERMOVESCRIPT : MonoBehaviour {

    public CharacterController controller;
    public float playerSpeed = 2.0f;

    public Transform groundCheck;
    public float groundDistance = 1.4f;
    public LayerMask groundMask;

    private Vector3 playerVelocity;
    private bool isGrounded;

    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    private float lowJumpMultiplierFloat = 2f;

    void Update() {

      //HORIZONTAL MOVEMENT
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = transform.right * input.x + transform.forward * input.z;

        controller.Move(move * Time.deltaTime * playerSpeed);

        // VERTICAL MOVEMENT
        playerVelocity.y += gravityValue * Time.deltaTime;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (Input.GetButtonDown("Jump") && isGrounded) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        controller.Move(playerVelocity * Time.deltaTime);
    }
}
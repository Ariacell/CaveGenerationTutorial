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
        //todo: figure out why CheckSphere isn't working as expected
        // isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // Debug.Log("IsGrounded is: " + isGrounded + " and ground check position is: " + groundCheck.position);
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }


        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = transform.right * input.x + transform.forward * input.z;

        // float x = Input.GetAxis("Horizontal");
        // float z = Input.GetAxis("Vertical");
        // Vector3 move = transform.right * x + transform.forward * z;


        controller.Move(move * Time.deltaTime * playerSpeed);

        // if (move != Vector3.zero) {
        //     gameObject.transform.forward = move;
        // }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && isGrounded) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        // if (playerVelocity.y > 0 && !Input.GetButton("Jump")) {
        //     playerVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplierFloat - 1) * Time.deltaTime;
        // }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
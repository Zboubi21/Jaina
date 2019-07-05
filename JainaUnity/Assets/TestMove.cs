using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour {

	public float speed = 10;
	public LayerMask floorMask;
	public Camera camRayLength;
	public Animator anim;

	Rigidbody playerRigidbody;
	Vector3 movement;
	Vector3 moveDirection = Vector3.zero;

	void Start(){
		playerRigidbody = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
    {
        // Store the input axes.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
 
        // Move the player around the scene.
        Move(h, v);
 
        // Turn the player to face the mouse cursor.
        Turning();
 
        // Animate the player.
        Animating(h, v);

    }
 
    void Move(float h, float v)
    {
        // Set the movement vector based on the axis input.
        movement.Set(h, 0f, v);
 
        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * speed * Time.deltaTime;
 
        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(transform.position + movement);
    }
 
    void Turning()
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
 
        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;
 
        // Perform the raycast and if it hits something on the floor layer...
        if (Physics.Raycast(camRay, out floorHit, floorMask))
        {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = floorHit.point - transform.position;
 
            // Ensure the vector is entirely along the floor plane.
            playerToMouse.y = 0f;
 
            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
 
            // Set the player's rotation to this new rotation.
            playerRigidbody.MoveRotation(newRotation);
        }
    }
 
    void Animating(float h, float v)
    {
        moveDirection = new Vector3(h, 0, v);
 
        if (moveDirection.magnitude > 1.0f)
        {
            moveDirection = moveDirection.normalized;
        }
 
        moveDirection = transform.InverseTransformDirection(moveDirection);

        // Debug.Log("moveDirection = " + moveDirection);
 
        anim.SetFloat("moveX", moveDirection.x, 0.05f, Time.deltaTime);
        anim.SetFloat("moveY", moveDirection.z, 0.05f, Time.deltaTime);
    }

}

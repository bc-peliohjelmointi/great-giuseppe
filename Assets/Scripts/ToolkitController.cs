using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolkitController : MonoBehaviour {

	Vector2 desiredVelocity;
	Vector2 velocity;
	float directionX;
	float acceleration;
	float deceleration;
	float maxSpeed;
	float turnspeed;
	bool onGround;

	private Rigidbody2D rb;

	float maxSpeedChange;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		directionX = Input.GetAxis("Horizontal");
		desiredVelocity = new Vector2(directionX, 0.0f) * maxSpeed;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		// Check if on ground or not
		// Use ground values or air values

		if (directionX != 0)
		{
			if (Mathf.Sign(directionX) != Mathf.Sign(velocity.x))
			{
				maxSpeedChange = turnspeed * Time.deltaTime;
			}
			else
			{
				maxSpeedChange = acceleration * Time.deltaTime;
			}
		}
		else
		{
			maxSpeedChange = deceleration * Time.deltaTime;
		}
		velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
		rb.velocity = velocity;
	}
}

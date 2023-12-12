using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rb;
	private Animator animator;
	private Vector2 inputDirection;
	private SpriteRenderer spriteRenderer;
	public float jumpForce = 2.0f;

	public int coinsCollected = 0;

	public float gravity = -9.0f;
	public float playerSpeed = 2.0f;
	public float acceleration = 2.0f;
	public float slowdown = 2.0f;
	public float maxSpeed = 10.0f;
	public float maxFallSpeed = 20.0f;
	public bool isGrounded = false;
	public float jumpPressedTime = 0.0f;
	public float jumpWindow = 0.1f;
	private bool jumpRequestPending = false;
	private bool jumpExecutePending = false;

	private int coinAmountInMap;


	// UI Connection
	Text coinText;
	

	/*  Controlling the character
	 *  Can try to add forces to rigidbody, but this probably works better for ships and other things.
	 *		Needs least amount of variables.
	 *  Can manually change the velocity of the rigidbody, this is most straightforward.
	 *		Needs many variables: acceleration etc.
	 *		Should do manual gravity or not?
	 *
	 *	Can manually move the rigidbody. This is the most manual approach.
	 *		Needs all the variables. Can break unity maybe?
	 * 
	 */

	// Use this for initialization

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		GameObject coinUI = null;
		coinUI = GameObject.FindGameObjectWithTag("UI");
		coinText = coinUI.GetComponent<Text>();

		// Find all coins
		GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
		coinAmountInMap = coins.Length;
	}

	// Update is called once per frame
	void Update()
	{
		if (coinText == null)
		{

		}
		if (Input.GetKey(KeyCode.Space))
		{
			jumpPressedTime = Time.time;
			jumpRequestPending = true;
		}

		inputDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		animator.SetBool("HorizontalInput", inputDirection != Vector2.zero);
		float dir = Mathf.Sign(inputDirection.x);
		spriteRenderer.flipX = (dir != 0 && dir > 0.0);
	}

	void FixedUpdate()
	{
		MoveCharacter(inputDirection.x);
	}

	void MoveCharacter(float xMovement)
	{
		// rb.AddForce(Vector2.right * xMovement * playerSpeed * Time.fixedDeltaTime);

		// Velocity on X axis
		Vector2 newVelocity = rb.velocity;
		if (xMovement != 0.0f)
		{
			Vector2 velocityChange = new Vector2(1.0f, 0.0f) * xMovement * acceleration * Time.fixedDeltaTime;
			newVelocity = rb.velocity + velocityChange;
			if (Mathf.Abs(newVelocity.x) > maxSpeed)
			{
				newVelocity.x = maxSpeed * Mathf.Sign(newVelocity.x);
			}
		}
		else
		{
			float signBefore = Mathf.Sign(rb.velocity.x);
			newVelocity.x = rb.velocity.x -  signBefore * slowdown * Time.fixedDeltaTime;
			if ((signBefore < 0.0f && newVelocity.x > 0.0f) ||
				(signBefore > 0 && newVelocity.x < 0.0f)) {
				newVelocity.x = 0.0f;
			}
		}


		//Gravity
		if (!isGrounded)
		{
			newVelocity.y += gravity * Time.fixedDeltaTime;
			if (newVelocity.y < -maxFallSpeed)
			{
				newVelocity.y = -maxFallSpeed;
			}
		}

		// Jumping
		if (isGrounded && jumpRequestPending)
		{
			TryDoJump();
		}
		if (jumpExecutePending)
		{
			newVelocity.y = jumpForce;
			jumpExecutePending = false;
		}

		rb.velocity = newVelocity;
	}

	private void TryDoJump()
	{
		if (Time.time - jumpPressedTime <= jumpWindow)
		{
			jumpExecutePending = true;
		}
		jumpRequestPending = false;
	}

	private void OnGroundContact()
	{
		isGrounded = true;
	}

	// Notice when feet touch ground
	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Ground")
		{
			OnGroundContact();
		}
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Ground")
		{
			OnGroundContact();
			if (jumpExecutePending)
			{

				TryDoJump();
			}
		}
		else if (other.tag == "Coin")
		{
			GameObject.Destroy(other.gameObject);
			coinsCollected += 1;
			coinText.text = "Coins: " + System.Convert.ToString(coinsCollected);
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Ground")
		{
			isGrounded = false;
		}
	}
}

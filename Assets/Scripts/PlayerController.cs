using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rb;
	private Animator animator;
	private Vector2 inputDirection;
	private float lastLookDir = 0.0f;
	private SpriteRenderer spriteRenderer;
	public float jumpForce = 2.0f;

	public int coinsCollected = 0;
	private bool coinCollectedThisFrame = false;

	public float gravity = -9.0f;
	public float playerSpeed = 2.0f;
	public float acceleration = 2.0f;
	public float slowdown = 2.0f;
	public float maxSpeed = 10.0f;
	public float maxFallSpeed = 20.0f;
	public float damagePushAmount = 2.0f;


	public bool isGrounded = false;
	public float jumpPressedTime = 0.0f;
	public float jumpWindow = 0.1f;
	private bool jumpRequestPending = false;
	private bool jumpExecutePending = false;

	private int coinAmountInMap;
	private float health = 3.0f;
	private float maxHealth = 3.0f;
	private float lastDamageTime = 0.0f;
	public float damageInterval = 1.0f;

	private float winTime = 0.0f;
	public float winTimer = 3.0f;

	private Vector3 lastCheckPointPos;

	enum GameState {
		StartMenu,
		PlayGame,
		WinMenu
	};

	GameState currentState = GameState.StartMenu;

	// UI Connection
	Text coinText;
	HeartUI[] hearts;
	GameObject winPanel;

	// Audio connection
	PlayerSounds soundPlayer;
	

	/*  Controlling the character
	 *  Can try to add forces to rigidbody, but this probably works better for ships and other things.
	 *		Needs least amount of variables.
	 *  Can manually change the velocity of the rigidbody, this is most straightforward.
	 *		Needs many variables: acceleration etc.
	 *		Should do manual gravity or not?
	 *
	 *	Can manually move the rigidbody. This is the most manual approach.
	 *		Needs all the variables. Can break unity maybe?
	 */

	// Use this for initialization


	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		soundPlayer = GetComponent<PlayerSounds> ();


		GameObject coinUI = null;
		coinUI = GameObject.FindGameObjectWithTag("UI");
		coinText = coinUI.GetComponent<Text>();
		hearts = new HeartUI[3];
		for (int i = 1; i <= 3; i++) {
			string uiName = "HeartUI" + i.ToString ();
			GameObject heart = GameObject.Find(uiName);
			Debug.Assert (heart, "No heart ui found: " + uiName);
			hearts [i - 1] = heart.GetComponent<HeartUI> ();

		}

		ToggleStartUI (true);
		ToggleWinUI (false);

		// Find all coins
		GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
		coinAmountInMap = coins.Length;

		lastCheckPointPos = transform.position;
	}

	void ToggleWinUI(bool visible) {
		GameObject winPanel = GameObject.Find ("WinPanel");
		GameObject winText = GameObject.Find ("WinText");
		CanvasRenderer image = winPanel.GetComponent<CanvasRenderer> ();
		CanvasRenderer text = winText.GetComponentInChildren<CanvasRenderer> ();
		image.SetAlpha (visible? 1.0f : 0.0f);
		text.SetAlpha (visible ? 1.0f : 0.0f);
	}

	void ToggleStartUI(bool visible) {
		GameObject panelObj = GameObject.Find ("StartPanel");
		GameObject textObj = GameObject.Find ("StartText");
		CanvasRenderer image = panelObj.GetComponent<CanvasRenderer> ();
		CanvasRenderer text = textObj.GetComponentInChildren<CanvasRenderer> ();
		image.SetAlpha (visible? 1.0f : 0.0f);
		text.SetAlpha (visible ? 1.0f : 0.0f);

	}

	// Update is called once per frame
	void Update()
	{
		switch (currentState) {
		case GameState.StartMenu:
			if (Input.anyKey) {
				ToggleStartUI (false);
				currentState = GameState.PlayGame;
			}
			break;
		case GameState.PlayGame:
			coinCollectedThisFrame = false;
			if (health <= 0) {
				float sinceDeath = Time.time - lastDamageTime;
				if (sinceDeath > damageInterval) {
					TeleportToCheckpoint ();
				} else {
					float progress = sinceDeath / damageInterval;
					transform.localScale = new Vector3 (1.0f - progress, 1.0f - progress, 1.0f);
					transform.RotateAround (transform.position, new Vector3 (0, 0, 1), progress * 20.0f);
				}
				return;
			}


			inputDirection = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
			if (Input.GetKey (KeyCode.Space) || inputDirection.y > 0.1f) {
				jumpPressedTime = Time.time;
				jumpRequestPending = true;
			}

			animator.SetBool ("HorizontalInput", inputDirection != Vector2.zero);
			if (inputDirection != Vector2.zero) {
				lastLookDir = Mathf.Sign (inputDirection.x);
			}
			spriteRenderer.flipX = (lastLookDir != 0 && lastLookDir > 0.0);

			Debug.DrawLine (transform.position, lastCheckPointPos, Color.magenta);
			break;
		case GameState.WinMenu:
			
			if (Input.anyKey && Time.time - winTime > winTimer) {
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name, LoadSceneMode.Single);
			}
			break;
		};
	}

	void FixedUpdate()
	{
		if (currentState == GameState.PlayGame) {
			MoveCharacter (inputDirection.x);
		}
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
			soundPlayer.PlayJump ();
		}

		rb.velocity = newVelocity;
	}

	private void WinGame() {
		ToggleWinUI (true);
		rb.velocity = Vector2.zero;
		currentState = GameState.WinMenu;
		winTime = Time.time;
	}


	private void TeleportToCheckpoint()
	{
		transform.position = lastCheckPointPos;
		health = maxHealth;
		UpdateHealth ();
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		transform.rotation = Quaternion.identity;
		soundPlayer.PlayTeleport ();
	}

	private void TryDoJump()
	{
		if (Time.time - jumpPressedTime <= jumpWindow)
		{
			jumpExecutePending = true;
		}
		jumpRequestPending = false;
	}

	private void CollectCoin(GameObject coin) {
		if (coinCollectedThisFrame == true) {
			return;
		}
		GameObject.Destroy (coin);
		coinsCollected += 1;
		coinText.text = "Coins: " + System.Convert.ToString (coinsCollected) + "/" + coinAmountInMap;
		soundPlayer.PlayCollect ();
		coinCollectedThisFrame = true;
		if (coinsCollected == coinAmountInMap) {
			WinGame ();
		}
	}

	private void UpdateHealth()
	{
		float healthLeft = health;
		for (int i = 0; i < hearts.Length; i++) {
			hearts [i].SetHealth (healthLeft);
			healthLeft -= 1.0f;
		}
	}
	private void TakeDamage(float amount) {
		float sinceLast = Time.time - lastDamageTime;
		if (sinceLast > damageInterval) {
			health -= amount;

			lastDamageTime = Time.time;

			// Push away
			rb.velocity = rb.velocity + (rb.velocity * -1.0f * damagePushAmount);

			UpdateHealth ();
			// Is dead?
			if (health > 0.0f) {
				soundPlayer.PlayDamage ();
			}
		}
	}

	private void OnGroundContact()
	{
		isGrounded = true;
	}

	// Notice when feet touch ground
	void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "Ground" || other.tag == "Enemy")
		{
			OnGroundContact();
		} else if (other.tag == "Coin") {
			CollectCoin (other.gameObject);
		}
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Ground") {
			OnGroundContact ();
			if (jumpExecutePending) {
				TryDoJump ();
			}
		} else if (other.tag == "Coin") {
			CollectCoin (other.gameObject);
		} else if (other.tag == "Enemy") {
			OnGroundContact ();
			TakeDamage (0.5f);
		} else if (other.tag == "Checkpoint") {
			lastCheckPointPos = transform.position;
			other.GetComponentInParent<Checkpoint> ().Activate ();
		} else if (other.tag == "MainCamera") {
			TakeDamage(health);
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Ground" || other.tag == "Enemy")
		{
			isGrounded = false;
		}
	}
}

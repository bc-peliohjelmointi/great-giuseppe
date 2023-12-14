using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class DrillerController : MonoBehaviour {

	Animator animator;
	SpriteRenderer spriteRenderer;
	Rigidbody2D rb;
	public float speed = 1.0f;

	private Vector2 direction;
	bool locked = false;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		direction = new Vector2(-1, 0.0f);
	}

	public bool IsLocked()
	{
		return locked;
	}

	
	// Update is called once per frame
	void Update () {
		if (!locked)
		{
			rb.MovePosition(rb.position + speed * direction * Time.deltaTime);
		}
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			
			animator.SetTrigger("Player Near");
			locked = true;
		}
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			
			animator.SetTrigger("Player Left");
			locked = false;
		}
	}
	public void OnGroundTriggerExit()
	{
		// Turn around
		direction = new Vector3(direction.x * -1.0f, 0.0f, 0.0f);
		spriteRenderer.flipX = Mathf.Sign(direction.x) > 0;
	}
}

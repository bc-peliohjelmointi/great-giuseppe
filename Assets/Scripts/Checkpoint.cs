using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		// inactive color
		spriteRenderer.flipX = true;
		spriteRenderer.color = new Color (0.2f, 0.2f, 0.2f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Activate() {
		// Change tint on renderer
		spriteRenderer.color = Color.white;
		spriteRenderer.flipX = false;
	}
}

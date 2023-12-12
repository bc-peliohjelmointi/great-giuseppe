using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	Transform playerTransform = null;
	public Vector3 cameraOffset;
	// Use this for initialization
	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(playerTransform.position.x + cameraOffset.x,
			playerTransform.position.y + cameraOffset.y, transform.position.z);
		
	}
}

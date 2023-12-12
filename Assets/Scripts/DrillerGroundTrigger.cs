using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillerGroundTrigger : MonoBehaviour {

	DrillerController parentController;
	// Use this for initialization
	void Start () {
		parentController = GetComponentInParent<DrillerController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Ground")
		{
			parentController.OnGroundTriggerExit();
		}
	}

}

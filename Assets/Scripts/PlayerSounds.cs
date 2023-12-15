using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {

	public AudioClip[] jumpSounds;
	public AudioClip coinCollectSound;
	public AudioClip takeDamageSound;
	public AudioClip teleportSound;

	private AudioSource output;

	// Use this for initialization
	void Start () {
		output = GetComponent<AudioSource> ();
		Debug.Assert (output != null, "Did not get player audio source");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayJump()
	{
		output.PlayOneShot(jumpSounds[Random.Range(0, jumpSounds.Length)]);
	}
	public void PlayCollect(){
		output.PlayOneShot (coinCollectSound);
	}
	public void PlayDamage(){
		output.PlayOneShot (takeDamageSound);
	}
	public void PlayTeleport(){
		output.PlayOneShot (teleportSound);
	}
}

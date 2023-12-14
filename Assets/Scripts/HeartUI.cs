using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour {

	public Sprite fullHeart;
	public Sprite halfHeart;
	public Sprite emptyHeart;
	private Image imageUI;
	// Use this for initialization
	void Start () {
		imageUI = GetComponent<Image> ();
		
	}

	/// <summary>
	/// Sets the health.
	/// </summary>
	/// <param name="amount">0-1.0</param>
	public void SetHealth(float amount)
	{
		if (amount == 0.0) {
			imageUI.sprite = emptyHeart;
		} else if (amount == 0.5f) {
			imageUI.sprite = halfHeart;
		} else if (amount >= 1.0f) {
			imageUI.sprite = fullHeart;
		}
	}
}

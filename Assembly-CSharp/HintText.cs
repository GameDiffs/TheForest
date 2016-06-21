using System;
using UnityEngine;

public class HintText : MonoBehaviour
{
	private string[] texts = new string[]
	{
		"Hint: Cover yourself in mud to hide from enemies",
		"Hint: Being cold will drain your energy ",
		"Hint: Keep your energy up by eating regularly",
		"Hint: You can regain energy by resting on a bench",
		"Hint: Explosives and molotovs are effective against all enemies",
		"Hint: Try upgrading your weapons!",
		"Hint: You can make armor out of lizard skin!",
		"Hint:Effigies will scare enemies away from your camp",
		"Hint: Hide in bushes to escape from enemies",
		"Hint: Build a shelter to save your game",
		"Hint: Find a pot to boil water!"
	};

	private new string name;

	private UILabel MyLabel;

	private void Awake()
	{
		this.name = this.texts[UnityEngine.Random.Range(0, this.texts.Length - 1)];
		this.MyLabel = base.gameObject.GetComponent<UILabel>();
		this.MyLabel.text = this.name;
	}
}

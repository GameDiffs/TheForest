using System;
using UnityEngine;

[Serializable]
public class BurnEffigy : MonoBehaviour
{
	public GameObject[] Fires;

	private int FireAmount;

	private bool Lit;

	private int Amount;

	public GameObject Enemies;

	public override void Update()
	{
		if (Input.GetButtonDown("leanForward") && !this.Lit)
		{
			this.Lit = true;
			this.InvokeRepeating("LightMe", (float)1, 0.2f);
		}
	}

	public override void LightMe()
	{
		if (this.Amount < 6)
		{
			this.Amount++;
			this.Fires[this.Amount].SetActive(true);
		}
		else
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}
	}

	public override void Main()
	{
	}
}

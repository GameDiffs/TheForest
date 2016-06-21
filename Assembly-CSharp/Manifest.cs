using System;
using UnityEngine;

public class Manifest : MonoBehaviour
{
	public GameObject[] Mark;

	private int PassengersFound;

	private void CrossOff()
	{
		this.PassengersFound++;
		this.Mark[this.PassengersFound - 1].SetActive(true);
	}
}

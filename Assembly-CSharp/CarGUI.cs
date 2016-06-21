using System;
using UnityEngine;

[RequireComponent(typeof(GUIText))]
public class CarGUI : MonoBehaviour
{
	private const float MphtoMps = 2.237f;

	public CarController car;

	private string display = "{0:0} mph \nGear: {1:0}/{2:0}\nRevs {3:0%}\nThrottle: {4:0%}\n";

	private void Update()
	{
		object[] args = new object[]
		{
			this.car.CurrentSpeed * 2.237f,
			this.car.GearNum + 1,
			this.car.NumGears,
			this.car.RevsFactor,
			this.car.AccelInput
		};
		base.GetComponent<GUIText>().text = string.Format(this.display, args);
	}
}

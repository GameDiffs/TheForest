using System;
using UnityEngine;

public class WarnNoEnemies : MonoBehaviour
{
	public GameObject VeganTitle;

	public GameObject VegetarianTitle;

	public GameObject ClassicTitle;

	public GameObject IronTitle;

	public GameObject PermaDeathTitle;

	private void Update()
	{
		if (Cheats.NoEnemies != this.VeganTitle.activeSelf)
		{
			this.VeganTitle.SetActive(Cheats.NoEnemies);
		}
		if (Cheats.NoEnemiesDuringDay != this.VegetarianTitle.activeSelf)
		{
			this.VegetarianTitle.SetActive(Cheats.NoEnemiesDuringDay);
		}
		if ((!Cheats.NoEnemiesDuringDay && !Cheats.NoEnemies) != this.ClassicTitle.activeSelf)
		{
			this.ClassicTitle.SetActive(!Cheats.NoEnemiesDuringDay && !Cheats.NoEnemies);
		}
		if (this.PermaDeathTitle && Cheats.PermaDeath != this.PermaDeathTitle.activeSelf)
		{
			this.PermaDeathTitle.SetActive(Cheats.PermaDeath);
		}
		if (Cheats.NoDestruction != this.IronTitle.activeSelf)
		{
			this.IronTitle.SetActive(Cheats.NoDestruction);
		}
	}
}

using System;
using UnityEngine;

public class Cheats : MonoBehaviour
{
	private string[] VeganModeCode;

	private string[] VegetarianModeCode;

	private string[] IronForestCode;

	private string[] MeatModeCode;

	private string[] RawMeatModeCode;

	private string[] creativeCode;

	private int VeganModeIndex;

	private int VegetarianModeIndex;

	private int IronForestModeIndex;

	private int MeatModeIndex;

	private int RawMeatModeIndex;

	public static bool NoEnemies;

	public static bool NoEnemiesDuringDay;

	public static bool Creative;

	public static bool NoDestruction;

	public static bool GodMode;

	public static bool InfiniteEnergy;

	public static bool PermaDeath;

	private void Start()
	{
		this.MeatModeCode = new string[]
		{
			"m",
			"e",
			"a",
			"t",
			"m",
			"o",
			"d",
			"e"
		};
		this.VeganModeCode = new string[]
		{
			"v",
			"e",
			"g",
			"a",
			"n",
			"m",
			"o",
			"d",
			"e"
		};
		this.VegetarianModeCode = new string[]
		{
			"v",
			"e",
			"g",
			"e",
			"t",
			"a",
			"r",
			"i",
			"a",
			"n",
			"m",
			"o",
			"d",
			"e"
		};
		this.IronForestCode = new string[]
		{
			"i",
			"r",
			"o",
			"n",
			"f",
			"o",
			"r",
			"e",
			"s",
			"t"
		};
		this.RawMeatModeCode = new string[]
		{
			"r",
			"a",
			"w",
			"m",
			"e",
			"a",
			"t",
			"m",
			"o",
			"d",
			"e"
		};
		switch (PlayerPrefs.GetInt("Mode", 0))
		{
		case 0:
			Cheats.NoEnemiesDuringDay = false;
			Cheats.NoEnemies = false;
			Cheats.PermaDeath = false;
			break;
		case 1:
			Cheats.NoEnemiesDuringDay = false;
			Cheats.NoEnemies = true;
			Cheats.PermaDeath = false;
			break;
		case 2:
			Cheats.NoEnemiesDuringDay = true;
			Cheats.NoEnemies = false;
			Cheats.PermaDeath = false;
			break;
		case 3:
			Cheats.NoEnemiesDuringDay = false;
			Cheats.NoEnemies = false;
			Cheats.PermaDeath = true;
			break;
		}
		Cheats.NoDestruction = (PlayerPrefs.GetInt("Mode2", 0) == 1);
	}

	private void Update()
	{
		if (Input.anyKeyDown)
		{
			if (Input.GetKeyDown(this.MeatModeCode[this.MeatModeIndex]))
			{
				this.MeatModeIndex++;
			}
			else
			{
				this.MeatModeIndex = 0;
			}
			if (Input.GetKeyDown(this.RawMeatModeCode[this.RawMeatModeIndex]))
			{
				this.RawMeatModeIndex++;
				this.MeatModeIndex = 0;
			}
			else
			{
				this.RawMeatModeIndex = 0;
			}
			if (Input.GetKeyDown(this.VeganModeCode[this.VeganModeIndex]))
			{
				this.VeganModeIndex++;
			}
			else
			{
				this.VeganModeIndex = 0;
			}
			if (Input.GetKeyDown(this.VegetarianModeCode[this.VegetarianModeIndex]))
			{
				this.VegetarianModeIndex++;
			}
			else
			{
				this.VegetarianModeIndex = 0;
			}
			if (Input.GetKeyDown(this.IronForestCode[this.IronForestModeIndex]))
			{
				this.IronForestModeIndex++;
			}
			else
			{
				this.IronForestModeIndex = 0;
			}
		}
		if (this.MeatModeIndex == this.MeatModeCode.Length)
		{
			Debug.Log("Normal mode set");
			Cheats.NoEnemiesDuringDay = false;
			Cheats.NoEnemies = false;
			Cheats.PermaDeath = false;
			this.MeatModeIndex = 0;
			PlayerPrefs.SetInt("Mode", 0);
			PlayerPrefs.Save();
		}
		if (this.VeganModeIndex == this.VeganModeCode.Length)
		{
			Debug.Log("Vegan mode set");
			Cheats.NoEnemiesDuringDay = false;
			Cheats.NoEnemies = true;
			Cheats.PermaDeath = false;
			this.VeganModeIndex = 0;
			PlayerPrefs.SetInt("Mode", 1);
			PlayerPrefs.Save();
		}
		if (this.VegetarianModeIndex == this.VegetarianModeCode.Length)
		{
			Debug.Log("Vegetarian mode set");
			Cheats.NoEnemiesDuringDay = true;
			Cheats.NoEnemies = false;
			Cheats.PermaDeath = false;
			this.VegetarianModeIndex = 0;
			PlayerPrefs.SetInt("Mode", 2);
			PlayerPrefs.Save();
		}
		if (this.IronForestModeIndex == this.IronForestCode.Length)
		{
			Cheats.NoDestruction = !Cheats.NoDestruction;
			Debug.Log("IronForest " + ((!Cheats.NoDestruction) ? "unset" : "set"));
			this.IronForestModeIndex = 0;
			PlayerPrefs.SetInt("Mode2", (!Cheats.NoDestruction) ? 0 : 1);
			PlayerPrefs.Save();
		}
		if (this.RawMeatModeIndex == this.RawMeatModeCode.Length)
		{
			Debug.Log("PermaDeath mode set");
			Cheats.NoEnemiesDuringDay = false;
			Cheats.NoEnemies = false;
			Cheats.PermaDeath = true;
			this.RawMeatModeIndex = 0;
			PlayerPrefs.SetInt("Mode", 3);
			PlayerPrefs.Save();
		}
	}
}

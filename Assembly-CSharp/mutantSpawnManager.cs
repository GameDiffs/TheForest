using System;
using TheForest.Utils;
using UnityEngine;

public class mutantSpawnManager : MonoBehaviour
{
	public int desiredSkinny;

	public int desiredSkinnyPale;

	public int desiredRegular;

	public int desiredPale;

	public int desiredCreepy;

	[SerializeThis]
	public int offsetSkinny;

	[SerializeThis]
	public int offsetSkinnyPale;

	[SerializeThis]
	public int offsetRegular;

	[SerializeThis]
	public int offsetPale;

	[SerializeThis]
	public int offsetCreepy;

	public int maxSkinny;

	public int maxSkinnyPale;

	public int maxRegular;

	public int maxPale;

	public int maxCreepy;

	public int maxSleepingSpawns;

	public int numDesiredSpawns;

	private void Start()
	{
		float repeatRate = 220f;
		this.setMaxAmounts();
		if (!base.IsInvoking("addToMutantAmounts"))
		{
			base.InvokeRepeating("addToMutantAmounts", 1f, repeatRate);
		}
	}

	private void OnDeserialized()
	{
		float repeatRate = 220f;
		if (!base.IsInvoking("addToMutantAmounts"))
		{
			base.InvokeRepeating("addToMutantAmounts", 1f, repeatRate);
		}
	}

	public void offsetSleepAmounts()
	{
		if (Scene.MutantControler.hordeModeActive)
		{
			return;
		}
		if (this.offsetSkinny < 0)
		{
			this.offsetSkinny += 2;
		}
		if (this.offsetSkinnyPale < 0)
		{
			this.offsetSkinnyPale += 2;
		}
		if (this.offsetRegular < 0)
		{
			this.offsetRegular += 2;
		}
		if (this.offsetPale < 0)
		{
			this.offsetPale += 2;
		}
		if (this.offsetCreepy < 0)
		{
			this.offsetCreepy += 2;
		}
	}

	private void setMaxAmounts()
	{
		if (Scene.MutantControler.hordeModeActive)
		{
			return;
		}
		this.maxSkinny = this.desiredSkinny;
		this.maxSkinnyPale = this.desiredSkinnyPale;
		this.maxRegular = this.desiredRegular;
		this.maxPale = this.desiredPale;
		this.maxCreepy = this.desiredCreepy;
		this.desiredSkinny += this.offsetSkinny;
		this.desiredSkinnyPale += this.offsetSkinnyPale;
		this.desiredRegular += this.offsetRegular;
		this.desiredPale += this.offsetPale;
		this.desiredCreepy += this.offsetCreepy;
		if (this.desiredSkinny < 0)
		{
			this.maxSkinny = 0;
		}
		if (this.desiredSkinnyPale < 0)
		{
			this.maxSkinnyPale = 0;
		}
		if (this.desiredRegular < 0)
		{
			this.maxRegular = 0;
		}
		if (this.desiredPale < 0)
		{
			this.maxPale = 0;
		}
		if (this.desiredCreepy < 0)
		{
			this.maxCreepy = 0;
		}
	}

	private void addToMutantAmounts()
	{
		if (Scene.MutantControler.hordeModeActive)
		{
			return;
		}
		if (this.desiredSkinny < this.maxSkinny)
		{
			this.desiredSkinny++;
		}
		if (this.desiredSkinnyPale < this.maxSkinnyPale)
		{
			this.desiredSkinnyPale++;
		}
		if (this.desiredRegular < this.maxRegular)
		{
			this.desiredRegular++;
		}
		if (this.desiredPale < this.maxPale)
		{
			this.desiredPale++;
		}
		if (this.desiredCreepy < this.maxCreepy)
		{
			this.desiredCreepy++;
		}
		if (this.offsetSkinny < 0)
		{
			this.offsetSkinny++;
		}
		if (this.offsetSkinnyPale < 0)
		{
			this.offsetSkinnyPale++;
		}
		if (this.offsetRegular < 0)
		{
			this.offsetRegular++;
		}
		if (this.offsetPale < 0)
		{
			this.offsetPale++;
		}
		if (this.offsetCreepy < 0)
		{
			this.offsetCreepy++;
		}
	}

	public int countAllSpawns()
	{
		this.numDesiredSpawns = this.desiredCreepy + this.desiredPale + this.desiredRegular + this.desiredSkinny + this.desiredSkinnyPale;
		return this.numDesiredSpawns;
	}

	public void setMutantSpawnAmounts()
	{
		if (Scene.MutantControler.hordeModeActive)
		{
			return;
		}
		if (Clock.Day == 0)
		{
			this.setAmountDay0();
		}
		else if (Clock.Day == 1)
		{
			this.setAmountDay1();
		}
		else if (Clock.Day == 2)
		{
			this.setAmountDay2();
		}
		else if (Clock.Day == 3)
		{
			this.setAmountDay3();
		}
		else if (Clock.Day == 4)
		{
			this.setAmountDay4();
		}
		else if (Clock.Day == 5)
		{
			this.setAmountDay5();
		}
		else if (Clock.Day == 6)
		{
			this.setAmountDay6();
		}
		else if (Clock.Day == 7)
		{
			this.setAmountDay7();
		}
		else if (Clock.Day == 8)
		{
			this.setAmountDay8();
		}
		else if (Clock.Day == 9)
		{
			this.setAmountDay9();
		}
		else if (Clock.Day >= 10)
		{
			this.setAmountDay10();
		}
	}

	private void setAmountMP()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 4;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 4;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		else
		{
			this.desiredSkinny = 3;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 5;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		this.maxSleepingSpawns = 4;
	}

	private void setAmountDay0()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 6;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 0;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		else
		{
			this.desiredSkinny = 6;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 0;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		this.maxSleepingSpawns = 3;
		this.setMaxAmounts();
	}

	private void setAmountDay1()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 6;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 1;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		else
		{
			this.desiredSkinny = 6;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 2;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		this.maxSleepingSpawns = 3;
		this.setMaxAmounts();
	}

	private void setAmountDay2()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 5;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 2;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		else
		{
			this.desiredSkinny = 5;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 2;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		this.maxSleepingSpawns = 3;
		this.setMaxAmounts();
	}

	private void setAmountDay3()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 3;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 3;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		else
		{
			this.desiredSkinny = 3;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 3;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		this.maxSleepingSpawns = 3;
		this.setMaxAmounts();
	}

	private void setAmountDay4()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 2;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 6;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		else
		{
			this.desiredSkinny = 1;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 4;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		this.maxSleepingSpawns = 3;
		this.setMaxAmounts();
	}

	private void setAmountDay5()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 1;
			this.desiredSkinnyPale = 0;
			this.desiredRegular = 6;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		else
		{
			this.desiredSkinny = 1;
			this.desiredSkinnyPale = 3;
			this.desiredRegular = 4;
			this.desiredPale = 0;
			this.desiredCreepy = 0;
		}
		this.maxSleepingSpawns = 3;
		this.setMaxAmounts();
	}

	private void setAmountDay6()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 1;
			this.desiredSkinnyPale = 3;
			this.desiredRegular = 7;
			this.desiredPale = 1;
			this.desiredCreepy = 0;
		}
		else
		{
			this.desiredSkinny = 1;
			this.desiredSkinnyPale = 2;
			this.desiredRegular = 4;
			this.desiredPale = 2;
			this.desiredCreepy = 0;
		}
		this.maxSleepingSpawns = 3;
		this.setMaxAmounts();
	}

	private void setAmountDay7()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 1;
			this.desiredSkinnyPale = 1;
			this.desiredRegular = 7;
			this.desiredPale = 2;
			this.desiredCreepy = 0;
		}
		else
		{
			this.desiredSkinny = 1;
			this.desiredSkinnyPale = 1;
			this.desiredRegular = 4;
			this.desiredPale = 3;
			this.desiredCreepy = 1;
		}
		this.maxSleepingSpawns = 2;
		this.setMaxAmounts();
	}

	private void setAmountDay8()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 0;
			this.desiredSkinnyPale = 3;
			this.desiredRegular = 4;
			this.desiredPale = 2;
			this.desiredCreepy = 0;
		}
		else
		{
			this.desiredSkinny = 0;
			this.desiredSkinnyPale = 1;
			this.desiredRegular = 4;
			this.desiredPale = 3;
			this.desiredCreepy = 1;
		}
		this.maxSleepingSpawns = 2;
		this.setMaxAmounts();
	}

	private void setAmountDay9()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = 1;
			this.desiredSkinnyPale = 1;
			this.desiredRegular = 6;
			this.desiredPale = 3;
			this.desiredCreepy = 2;
		}
		else
		{
			this.desiredSkinny = 1;
			this.desiredSkinnyPale = 1;
			this.desiredRegular = 5;
			this.desiredPale = 3;
			this.desiredCreepy = 2;
		}
		this.maxSleepingSpawns = 2;
		this.setMaxAmounts();
	}

	private void setAmountDay10()
	{
		if (!Clock.Dark)
		{
			this.desiredSkinny = UnityEngine.Random.Range(0, 2);
			this.desiredRegular = UnityEngine.Random.Range(1, 7);
			this.desiredSkinnyPale = UnityEngine.Random.Range(0, 4);
			this.desiredPale = UnityEngine.Random.Range(1, 4);
			this.desiredCreepy = UnityEngine.Random.Range(1, 5);
		}
		else
		{
			this.desiredSkinny = UnityEngine.Random.Range(0, 3);
			this.desiredRegular = UnityEngine.Random.Range(1, 7);
			this.desiredSkinnyPale = UnityEngine.Random.Range(0, 2);
			this.desiredPale = UnityEngine.Random.Range(1, 4);
			this.desiredCreepy = UnityEngine.Random.Range(1, 3);
		}
		this.maxSleepingSpawns = 1;
		this.setMaxAmounts();
	}

	private void setLevel1()
	{
		this.desiredSkinny = 2;
		this.desiredSkinnyPale = 0;
		this.desiredRegular = 1;
		this.desiredPale = 0;
		this.desiredCreepy = 0;
		this.maxSleepingSpawns = 0;
		this.setMaxAmounts();
	}

	private void setLevel2()
	{
		this.desiredSkinny = 2;
		this.desiredSkinnyPale = 0;
		this.desiredRegular = 2;
		this.desiredPale = 0;
		this.desiredCreepy = 0;
		this.maxSleepingSpawns = 0;
		this.setMaxAmounts();
	}

	private void setLevel3()
	{
		this.desiredSkinny = 1;
		this.desiredSkinnyPale = 0;
		this.desiredRegular = 3;
		this.desiredPale = 0;
		this.desiredCreepy = 0;
		this.maxSleepingSpawns = 0;
		this.setMaxAmounts();
	}

	private void setLevel4()
	{
		this.desiredSkinny = 0;
		this.desiredSkinnyPale = 3;
		this.desiredRegular = 2;
		this.desiredPale = 0;
		this.desiredCreepy = 0;
		this.maxSleepingSpawns = 0;
		this.setMaxAmounts();
	}

	private void setLevel5()
	{
		this.desiredSkinny = 0;
		this.desiredSkinnyPale = 2;
		this.desiredRegular = 2;
		this.desiredPale = 1;
		this.desiredCreepy = 0;
		this.maxSleepingSpawns = 0;
		this.setMaxAmounts();
	}

	private void setLevel6()
	{
		this.desiredSkinny = 0;
		this.desiredSkinnyPale = 2;
		this.desiredRegular = 2;
		this.desiredPale = 2;
		this.desiredCreepy = 0;
		this.maxSleepingSpawns = 0;
		this.setMaxAmounts();
	}

	private void setLevel7()
	{
		this.desiredSkinny = 0;
		this.desiredSkinnyPale = 3;
		this.desiredRegular = 1;
		this.desiredPale = 3;
		this.desiredCreepy = 0;
		this.maxSleepingSpawns = 0;
		this.setMaxAmounts();
	}

	private void setLevel8()
	{
		this.desiredSkinny = 0;
		this.desiredSkinnyPale = 3;
		this.desiredRegular = 0;
		this.desiredPale = 3;
		this.desiredCreepy = 1;
		this.maxSleepingSpawns = 0;
		this.setMaxAmounts();
	}

	private void setLevel9()
	{
		this.desiredSkinny = 0;
		this.desiredSkinnyPale = 1;
		this.desiredRegular = 2;
		this.desiredPale = 3;
		this.desiredCreepy = 2;
		this.maxSleepingSpawns = 0;
		this.setMaxAmounts();
	}

	private void setLevel10()
	{
		this.desiredSkinny = 1;
		this.desiredSkinnyPale = 1;
		this.desiredRegular = 4;
		this.desiredPale = 2;
		this.desiredCreepy = 3;
		this.maxSleepingSpawns = 0;
		this.setMaxAmounts();
	}

	public void setHordeSpawnAmounts()
	{
		switch (Scene.MutantControler.hordeLevel)
		{
		case 1:
			this.setLevel1();
			break;
		case 2:
			this.setLevel2();
			break;
		case 3:
			this.setLevel3();
			break;
		case 4:
			this.setLevel4();
			break;
		case 5:
			this.setLevel5();
			break;
		case 6:
			this.setLevel6();
			break;
		case 7:
			this.setLevel7();
			break;
		case 8:
			this.setLevel8();
			break;
		case 9:
			this.setLevel9();
			break;
		case 10:
			this.setLevel10();
			break;
		}
		if (Scene.MutantControler.hordeLevel > 10)
		{
			this.setLevel10();
		}
	}
}

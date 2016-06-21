using System;
using UnityEngine;

public class dummyTypeSetup : MonoBehaviour
{
	public EnemyType _type;

	private setupBodyVariation setupBody;

	private void Awake()
	{
		this.setupBody = base.GetComponentInChildren<setupBodyVariation>();
		if (!this.setupBody)
		{
			this.setupBody = base.GetComponent<setupBodyVariation>();
		}
	}

	private void setFemaleSkinny(Transform getTr)
	{
		if (getTr != null)
		{
			base.transform.localScale = getTr.localScale;
		}
		this.setupBody.enableFemaleSkinny();
		this._type = EnemyType.skinnyFemale;
	}

	private void setMaleSkinny(Transform getTr)
	{
		if (getTr != null)
		{
			base.transform.localScale = getTr.localScale;
		}
		this._type = EnemyType.skinnyMale;
	}

	private void setRegularMale()
	{
		this._type = EnemyType.regularMale;
	}

	private void setRegularMaleFireman()
	{
		this._type = EnemyType.regularMaleFireman;
	}

	private void setRegularMaleLeader()
	{
		this._type = EnemyType.regularMaleLeader;
	}

	private void setPaleMale()
	{
		this._type = EnemyType.paleMale;
	}

	private void setRegularFemale()
	{
		this._type = EnemyType.regularFemale;
	}

	private void setCreepyArmsy()
	{
		this._type = EnemyType.creepyArmsy;
	}

	private void setCreepySpiderLady()
	{
		this._type = EnemyType.creepySpiderLady;
	}

	private void setCreepyBaby()
	{
		this._type = EnemyType.creepyBaby;
	}

	private void setMaleSkinnyPale()
	{
		this._type = EnemyType.skinnyMalePale;
	}
}

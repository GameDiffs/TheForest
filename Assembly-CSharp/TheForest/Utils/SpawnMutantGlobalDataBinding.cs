using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class SpawnMutantGlobalDataBinding : MonoBehaviour
	{
		private const int UnsetDataKey = -1337;

		public int _dataKey = -1337;

		private void OnDestroy()
		{
			if (Scene.ActiveMB)
			{
				this.OnSerializing();
			}
		}

		private void Start()
		{
			spawnMutants component = base.GetComponent<spawnMutants>();
			int @int = GlobalDataSaver.GetInt("sm_male_skinny_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_male_skinny = @int;
			}
			@int = GlobalDataSaver.GetInt("sm_female_skinny_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_female_skinny = @int;
			}
			@int = GlobalDataSaver.GetInt("sm_skinny_pale_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_skinny_pale = @int;
			}
			@int = GlobalDataSaver.GetInt("sm_male_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_male = @int;
			}
			@int = GlobalDataSaver.GetInt("sm_female_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_female = @int;
			}
			@int = GlobalDataSaver.GetInt("sm_fireman_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_fireman = @int;
			}
			@int = GlobalDataSaver.GetInt("sm_pale_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_pale = @int;
			}
			@int = GlobalDataSaver.GetInt("sm_armsy_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_armsy = @int;
			}
			@int = GlobalDataSaver.GetInt("sm_vags_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_vags = @int;
			}
			@int = GlobalDataSaver.GetInt("sm_baby_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_baby = @int;
			}
			@int = GlobalDataSaver.GetInt("sm_fat_" + this._dataKey, -1337);
			if (@int > -1337)
			{
				component.amount_fat = @int;
			}
		}

		private void OnSerializing()
		{
			if (!Cheats.NoEnemies && !Cheats.NoEnemiesDuringDay)
			{
				spawnMutants component = base.GetComponent<spawnMutants>();
				GlobalDataSaver.SetInt("sm_male_skinny_" + this._dataKey, component.amount_male_skinny);
				GlobalDataSaver.SetInt("sm_female_skinny_" + this._dataKey, component.amount_female_skinny);
				GlobalDataSaver.SetInt("sm_skinny_pale_" + this._dataKey, component.amount_skinny_pale);
				GlobalDataSaver.SetInt("sm_male_" + this._dataKey, component.amount_male);
				GlobalDataSaver.SetInt("sm_female_" + this._dataKey, component.amount_female);
				GlobalDataSaver.SetInt("sm_fireman_" + this._dataKey, component.amount_fireman);
				GlobalDataSaver.SetInt("sm_pale_" + this._dataKey, component.amount_pale);
				GlobalDataSaver.SetInt("sm_armsy_" + this._dataKey, component.amount_armsy);
				GlobalDataSaver.SetInt("sm_vags_" + this._dataKey, component.amount_vags);
				GlobalDataSaver.SetInt("sm_baby_" + this._dataKey, component.amount_baby);
				GlobalDataSaver.SetInt("sm_fat_" + this._dataKey, component.amount_fat);
			}
		}
	}
}

using Bolt;
using System;

public class mutantAI_net : EntityBehaviour<IMutantState>
{
	private const int leader_bit = 1;

	private const int maleSkinny_bit = 2;

	private const int femaleSkinny_bit = 4;

	private const int male_bit = 8;

	private const int female_bit = 16;

	private const int creepy_bit = 32;

	private const int creepy_male_bit = 64;

	private const int creepy_baby_bit = 128;

	private const int fireman_bit = 256;

	private const int pale_bit = 512;

	private const int creepy_fat_bit = 1024;

	public bool leader;

	public bool maleSkinny;

	public bool femaleSkinny;

	public bool male;

	public bool female;

	public bool creepy;

	public bool creepy_male;

	public bool creepy_baby;

	public bool fireman;

	public bool pale;

	public bool creepy_fat;

	private bool has_mask(int mask)
	{
		return (base.state.ai_mask & mask) == mask;
	}

	public override void Attached()
	{
		if (!this.entity.IsOwner())
		{
			base.state.AddCallback("ai_mask", delegate
			{
				this.creepy = this.has_mask(32);
				this.creepy_baby = this.has_mask(128);
				this.creepy_male = this.has_mask(64);
				this.creepy_fat = this.has_mask(1024);
				this.female = this.has_mask(16);
				this.femaleSkinny = this.has_mask(4);
				this.fireman = this.has_mask(256);
				this.leader = this.has_mask(1);
				this.male = this.has_mask(8);
				this.maleSkinny = this.has_mask(2);
				this.pale = this.has_mask(512);
			});
		}
	}

	public void TriggerSync(mutantAI ai)
	{
		if (BoltNetwork.isRunning && this.entity.IsOwner())
		{
			int num = 0;
			if (this.creepy = ai.creepy)
			{
				num |= 32;
			}
			if (this.creepy_baby = ai.creepy_baby)
			{
				num |= 128;
			}
			if (this.creepy_male = ai.creepy_male)
			{
				num |= 64;
			}
			if (this.creepy_fat = ai.creepy_fat)
			{
				num |= 1024;
			}
			if (this.female = ai.female)
			{
				num |= 16;
			}
			if (this.femaleSkinny = ai.femaleSkinny)
			{
				num |= 4;
			}
			if (this.fireman = ai.fireman)
			{
				num |= 256;
			}
			if (this.leader = ai.leader)
			{
				num |= 1;
			}
			if (this.male = ai.male)
			{
				num |= 8;
			}
			if (this.maleSkinny = ai.maleSkinny)
			{
				num |= 2;
			}
			if (this.pale = ai.pale)
			{
				num |= 512;
			}
			if (base.state.ai_mask != num)
			{
				base.state.ai_mask = num;
			}
		}
	}
}

using System;
using UnityEngine;

[Serializable]
public class PlaySoundRandomly : MonoBehaviour
{
	private bool Playing;

	private int WindDice;

	private int WindDiceStop;

	public override void Awake()
	{
		FMOD_StudioEventEmitter fMOD_StudioEventEmitter = (FMOD_StudioEventEmitter)this.GetComponent(typeof(FMOD_StudioEventEmitter));
		this.WindDice = UnityEngine.Random.Range(0, 4);
		this.WindDiceStop = UnityEngine.Random.Range(0, 3);
		if (this.WindDice == 2 && !this.Playing)
		{
			this.Playing = true;
			fMOD_StudioEventEmitter.Play();
		}
		else if (this.WindDiceStop == 2 && this.Playing)
		{
			this.Playing = false;
			fMOD_StudioEventEmitter.Stop();
		}
	}

	public override void Main()
	{
	}
}

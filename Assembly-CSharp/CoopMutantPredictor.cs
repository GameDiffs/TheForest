using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CoopMutantPredictor : MonoBehaviour
{
	public Animator anim;

	private float resetWeight;

	public float LerpSpeed = 2f;

	public int HitLayer = 1;

	public int NormalLayer;

	public float Weight = 1f;

	public float PredictionTime = 1f;

	private void getAttackDirection(int hitDir)
	{
	}

	private void HitHead()
	{
	}

	private void getCombo(int combo)
	{
	}

	private void StartPrediction()
	{
		base.StopCoroutine("PredictionRoutine");
		base.StartCoroutine("PredictionRoutine");
	}

	[DebuggerHidden]
	private IEnumerator PredictionRoutine()
	{
		CoopMutantPredictor.<PredictionRoutine>c__Iterator1F <PredictionRoutine>c__Iterator1F = new CoopMutantPredictor.<PredictionRoutine>c__Iterator1F();
		<PredictionRoutine>c__Iterator1F.<>f__this = this;
		return <PredictionRoutine>c__Iterator1F;
	}
}

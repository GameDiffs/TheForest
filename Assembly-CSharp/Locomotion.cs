using System;
using UnityEngine;

public class Locomotion
{
	private Animator m_Animator;

	private int m_SpeedId;

	private int m_AgularSpeedId;

	private int m_DirectionId;

	public float m_SpeedDampTime = 0.1f;

	public float m_AnguarSpeedDampTime = 0.25f;

	public float m_DirectionResponseTime = 0.2f;

	public Locomotion(Animator animator)
	{
		this.m_Animator = animator;
		this.m_SpeedId = Animator.StringToHash("Speed");
		this.m_AgularSpeedId = Animator.StringToHash("AngularSpeed");
		this.m_DirectionId = Animator.StringToHash("Direction");
	}

	public void Do(float speed, float direction)
	{
		AnimatorStateInfo currentAnimatorStateInfo = this.m_Animator.GetCurrentAnimatorStateInfo(0);
		bool flag = this.m_Animator.IsInTransition(0);
		bool flag2 = currentAnimatorStateInfo.IsName("Locomotion.Idle");
		bool flag3 = currentAnimatorStateInfo.IsName("Locomotion.TurnOnSpot") || currentAnimatorStateInfo.IsName("Locomotion.PlantNTurnLeft") || currentAnimatorStateInfo.IsName("Locomotion.PlantNTurnRight");
		bool flag4 = currentAnimatorStateInfo.IsName("Locomotion.WalkRun");
		float dampTime = (!flag2) ? this.m_SpeedDampTime : 0f;
		float dampTime2 = (!flag4 && !flag) ? 0f : this.m_AnguarSpeedDampTime;
		float dampTime3 = (float)((!flag3 && !flag) ? 0 : 1000000);
		float value = direction / this.m_DirectionResponseTime;
		this.m_Animator.SetFloatReflected(this.m_SpeedId, speed, dampTime, Time.deltaTime);
		this.m_Animator.SetFloatReflected(this.m_AgularSpeedId, value, dampTime2, Time.deltaTime);
		this.m_Animator.SetFloatReflected(this.m_DirectionId, direction, dampTime3, Time.deltaTime);
	}
}

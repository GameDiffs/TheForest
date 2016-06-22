using System;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(ThirdPersonCharacter))]
public class AICharacterControl : MonoBehaviour
{
	public Transform target;

	public float targetChangeTolerance = 1f;

	private Vector3 targetPos;

	public NavMeshAgent agent
	{
		get;
		private set;
	}

	public ThirdPersonCharacter character
	{
		get;
		private set;
	}

	private void Start()
	{
		this.agent = base.GetComponentInChildren<NavMeshAgent>();
		this.character = base.GetComponent<ThirdPersonCharacter>();
	}

	private void Update()
	{
		if (this.target != null)
		{
			if ((this.target.position - this.targetPos).magnitude > this.targetChangeTolerance)
			{
				this.targetPos = this.target.position;
				this.agent.SetDestination(this.targetPos);
			}
			this.agent.transform.position = base.transform.position;
			this.character.Move(this.agent.desiredVelocity, false, false, this.targetPos);
		}
		else
		{
			this.character.Move(Vector3.zero, false, false, base.transform.position + base.transform.forward * 100f);
		}
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
	}
}

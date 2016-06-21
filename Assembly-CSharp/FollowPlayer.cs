using System;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	public GameObject m_player;

	private Vector2 uvOffset = Vector2.zero;

	public Vector2 uvAnimationRate = new Vector2(0f, 0.2f);

	private Vector3 lastPos;

	private Vector3 currPos;

	public Vector2 viewDir;

	public Vector2 moveDir;

	public Vector3 targetSideDir;

	public float strafe;

	private void Start()
	{
		this.lastPos = this.m_player.transform.position;
	}

	private void Update()
	{
		Vector3 position = this.m_player.transform.position;
		position.y -= 15f;
		base.transform.position = position;
	}
}

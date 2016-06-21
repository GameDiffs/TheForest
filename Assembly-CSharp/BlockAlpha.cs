using System;
using UnityEngine;

public class BlockAlpha : MonoBehaviour
{
	public GameObject AlphaMessage;

	private Collider _collider;

	private Collider _playerCollider;

	private void Awake()
	{
		this.AlphaMessage.transform.position = new Vector3(0.5f, 0.5f, 0f);
		this._collider = base.GetComponent<Collider>();
		base.enabled = false;
	}

	private void Update()
	{
		if (!this._playerCollider || !this._collider.bounds.Intersects(this._playerCollider.bounds))
		{
			this.OnTriggerExit(this._playerCollider);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.AlphaMessage.SetActive(true);
			this._playerCollider = other;
			base.enabled = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other || other.gameObject.CompareTag("Player"))
		{
			this.AlphaMessage.SetActive(false);
			this._playerCollider = null;
			base.enabled = false;
		}
	}
}

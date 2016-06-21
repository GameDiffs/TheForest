using System;
using UnityEngine;

public class ReplaceWithNewPrefab : MonoBehaviour
{
	public Transform _newPrefab;

	public GameObject _replaceTarget;

	public bool _destroyIfClient;

	public bool _replaceIfClient;

	public bool _doLocalOnlyCheck;

	private void Start()
	{
		if (!BoltNetwork.isClient || this._replaceIfClient)
		{
			Transform transform = (Transform)UnityEngine.Object.Instantiate(this._newPrefab, this._replaceTarget.transform.position, this._replaceTarget.transform.rotation);
			if (BoltNetwork.isServer && !this._doLocalOnlyCheck)
			{
				BoltNetwork.Attach(transform.gameObject);
			}
			transform.parent = this._replaceTarget.transform.parent;
			this._replaceTarget.transform.parent = null;
			this._replaceTarget.SetActive(false);
			UnityEngine.Object.Destroy(this._replaceTarget);
		}
		else if (this._destroyIfClient)
		{
			BoltEntity component = this._replaceTarget.GetComponent<BoltEntity>();
			if (!this._doLocalOnlyCheck || !component || !component.isAttached)
			{
				UnityEngine.Object.Destroy(this._replaceTarget);
			}
		}
	}
}

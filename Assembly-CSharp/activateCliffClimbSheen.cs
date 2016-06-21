using System;
using TheForest.Items;
using TheForest.Tools;
using TheForest.Utils;
using UnityEngine;

public class activateCliffClimbSheen : MonoBehaviour
{
	[ItemIdPicker]
	public int _itemId;

	public GameObject _sheen;

	public activateCliffClimb _climb;

	public LayerMask _layerMask;

	private GameObject _targetObject;

	private bool _canMove = true;

	private void Awake()
	{
		this._sheen.SetActive(false);
		base.enabled = false;
	}

	private void Update()
	{
		if (this._climb.gameObject.activeSelf)
		{
			this._sheen.SetActive(false);
			this._sheen.transform.parent = base.transform;
		}
		else if (this._canMove)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(LocalPlayer.MainCamTr.position, LocalPlayer.MainCamTr.forward, out raycastHit, 6f, this._layerMask))
			{
				bool flag = false;
				notClimbable component = raycastHit.transform.GetComponent<notClimbable>();
				if (component)
				{
					flag = true;
				}
				if (!flag)
				{
					this._canMove = false;
					this._sheen.transform.parent = null;
					this._sheen.SetActive(true);
					Vector3 vector = raycastHit.point;
					vector += LocalPlayer.MainCamTr.forward * -1.5f;
					vector.y -= 1f;
					this._sheen.transform.position = Vector3.Lerp(this._sheen.transform.position, vector, Time.deltaTime * 20f);
				}
				else
				{
					this._sheen.SetActive(false);
					this._sheen.transform.parent = base.transform;
				}
			}
			else
			{
				this._sheen.SetActive(false);
				this._sheen.transform.parent = base.transform;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((1 << other.gameObject.layer & this._layerMask) > 0 && other.gameObject.CompareTag("climbWall"))
		{
			this._targetObject = other.gameObject;
			this._canMove = true;
			base.enabled = true;
			if (!LocalPlayer.Inventory.Owns(this._itemId))
			{
				EventRegistry.Player.Publish(TfEvent.StoryProgress, GameStats.StoryElements.FoundClimbWall);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == this._targetObject)
		{
			this._targetObject = null;
			base.enabled = false;
		}
	}
}

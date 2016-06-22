using Bolt;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class coopDeadSharkSetup : EntityBehaviour<IDeadSharkState>
{
	[SerializeField]
	public Transform targetTransform;

	private Rigidbody targetRigidbody;

	private sharkGoRagdoll goRagdoll;

	public GameObject spawnGo;

	public Transform[] jointsToSync;

	public Transform rootTr;

	public GameObject storePrefab;

	private void Start()
	{
		this.goRagdoll = base.transform.GetComponentInChildren<sharkGoRagdoll>();
	}

	public override void Attached()
	{
		float num = float.PositiveInfinity;
		GameObject gameObject = null;
		for (int i = 0; i < Scene.SceneTracker.storedRagDollPrefabs.Count; i++)
		{
			if (Scene.SceneTracker.storedRagDollPrefabs[i] != null)
			{
				float num2 = Vector3.Distance(base.transform.position, Scene.SceneTracker.storedRagDollPrefabs[i].transform.position);
				if (num2 < num)
				{
					num = num2;
					if (num2 < 1f)
					{
						gameObject = Scene.SceneTracker.storedRagDollPrefabs[i];
					}
				}
			}
		}
		if (gameObject)
		{
			storeLocalMutantInfo2 component = gameObject.GetComponent<storeLocalMutantInfo2>();
			base.transform.position = component.rootPosition;
			base.transform.rotation = component.rootRotation;
			for (int j = 0; j < this.jointsToSync.Length; j++)
			{
				this.jointsToSync[j].localRotation = component.jointAngles[j];
			}
			UnityEngine.Object.Destroy(gameObject);
			Scene.SceneTracker.storedRagDollPrefabs.RemoveAll((GameObject o) => o == null);
			Scene.SceneTracker.storedRagDollPrefabs.TrimExcess();
		}
	}

	private void destroyShark()
	{
		if (this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void switchToRagdoll()
	{
		this.goRagdoll.enableRagDoll();
	}

	private void switchToCutHead()
	{
		if (this.spawnGo)
		{
			if (BoltNetwork.isClient)
			{
				this.generateStorePrefab();
			}
			this.spawnGo.SetActive(true);
		}
	}

	private void generateStorePrefab()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.storePrefab, base.transform.position, base.transform.rotation) as GameObject;
		storeLocalMutantInfo2 component = gameObject.GetComponent<storeLocalMutantInfo2>();
		for (int i = 0; i < this.jointsToSync.Length; i++)
		{
			component.jointAngles.Add(this.jointsToSync[i].localRotation);
		}
		component.rootPosition = this.rootTr.position;
		component.rootRotation = this.rootTr.rotation;
		Scene.SceneTracker.storedRagDollPrefabs.Add(gameObject);
	}

	private void sendSyncRagDoll(GameObject go)
	{
		List<Quaternion> list = new List<Quaternion>();
		for (int i = 0; i < this.jointsToSync.Length; i++)
		{
			list.Add(this.jointsToSync[i].localRotation);
		}
		go.SendMessage("receiveSyncRagDoll", list, SendMessageOptions.DontRequireReceiver);
	}

	private void receiveSyncRagDoll(List<Quaternion> rot)
	{
		for (int i = 0; i < this.jointsToSync.Length; i++)
		{
			this.jointsToSync[i].localRotation = rot[i];
		}
	}
}

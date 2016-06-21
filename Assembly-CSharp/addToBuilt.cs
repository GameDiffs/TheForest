using System;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class addToBuilt : MonoBehaviour
{
	private sceneTracker info;

	public bool addToStructures;

	public bool addToRecentlyBuilt;

	public bool addToRabbitTraps;

	public bool addToFires;

	private bool setupDone;

	private void Awake()
	{
		this.info = Scene.SceneTracker;
	}

	private void OnDeserialized()
	{
		this.setupLists();
	}

	private void Start()
	{
		this.setupLists();
	}

	private void OnEnable()
	{
		this.setupLists();
	}

	private void setupLists()
	{
		if (!this.setupDone)
		{
			if (this.addToStructures && !this.info.structuresBuilt.Contains(base.gameObject))
			{
				this.info.addToStructures(base.gameObject);
			}
			if (this.addToRecentlyBuilt && !this.info.recentlyBuilt.Contains(base.gameObject))
			{
				this.info.addToBuilt(base.gameObject);
			}
			if (this.addToRabbitTraps && !this.info.allRabbitTraps.Contains(base.gameObject))
			{
				this.info.allRabbitTraps.Add(base.gameObject);
			}
			if (this.addToFires && !this.info.allPlayerFires.Contains(base.gameObject))
			{
				this.info.allPlayerFires.Add(base.gameObject);
			}
			this.setupDone = true;
		}
	}

	private void OnDestroy()
	{
		if (this.addToStructures && this.info.structuresBuilt.Contains(base.gameObject))
		{
			this.info.structuresBuilt.Remove(base.gameObject);
		}
		if (this.addToRecentlyBuilt && this.info.recentlyBuilt.Contains(base.gameObject))
		{
			this.info.recentlyBuilt.Remove(base.gameObject);
		}
		if (this.addToRabbitTraps && this.info.allRabbitTraps.Contains(base.gameObject))
		{
			this.info.allRabbitTraps.Remove(base.gameObject);
		}
		if (this.addToFires && this.info.allPlayerFires.Contains(base.gameObject))
		{
			this.info.allPlayerFires.Remove(base.gameObject);
		}
	}
}

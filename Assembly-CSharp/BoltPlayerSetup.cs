using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.World;
using TheForest.Networking;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

public class BoltPlayerSetup : EntityEventListener<IPlayerState>, IPriorityCalculator
{
	private itemConstrainToHand lh;

	private itemConstrainToHand rh;

	private itemConstrainToHand feet;

	private BatteryBasedLight plasticTorch;

	[SerializeField]
	private GameObject RespawnDeadTrigger;

	[SerializeField]
	private GameObject StealItemTrigger;

	private Transform sledLookat;

	bool IPriorityCalculator.Always
	{
		get
		{
			return true;
		}
	}

	float IPriorityCalculator.CalculateEventPriority(BoltConnection connection, Bolt.Event evnt)
	{
		return CoopUtils.CalculatePriorityFor(connection, this.entity, 1f, 1);
	}

	float IPriorityCalculator.CalculateStatePriority(BoltConnection connection, int skipped)
	{
		return CoopUtils.CalculatePriorityFor(connection, this.entity, 1f, skipped);
	}

	private void FindHands()
	{
		Transform[] allComponentsInChildren = base.transform.GetAllComponentsInChildren<Transform>();
		for (int i = 0; i < allComponentsInChildren.Length; i++)
		{
			Transform transform = allComponentsInChildren[i];
			if (transform.gameObject.name == "rightHandHeld")
			{
				this.rh = transform.GetComponent<itemConstrainToHand>();
				if (this.rh && this.lh && this.feet)
				{
					break;
				}
			}
			if (transform.gameObject.name == "leftHandHeld")
			{
				this.lh = transform.GetComponent<itemConstrainToHand>();
				if (this.rh && this.lh && this.feet)
				{
					break;
				}
			}
			if (transform.gameObject.name == "char_Hips")
			{
				this.feet = transform.GetComponent<itemConstrainToHand>();
				if (this.rh && this.lh && this.feet)
				{
					break;
				}
			}
		}
	}

	public void WaitForInventoryToBeEnabledAndThenDo(Action action)
	{
		base.StartCoroutine(this.WaitForInv(action));
	}

	[DebuggerHidden]
	private IEnumerator WaitForInv(Action action)
	{
		BoltPlayerSetup.<WaitForInv>c__Iterator14 <WaitForInv>c__Iterator = new BoltPlayerSetup.<WaitForInv>c__Iterator14();
		<WaitForInv>c__Iterator.action = action;
		<WaitForInv>c__Iterator.<$>action = action;
		return <WaitForInv>c__Iterator;
	}

	private void PlayerVariationBodySetup(int variationNumber)
	{
		variationNumber -= 10;
		CoopPlayerVariations component = base.GetComponent<CoopPlayerVariations>();
		if (component)
		{
			component.Body.material = component.BodyMaterials[variationNumber];
		}
	}

	private void PlayerVariationSetup(int variationNumber)
	{
		variationNumber -= 10;
		CoopPlayerVariations component = base.GetComponent<CoopPlayerVariations>();
		if (component)
		{
			CoopPlayerVariation[] variations = component.Variations;
			for (int i = 0; i < variations.Length; i++)
			{
				if (variations[i].Head)
				{
					variations[i].Head.gameObject.SetActive(i == variationNumber);
				}
				if (variations[i].Hair)
				{
					variations[i].Hair.gameObject.SetActive(i == variationNumber);
				}
			}
			component.Arms.materials = new Material[]
			{
				variations[variationNumber].MaterialArms
			};
		}
	}

	public override void Attached()
	{
		base.state.Transform.SetTransforms(base.transform);
		base.state.AddCallback("PlayerVariation", delegate
		{
			this.PlayerVariationSetup(base.state.PlayerVariation);
		});
		base.state.AddCallback("PlayerVariationBody", delegate
		{
			this.PlayerVariationBodySetup(base.state.PlayerVariationBody);
		});
		if (this.entity.isOwner)
		{
			CoopPlayerVariations component = base.GetComponent<CoopPlayerVariations>();
			base.state.PlayerVariation = LocalPlayer.Stats.PlayerVariation + 10;
			base.state.PlayerVariationBody = LocalPlayer.Stats.PlayerVariationBody + 10;
			if (base.state.PlayerVariation - 10 == 1)
			{
				this.entity.GetComponent<PlayerStats>().IsBlackMan();
			}
			this.PlayerVariationSetup(base.state.PlayerVariation);
			this.PlayerVariationBodySetup(base.state.PlayerVariationBody);
			this.FindHands();
		}
		else
		{
			Transform playerTr = base.transform;
			PlayerName pn = base.GetComponentInChildren<PlayerName>();
			base.state.AddCallback("name", delegate
			{
				pn.Init(this.state.name);
			});
			this.plasticTorch = base.GetComponentsInChildren<BatteryBasedLight>(true).FirstOrDefault<BatteryBasedLight>();
			if (BoltNetwork.isClient && Scene.SceneTracker)
			{
				if (!Scene.SceneTracker.allPlayers.Contains(base.gameObject))
				{
					Scene.SceneTracker.allPlayers.Add(this.entity.gameObject);
				}
				if (!Scene.SceneTracker.allPlayerEntities.Contains(this.entity))
				{
					Scene.SceneTracker.allPlayerEntities.Add(this.entity);
				}
			}
			base.state.AddCallback("CurrentView", delegate
			{
				if (this.state.CurrentView == 7 != this.RespawnDeadTrigger.activeSelf)
				{
					this.RespawnDeadTrigger.SetActive(!this.RespawnDeadTrigger.activeSelf);
				}
				pn.OnCurrentViewChanged();
				if (this.state.CurrentView == 8)
				{
					if (Scene.SceneTracker.allPlayers.Contains(this.gameObject))
					{
						Scene.SceneTracker.allPlayers.Remove(this.gameObject);
					}
					if (Scene.SceneTracker.allPlayerEntities.Contains(this.entity))
					{
						Scene.SceneTracker.allPlayerEntities.Remove(this.entity);
					}
					for (int i = playerTr.childCount - 1; i >= 0; i--)
					{
						Transform child = playerTr.GetChild(i);
						if (!child.GetComponent<Animator>())
						{
							UnityEngine.Object.Destroy(child.gameObject);
						}
						else
						{
							for (int j = child.childCount - 1; j >= 0; j--)
							{
								UnityEngine.Object.Destroy(child.GetChild(j).gameObject);
							}
							Component[] components = child.GetComponents(typeof(MonoBehaviour));
							Component[] array = components;
							for (int k = 0; k < array.Length; k++)
							{
								Component component2 = array[k];
								if (!(component2 is Animator))
								{
									UnityEngine.Object.DestroyImmediate(component2);
								}
							}
						}
					}
					Component[] components2 = this.GetComponents(typeof(MonoBehaviour));
					Component[] array2 = components2;
					for (int l = 0; l < array2.Length; l++)
					{
						Component component3 = array2[l];
						if (!(component3 is BoltEntity))
						{
							UnityEngine.Object.DestroyImmediate(component3);
						}
					}
					StealItemTrigger stealItemTrigger = UnityEngine.Object.Instantiate<StealItemTrigger>(Prefabs.Instance.DeadBackpackPrefab);
					stealItemTrigger._entity = this.entity;
					stealItemTrigger.transform.parent = playerTr;
					stealItemTrigger.transform.localPosition = Vector3.zero;
				}
			});
			base.state.AddCallback("BatteryTorchEnabled", delegate
			{
				this.plasticTorch.SetEnabled(this.state.BatteryTorchEnabled);
			});
			base.state.AddCallback("BatteryTorchColor", delegate
			{
				this.plasticTorch.SetColor(this.state.BatteryTorchColor);
			});
			base.state.AddCallback("BatteryTorchIntensity", delegate
			{
				this.plasticTorch.SetIntensity(this.state.BatteryTorchIntensity);
			});
		}
	}

	private void OnDestroy()
	{
		if (!Scene.SceneTracker)
		{
			return;
		}
		if (Scene.SceneTracker.allPlayers.Contains(base.gameObject))
		{
			Scene.SceneTracker.allPlayers.Remove(base.gameObject);
		}
		if (Scene.SceneTracker.allPlayerEntities.Contains(this.entity))
		{
			Scene.SceneTracker.allPlayerEntities.Remove(this.entity);
		}
	}

	public override void OnEvent(HitPlayer evnt)
	{
		if (this.entity.isOwner)
		{
			this.entity.SendMessage("hitFromEnemy", 5);
		}
	}

	private void UpdateLH()
	{
		if (this.lh)
		{
			for (int i = 0; i < this.lh.Available.Length; i++)
			{
				if (this.lh.Available[i].activeSelf)
				{
					base.state.itemInLeftHand = i;
					return;
				}
			}
			base.state.itemInLeftHand = -1;
		}
	}

	private void UpdateRH()
	{
		if (this.rh)
		{
			for (int i = 0; i < this.rh.Available.Length; i++)
			{
				if (this.rh.Available[i].activeSelf)
				{
					if (base.state.itemInRightHand != i)
					{
						base.state.itemInRightHand = i;
					}
					return;
				}
			}
			base.state.itemInRightHand = -1;
		}
	}

	private void UpdateFeet()
	{
		if (this.feet)
		{
			for (int i = 0; i < this.feet.Available.Length; i++)
			{
				if (this.feet.Available[i].activeSelf)
				{
					if (base.state.itemAtFeet != i)
					{
						base.state.itemAtFeet = i;
					}
					return;
				}
			}
			base.state.itemAtFeet = -1;
		}
	}

	private void Update()
	{
		if (this.entity.IsOwner())
		{
			this.UpdateLH();
			this.UpdateRH();
			this.UpdateFeet();
		}
	}

	internal void SnapToSpawn()
	{
		base.StartCoroutine(this.SnapToSpawnRoutine());
	}

	[DebuggerHidden]
	private IEnumerator SnapToSpawnRoutine()
	{
		BoltPlayerSetup.<SnapToSpawnRoutine>c__Iterator15 <SnapToSpawnRoutine>c__Iterator = new BoltPlayerSetup.<SnapToSpawnRoutine>c__Iterator15();
		<SnapToSpawnRoutine>c__Iterator.<>f__this = this;
		return <SnapToSpawnRoutine>c__Iterator;
	}
}

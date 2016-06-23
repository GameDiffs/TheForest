using PathologicalGames;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Items.Core;
using TheForest.Items.Inventory;
using TheForest.Networking;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Save
{
	[DoNotSerializePublic]
	public class PlayerRespawnMP : MonoBehaviour
	{
		public float _minDownDuration = 5f;

		public float _respawnDuration = 5f;

		public float _maxDownDuration = 120f;

		public bool _blockDeathInput;

		private GameObject _lastCorpse;

		private float _deathTime;

		private float _manualRespawnTime;

		private static PlayerRespawnMP _instance;

		public static PlayerRespawnMP Instance
		{
			get
			{
				if (!PlayerRespawnMP._instance)
				{
					GameObject gameObject = new GameObject("PlayerPlanePosition");
					GameObject gameObject2 = GameObject.Find("Hull(Clone)") ?? GameObject.Find("Hull");
					gameObject.transform.parent = gameObject2.transform;
					gameObject.transform.localPosition = new Vector3(0f, 0.0335f, 0f);
					PlayerRespawnMP._instance = gameObject.AddComponent<PlayerRespawnMP>();
				}
				return PlayerRespawnMP._instance;
			}
			set
			{
				PlayerRespawnMP._instance = value;
			}
		}

		private void Awake()
		{
			PlayerRespawnMP.Instance = this;
			base.enabled = false;
		}

		private void Update()
		{
			if (!Scene.Cams.DeadCam.activeSelf)
			{
				if (!Scene.HudGui.MpRespawnLabel.gameObject.activeSelf && this._deathTime + this._minDownDuration < Time.time)
				{
					Scene.HudGui.MpRespawnLabel.gameObject.SetActive(true);
				}
				else if ((TheForest.Utils.Input.GetButtonDown("Take") && Scene.HudGui.MpRespawnMaxTimer.gameObject.activeSelf) || this._deathTime + this._maxDownDuration < Time.time)
				{
					UnityEngine.Debug.Log("killed player");
					PlayerRespawnMP.KillPlayer();
				}
			}
			else if (this._manualRespawnTime + this._respawnDuration < Time.time)
			{
				this.Respawn();
			}
			Scene.HudGui.MpRespawnMaxTimer.fillAmount = Mathf.Clamp01((Time.time - this._deathTime) / this._maxDownDuration);
		}

		private void OnDestroy()
		{
			if (PlayerRespawnMP.Instance == this)
			{
				PlayerRespawnMP.Instance = null;
			}
		}

		public static void TakeDownPlayer()
		{
			Scene.HudGui.MpRespawnMaxTimer.fillAmount = 0f;
			Scene.HudGui.MpRespawnMaxTimer.gameObject.SetActive(true);
			Scene.SceneTracker.allPlayers.Remove(LocalPlayer.GameObject);
			LocalPlayer.Create.CancelPlace();
			LocalPlayer.Create.CloseTheBook();
			LocalPlayer.Inventory.Close();
			LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.Death;
			LocalPlayer.FpCharacter.enabled = false;
			LocalPlayer.MainRotator.enabled = false;
			LocalPlayer.CamRotator.enabled = false;
			LocalPlayer.GameObject.GetComponent<Rigidbody>().isKinematic = true;
			LocalPlayer.GameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			PlayerRespawnMP.Instance._deathTime = Time.time;
			PlayerRespawnMP.Instance.enabled = true;
		}

		public static void offsetDeathTime()
		{
			PlayerRespawnMP.Instance._deathTime = Time.time;
		}

		public static void enableRespawnTimer()
		{
			Scene.HudGui.MpRespawnMaxTimer.fillAmount = 0f;
			Scene.HudGui.MpRespawnMaxTimer.gameObject.SetActive(true);
			Scene.SceneTracker.allPlayers.Remove(LocalPlayer.GameObject);
			LocalPlayer.Create.CancelPlace();
			LocalPlayer.Create.CloseTheBook();
			LocalPlayer.Inventory.Close();
			LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.Death;
			PlayerRespawnMP.Instance._deathTime = Time.time;
			PlayerRespawnMP.Instance.enabled = true;
		}

		public static bool IsKillable()
		{
			return PlayerRespawnMP.Instance._deathTime + 0.5f < Time.time && PlayerRespawnMP.Instance.enabled;
		}

		public static void KillPlayer()
		{
			if (!Scene.Cams.DeadCam.activeSelf)
			{
				Scene.Cams.DeadCam.SetActive(true);
				Scene.HudGui.GuiCamC.enabled = false;
				LocalPlayer.PlayerDeadCam.SetActive(true);
				Scene.HudGui.MpRespawnMaxTimer.gameObject.SetActive(false);
				Scene.HudGui.MpRespawnLabel.gameObject.SetActive(false);
				ChatBox.Instance.Close();
				LocalPlayer.Animator.SetBool("injuredBool", false);
				LocalPlayer.Animator.SetBool("deathBool", true);
				PlayerRespawnMP.Instance._manualRespawnTime = Time.time;
				PlayerRespawnMP.Instance.enabled = true;
			}
		}

		public static void Cancel()
		{
			Scene.HudGui.GuiCamC.enabled = true;
			Scene.Cams.DeadCam.SetActive(false);
			LocalPlayer.PlayerDeadCam.SetActive(false);
			Scene.HudGui.MpRespawnLabel.gameObject.SetActive(false);
			Scene.HudGui.MpRespawnMaxTimer.gameObject.SetActive(false);
			if (!Scene.SceneTracker.allPlayers.Contains(LocalPlayer.GameObject))
			{
				Scene.SceneTracker.allPlayers.Add(LocalPlayer.GameObject);
			}
			LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.World;
			LocalPlayer.Inventory.enabled = true;
			LocalPlayer.FpCharacter.enabled = true;
			LocalPlayer.CamFollowHead.followAnim = false;
			LocalPlayer.CamRotator.resetOriginalRotation = true;
			LocalPlayer.MainRotator.enabled = true;
			LocalPlayer.CamRotator.enabled = true;
			LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
			LocalPlayer.MainCamTr.localEulerAngles = Vector3.zero;
			LocalPlayer.Transform.eulerAngles = new Vector3(0f, LocalPlayer.Transform.eulerAngles.y, 0f);
			LocalPlayer.PlayerBase.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			LocalPlayer.AnimControl.StartCoroutine("smoothEnableLayerNew", 4);
			LocalPlayer.ScriptSetup.pmControl.SendEvent("toResetPlayer");
			LocalPlayer.GameObject.SendMessage("GotBloody");
			LocalPlayer.GameObject.GetComponent<Rigidbody>().isKinematic = false;
			if (LocalPlayer.Stats.Fullness < 0.35f)
			{
				LocalPlayer.Stats.Fullness = 0.35f;
			}
			if (LocalPlayer.Stats.Thirst > 0.35f)
			{
				LocalPlayer.Stats.Thirst = 0.35f;
			}
			PlayerRespawnMP.Instance.enabled = false;
		}

		private void forceAnimSpineReset()
		{
			LocalPlayer.Animator.SetLayerWeight(4, 1f);
		}

		private void Respawn()
		{
			UnityEngine.Debug.Log("LocalPlayer -> Respawn");
			if (LocalPlayer.Stats.Dead)
			{
				LocalPlayer.GameObject.SendMessage("NotInACave");
				PlayerInventory inventory = LocalPlayer.Inventory;
				string name = LocalPlayer.Entity.GetState<IPlayerState>().name;
				LocalPlayer.Inventory.HideAllEquiped(false);
				LocalPlayer.Inventory.enabled = false;
				if (Scene.SceneTracker.allPlayers.Contains(LocalPlayer.GameObject))
				{
					Scene.SceneTracker.allPlayers.Remove(LocalPlayer.GameObject);
				}
				if (Scene.SceneTracker.allPlayerEntities.Contains(LocalPlayer.Entity))
				{
					Scene.SceneTracker.allPlayerEntities.Remove(LocalPlayer.Entity);
				}
				BoltNetwork.Detach(LocalPlayer.Entity);
				GameObject gameObject = LocalPlayer.GameObject;
				BoltEntity entity = LocalPlayer.Entity;
				gameObject.name = "player Corpse - " + name;
				gameObject.tag = "Untagged";
				LocalPlayer.MainCamTr.parent = LocalPlayer.Transform;
				LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.Loot;
				for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
				{
					Transform child = gameObject.transform.GetChild(i);
					UnityEngine.Object.Destroy(child.gameObject);
				}
				Component[] components = gameObject.GetComponents(typeof(MonoBehaviour));
				Component[] array = components;
				for (int j = 0; j < array.Length; j++)
				{
					Component component = array[j];
					if (!(component is BoltEntity))
					{
						UnityEngine.Object.DestroyImmediate(component);
					}
				}
				Transform transform = base.transform;
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(Prefabs.Instance.PlayerPrefab, transform.position, transform.rotation);
				gameObject2.transform.localEulerAngles = new Vector3(0f, gameObject2.transform.localEulerAngles.y, 0f);
				gameObject2.name = Prefabs.Instance.PlayerPrefab.name;
				LocalPlayer.FpCharacter.UnLockView();
				LocalPlayer.CamFollowHead.enableMouseControl(false);
				LocalPlayer.MainCamTr.localEulerAngles = Vector3.zero;
				LocalPlayer.MainRotator.enabled = true;
				LocalPlayer.CamRotator.enabled = true;
				LocalPlayer.Stats.Health = 28f;
				LocalPlayer.Stats.Energy = 100f;
				LocalPlayer.Stats.Fullness = 0.35f;
				LocalPlayer.Stats.Thirst = 0.35f;
				LocalPlayer.Stats.Invoke("CheckArmsStart", 2f);
				LocalPlayer.Stats.Invoke("PlayWakeMusic", 0.5f);
				Scene.RainFollowGO.GetComponent<SmoothTransformConstraint>().target = LocalPlayer.Transform;
				gameObject2.SetActive(true);
				CoopUtils.AttachLocalPlayer(gameObject2, name);
				Scene.SceneTracker.allPlayers.Add(LocalPlayer.GameObject);
				LocalPlayer.GreebleRoot.SetActive(true);
				StealItemTrigger stealItemTrigger = (StealItemTrigger)UnityEngine.Object.Instantiate(Prefabs.Instance.DeadBackpackPrefab, gameObject.transform.position, gameObject.transform.rotation);
				stealItemTrigger._entity = entity;
				stealItemTrigger.transform.parent = gameObject.transform;
				gameObject.AddComponent<DeathMPTut>();
				ItemStorage cis = gameObject.AddComponent<ItemStorage>();
				for (int k = 0; k < inventory._possessedItems.Count; k++)
				{
					InventoryItem inventoryItem = inventory._possessedItems[k];
					if (!LocalPlayer.Inventory.Owns(inventoryItem._itemId))
					{
						this.AddItemToStorage(inventoryItem._itemId, inventoryItem._amount, inventoryItem._maxAmountBonus, cis);
					}
				}
				for (int l = 0; l < inventory.EquipmentSlots.Length; l++)
				{
					InventoryItemView inventoryItemView = inventory.EquipmentSlots[l];
					if (inventoryItemView && inventoryItemView._itemId > 0)
					{
						this.AddItemToStorage(inventoryItemView._itemId, 1, 0, cis);
					}
				}
				animalAI[] array2 = UnityEngine.Object.FindObjectsOfType<animalAI>();
				animalAI[] array3 = array2;
				for (int m = 0; m < array3.Length; m++)
				{
					animalAI animalAI = array3[m];
					animalAI.SendMessage("updatePlayerTargets");
				}
				mutantAI[] array4 = UnityEngine.Object.FindObjectsOfType<mutantAI>();
				mutantAI[] array5 = array4;
				for (int n = 0; n < array5.Length; n++)
				{
					mutantAI mutantAI = array5[n];
					mutantAI.SendMessage("updatePlayerTargets");
				}
				Fish[] array6 = UnityEngine.Object.FindObjectsOfType<Fish>();
				Fish[] array7 = array6;
				for (int num = 0; num < array7.Length; num++)
				{
					Fish fish = array7[num];
					fish.SendMessage("updatePlayerTargets");
				}
				mutantScriptSetup[] array8 = UnityEngine.Object.FindObjectsOfType<mutantScriptSetup>();
				mutantScriptSetup[] array9 = array8;
				for (int num2 = 0; num2 < array9.Length; num2++)
				{
					mutantScriptSetup mutantScriptSetup = array9[num2];
					mutantScriptSetup.setupPlayer();
					mutantScriptSetup.search.refreshCurrentTarget();
				}
				Terrain.activeTerrain.GetComponent<Collider>().enabled = true;
				Scene.Clock.IsNotCave();
			}
			PlayerRespawnMP.Cancel();
		}

		private void AddItemToStorage(int itemId, int amount, int maxAmountBonus, ItemStorage cis)
		{
			if (ItemDatabase.IsItemidValid(itemId))
			{
				Item item = ItemDatabase.ItemById(itemId);
				if (item != null)
				{
					if (!item.MatchType(Item.Types.Story))
					{
						cis.Add(itemId, amount, maxAmountBonus);
					}
					else if (amount > 0)
					{
						base.StartCoroutine(this.DelayedAddItem(itemId, amount));
					}
				}
				else
				{
					UnityEngine.Debug.LogError("Item not found while creating loot backpack id=" + itemId);
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedAddItem(int itemId, int amount)
		{
			PlayerRespawnMP.<DelayedAddItem>c__Iterator1B0 <DelayedAddItem>c__Iterator1B = new PlayerRespawnMP.<DelayedAddItem>c__Iterator1B0();
			<DelayedAddItem>c__Iterator1B.itemId = itemId;
			<DelayedAddItem>c__Iterator1B.amount = amount;
			<DelayedAddItem>c__Iterator1B.<$>itemId = itemId;
			<DelayedAddItem>c__Iterator1B.<$>amount = amount;
			return <DelayedAddItem>c__Iterator1B;
		}

		private void SetLayerRecursively(Transform tr, LayerMask layer)
		{
			tr.gameObject.layer = layer;
			foreach (Transform tr2 in tr)
			{
				this.SetLayerRecursively(tr2, layer);
			}
		}
	}
}

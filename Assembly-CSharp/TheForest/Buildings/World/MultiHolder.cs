using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class MultiHolder : EntityBehaviour<IMultiHolderState>
	{
		public enum ContentTypes
		{
			None,
			Log,
			Body,
			Rock
		}

		public GameObject[] LogRender;

		public GameObject[] RockRender;

		public GameObject[] MutantBodySlots;

		public GameObject TakeIcon;

		public GameObject AddIcon;

		public GameObject TakeRockIcon;

		public GameObject AddRockIcon;

		public bool Pushable;

		[ItemIdPicker]
		public int RockItemId;

		[SerializeThis]
		public MultiHolder.ContentTypes _content;

		[SerializeThis]
		public int _contentAmount;

		[SerializeThis]
		public EnemyType[] _bodyTypes;

		private GameObject[] _bodies;

		private FMOD_StudioEventEmitter _emitter;

		private float _originalMass;

		private float _originalDrag;

		[SerializeField]
		private SphereCollider _sc;

		public MultiHolder.ContentTypes _contentTypeActual
		{
			get
			{
				return this._content;
			}
			set
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
				{
					base.state.ContentType = (int)value;
				}
				this._content = value;
			}
		}

		public int _contentActual
		{
			get
			{
				if (BoltNetwork.isRunning)
				{
					return base.state.LogCount;
				}
				return this._contentAmount;
			}
			set
			{
				if (BoltNetwork.isRunning)
				{
					base.state.LogCount = value;
				}
				this._contentAmount = value;
			}
		}

		private void Awake()
		{
			this._emitter = base.GetComponent<FMOD_StudioEventEmitter>();
			this._originalMass = base.transform.parent.GetComponent<Rigidbody>().mass;
			this._originalDrag = base.transform.parent.GetComponent<Rigidbody>().drag;
			this.SetEnabled(false);
		}

		private bool MP_CanInterract()
		{
			if (!this.entity || !this.entity.isAttached || !Grabber.FocusedItem || !Grabber.FocusedItem.gameObject.Equals(base.gameObject))
			{
				return false;
			}
			if (base.state.IsReal)
			{
				return !base.state.GrabbedBy;
			}
			return !this.entity.isOwner;
		}

		private void Update()
		{
			if (BoltNetwork.isRunning)
			{
				if (this.MP_CanInterract())
				{
					MultiHolder.ContentTypes contentTypes = MultiHolder.ContentTypes.None;
					if (this._contentTypeActual == MultiHolder.ContentTypes.Log || this._contentTypeActual == MultiHolder.ContentTypes.None)
					{
						this.LogContentUpdate(ref contentTypes);
					}
					if (this._contentTypeActual == MultiHolder.ContentTypes.Body || this._contentTypeActual == MultiHolder.ContentTypes.None)
					{
					}
					if (this._contentTypeActual == MultiHolder.ContentTypes.Rock || this._contentTypeActual == MultiHolder.ContentTypes.None)
					{
						this.RockContentUpdate(ref contentTypes);
					}
					this.AddIcon.SetActive(contentTypes == MultiHolder.ContentTypes.Log || contentTypes == MultiHolder.ContentTypes.Body);
					this.AddRockIcon.SetActive(contentTypes == MultiHolder.ContentTypes.Rock);
				}
				else
				{
					if (this.AddIcon.activeSelf)
					{
						this.AddIcon.SetActive(false);
					}
					if (this.TakeIcon.activeSelf)
					{
						this.TakeIcon.SetActive(false);
					}
					if (this.AddRockIcon.activeSelf)
					{
						this.AddRockIcon.SetActive(false);
					}
					if (this.TakeRockIcon.activeSelf)
					{
						this.TakeRockIcon.SetActive(false);
					}
				}
			}
			else if (!LocalPlayer.FpCharacter.PushingSled)
			{
				MultiHolder.ContentTypes contentTypes2 = MultiHolder.ContentTypes.None;
				if (this._contentTypeActual == MultiHolder.ContentTypes.Log || this._contentTypeActual == MultiHolder.ContentTypes.None)
				{
					this.LogContentUpdate(ref contentTypes2);
				}
				if (this._contentTypeActual == MultiHolder.ContentTypes.Body || this._contentTypeActual == MultiHolder.ContentTypes.None)
				{
					this.BodyContentUpdate(ref contentTypes2);
				}
				if (this._contentTypeActual == MultiHolder.ContentTypes.Rock || this._contentTypeActual == MultiHolder.ContentTypes.None)
				{
					this.RockContentUpdate(ref contentTypes2);
				}
				this.AddIcon.SetActive(contentTypes2 == MultiHolder.ContentTypes.Log || contentTypes2 == MultiHolder.ContentTypes.Body);
				this.AddRockIcon.SetActive(contentTypes2 == MultiHolder.ContentTypes.Rock);
			}
			else
			{
				if (this.AddIcon.activeSelf)
				{
					this.AddIcon.SetActive(false);
				}
				if (this.TakeIcon.activeSelf)
				{
					this.TakeIcon.SetActive(false);
				}
				if (this.AddRockIcon.activeSelf)
				{
					this.AddRockIcon.SetActive(false);
				}
				if (this.TakeRockIcon.activeSelf)
				{
					this.TakeRockIcon.SetActive(false);
				}
			}
		}

		private void OnDeserialized()
		{
			if (this._contentTypeActual == MultiHolder.ContentTypes.Body)
			{
				if (this._bodies == null)
				{
					this._bodies = new GameObject[3];
				}
				if (this._bodyTypes == null)
				{
					this._bodyTypes = new EnemyType[3];
				}
				for (int i = 0; i < this._contentActual; i++)
				{
					this.SpawnBody(i);
				}
			}
			else if (this._contentTypeActual == MultiHolder.ContentTypes.Rock && !BoltNetwork.isRunning)
			{
				for (int j = this._contentActual - 1; j >= 0; j--)
				{
					this.RockRender[j].SetActive(true);
				}
			}
		}

		private void SetEnabled(bool value)
		{
			base.enabled = value;
		}

		private void GrabEnter()
		{
			if (!LocalPlayer.AnimControl.doSledPushMode)
			{
				LocalPlayer.Inventory.DontShowDrop = true;
				this.SetEnabled(true);
			}
		}

		public void GrabExit()
		{
			if (this.AddIcon.activeSelf)
			{
				this.AddIcon.SetActive(false);
			}
			if (this.TakeIcon.activeSelf)
			{
				this.TakeIcon.SetActive(false);
			}
			if (this.AddRockIcon.activeSelf)
			{
				this.AddRockIcon.SetActive(false);
			}
			if (this.TakeRockIcon.activeSelf)
			{
				this.TakeRockIcon.SetActive(false);
			}
			LocalPlayer.Inventory.DontShowDrop = false;
			this.SetEnabled(false);
		}

		private void RefreshMassAndDrag()
		{
			if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner && !base.state.IsReal)
			{
				return;
			}
			if (this.Pushable)
			{
				base.transform.parent.GetComponent<Rigidbody>().mass = this._originalMass + (float)(((this._contentTypeActual != MultiHolder.ContentTypes.Log) ? 20 : 10) * this._contentActual);
				base.transform.parent.GetComponent<Rigidbody>().drag = this._originalDrag + 0.5f * (float)this._contentActual;
			}
		}

		private void RockContentUpdate(ref MultiHolder.ContentTypes showAddIcon)
		{
			bool flag = !BoltNetwork.isRunning || (base.transform.position - LocalPlayer.GameObject.transform.position).magnitude < 5f;
			if ((!BoltNetwork.isRunning && this._contentActual > 0 && LocalPlayer.Inventory.AmountOf(this.RockItemId, true) < 5) || (BoltNetwork.isRunning && this._contentActual > 0 && LocalPlayer.Inventory.AmountOf(this.RockItemId, true) < 5 && flag))
			{
				this.TakeRockIcon.SetActive(true);
				if (TheForest.Utils.Input.GetButtonDown("Take"))
				{
					if (BoltNetwork.isRunning)
					{
						ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
						itemHolderTakeItem.ContentType = (int)this._contentTypeActual;
						itemHolderTakeItem.Target = this.entity;
						itemHolderTakeItem.Player = LocalPlayer.Entity;
						itemHolderTakeItem.Send();
					}
					else if (LocalPlayer.Inventory.AddItem(this.RockItemId, 1, false, false, (WeaponStatUpgrade.Types)(-2)))
					{
						this.RockRender[this._contentActual - 1].SetActive(false);
						this._contentActual--;
						if (this._contentActual == 0)
						{
							this._contentTypeActual = MultiHolder.ContentTypes.None;
						}
						this.RefreshMassAndDrag();
					}
				}
			}
			else
			{
				this.TakeRockIcon.SetActive(false);
			}
			if (this._contentActual < this.RockRender.Length && LocalPlayer.Inventory.Owns(this.RockItemId) && flag && (this._content == MultiHolder.ContentTypes.Rock || (!LocalPlayer.Inventory.Logs.HasLogs && !LocalPlayer.AnimControl.carry)))
			{
				showAddIcon = MultiHolder.ContentTypes.Rock;
				if (TheForest.Utils.Input.GetButtonDown("Craft"))
				{
					LocalPlayer.Inventory.RemoveItem(this.RockItemId, 1, false);
					this._emitter.Play();
					if (BoltNetwork.isRunning)
					{
						ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
						itemHolderAddItem.ContentType = 3;
						itemHolderAddItem.Target = this.entity;
						itemHolderAddItem.Send();
					}
					else
					{
						this._contentTypeActual = MultiHolder.ContentTypes.Rock;
						this._contentActual++;
						this.RockRender[this._contentActual - 1].SetActive(true);
						this.RefreshMassAndDrag();
					}
				}
			}
		}

		private void LogContentUpdate(ref MultiHolder.ContentTypes showAddIcon)
		{
			bool flag = !BoltNetwork.isRunning || (base.transform.position - LocalPlayer.GameObject.transform.position).magnitude < 5f;
			if ((!BoltNetwork.isRunning && this._contentActual > 0 && LocalPlayer.Inventory.Logs.Amount < 2 && !LocalPlayer.AnimControl.carry) || (BoltNetwork.isRunning && this._contentActual > 0 && LocalPlayer.Inventory.Logs.Amount < 2 && !LocalPlayer.AnimControl.carry && flag))
			{
				this.TakeIcon.SetActive(true);
				if (TheForest.Utils.Input.GetButtonDown("Take"))
				{
					if (BoltNetwork.isRunning)
					{
						ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
						itemHolderTakeItem.ContentType = (int)this._contentTypeActual;
						itemHolderTakeItem.Target = this.entity;
						itemHolderTakeItem.Player = LocalPlayer.Entity;
						itemHolderTakeItem.Send();
					}
					else if (LocalPlayer.Inventory.Logs.Lift())
					{
						this.LogRender[this._contentActual - 1].SetActive(false);
						this._contentActual--;
						if (this._contentActual == 0)
						{
							this._contentTypeActual = MultiHolder.ContentTypes.None;
						}
						this.RefreshMassAndDrag();
					}
				}
			}
			else
			{
				this.TakeIcon.SetActive(false);
			}
			if (this._contentActual < this.LogRender.Length && LocalPlayer.Inventory.Logs.Amount > 0 && flag)
			{
				showAddIcon = MultiHolder.ContentTypes.Log;
				if (TheForest.Utils.Input.GetButtonDown("Craft"))
				{
					this._contentTypeActual = MultiHolder.ContentTypes.Log;
					this._emitter.Play();
					LocalPlayer.Inventory.Logs.PutDown(false, false, true);
					if (BoltNetwork.isRunning)
					{
						ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
						itemHolderAddItem.ContentType = 1;
						itemHolderAddItem.Target = this.entity;
						itemHolderAddItem.Send();
					}
					else
					{
						this._contentActual++;
						this.LogRender[this._contentActual - 1].SetActive(true);
						this.RefreshMassAndDrag();
					}
				}
			}
		}

		private void BodyContentUpdate_MP(ref MultiHolder.ContentTypes showAddIcon)
		{
			if ((base.state.Body0 || base.state.Body1 || base.state.Body2) && !LocalPlayer.Inventory.Logs.HasLogs)
			{
				this.TakeIcon.SetActive(true);
				if (TheForest.Utils.Input.GetButtonDown("Take"))
				{
					TakeBody takeBody = TakeBody.Create(GlobalTargets.OnlyServer);
					takeBody.Sled = this.entity;
					if (base.state.Body2)
					{
						takeBody.Body = base.state.Body2;
					}
					else if (base.state.Body1)
					{
						takeBody.Body = base.state.Body1;
					}
					else if (base.state.Body0)
					{
						takeBody.Body = base.state.Body0;
					}
					takeBody.Send();
					Debug.Log("TakeBody:Send");
				}
			}
			if (LocalPlayer.AnimControl.placedBodyGo && LocalPlayer.AnimControl.placedBodyGo.GetComponentInChildren<BoltEntity>() && (!base.state.Body0 || !base.state.Body1 || !base.state.Body2))
			{
				showAddIcon = MultiHolder.ContentTypes.Body;
				if (TheForest.Utils.Input.GetButtonDown("Craft"))
				{
					GameObject placedBodyGo = LocalPlayer.AnimControl.placedBodyGo;
					AddBody addBody = AddBody.Create(GlobalTargets.OnlyServer);
					addBody.Body = placedBodyGo.GetComponentInChildren<BoltEntity>();
					addBody.Sled = this.entity;
					addBody.Send();
					LocalPlayer.AnimControl.heldBodyGo.SetActive(false);
					LocalPlayer.Animator.SetBoolReflected("bodyHeld", false);
					LocalPlayer.AnimControl.carry = false;
					LocalPlayer.AnimControl.placedBodyGo = null;
					LocalPlayer.Inventory.ShowAllEquiped();
					Scene.HudGui.DropButton.SetActive(false);
					placedBodyGo.SetActive(true);
					placedBodyGo.SendMessage("dropFromCarry", SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		private void BodyContentUpdate(ref MultiHolder.ContentTypes showAddIcon)
		{
			if (BoltNetwork.isRunning)
			{
				this.BodyContentUpdate_MP(ref showAddIcon);
				return;
			}
			if (this._contentActual > 0 && !LocalPlayer.AnimControl.heldBodyGo.activeSelf && !LocalPlayer.Inventory.Logs.HasLogs)
			{
				this.TakeIcon.SetActive(true);
				if (TheForest.Utils.Input.GetButtonDown("Take"))
				{
					LocalPlayer.AnimControl.setMutantPickUp(this.PickUpBody());
					this.RefreshMassAndDrag();
					this.TakeIcon.SetActive(false);
				}
			}
			if (this._contentActual < 3 && LocalPlayer.AnimControl.carry)
			{
				showAddIcon = MultiHolder.ContentTypes.Body;
				if (TheForest.Utils.Input.GetButtonDown("Craft"))
				{
					if (this._bodies == null)
					{
						this._bodies = new GameObject[3];
					}
					if (this._bodyTypes == null || this._bodyTypes.Length < 3)
					{
						this._bodyTypes = new EnemyType[3];
					}
					this._contentTypeActual = MultiHolder.ContentTypes.Body;
					this._emitter.Play();
					LocalPlayer.AnimControl.heldBodyGo.SetActive(false);
					GameObject placedBodyGo = LocalPlayer.AnimControl.placedBodyGo;
					dummyTypeSetup dummyTypeSetup = placedBodyGo.GetComponent<dummyTypeSetup>() ?? placedBodyGo.GetComponentInChildren<dummyTypeSetup>();
					placedBodyGo.SetActive(true);
					placedBodyGo.SendMessage("dropFromCarry", SendMessageOptions.DontRequireReceiver);
					MultiHolder.GetTriggerChild(placedBodyGo.transform).gameObject.SetActive(false);
					placedBodyGo.transform.position = this.MutantBodySlots[this._contentActual].transform.position;
					placedBodyGo.transform.rotation = this.MutantBodySlots[this._contentActual].transform.rotation;
					placedBodyGo.transform.parent = base.transform.root;
					dummyAnimatorControl dummyAnimatorControl = placedBodyGo.GetComponent<dummyAnimatorControl>() ?? placedBodyGo.GetComponentInChildren<dummyAnimatorControl>();
					dummyAnimatorControl.enabled = false;
					this._bodyTypes[this._contentActual] = dummyTypeSetup._type;
					this._bodies[this._contentActual] = placedBodyGo;
					this._contentActual++;
					this.RefreshMassAndDrag();
					Scene.HudGui.DropButton.SetActive(false);
					LocalPlayer.Animator.SetBoolReflected("bodyHeld", false);
					LocalPlayer.AnimControl.carry = false;
					LocalPlayer.AnimControl.placedBodyGo = null;
					LocalPlayer.Inventory.ShowAllEquiped();
				}
			}
		}

		public void TakeBodyMP(BoltEntity body, BoltConnection from)
		{
			if (!body)
			{
				return;
			}
			dummyTypeSetup componentInChildren = body.gameObject.GetComponentInChildren<dummyTypeSetup>();
			MultiHolder multiHolder;
			if (base.state.IsReal)
			{
				IMultiHolderState state = base.state;
				multiHolder = this;
			}
			else
			{
				if (!base.state.Replaces)
				{
					return;
				}
				IMultiHolderState state = base.state.Replaces.GetState<IMultiHolderState>();
				multiHolder = base.state.Replaces.GetComponentsInChildren<MultiHolder>(true)[0];
			}
			bool flag = false;
			TakeBodyApprove takeBodyApprove = (from != null) ? TakeBodyApprove.Create(from) : TakeBodyApprove.Create(GlobalTargets.OnlySelf);
			if (base.state.Body0 == body)
			{
				Debug.Log("TakeBody:Body0:" + base.state.Body0);
				flag = true;
				takeBodyApprove.Body = body;
				takeBodyApprove.Send();
				base.state.Body0 = null;
				multiHolder._contentActual = 0;
				multiHolder._contentTypeActual = MultiHolder.ContentTypes.None;
				multiHolder._bodyTypes[0] = EnemyType.regularMale;
			}
			else if (base.state.Body1 == body)
			{
				Debug.Log("TakeBody:Body1:" + base.state.Body1);
				flag = true;
				takeBodyApprove.Body = body;
				takeBodyApprove.Send();
				base.state.Body1 = null;
				multiHolder._contentActual = 1;
				multiHolder._bodyTypes[1] = EnemyType.regularMale;
			}
			else if (base.state.Body2 == body)
			{
				Debug.Log("TakeBody:Body2:" + base.state.Body2);
				flag = true;
				takeBodyApprove.Body = body;
				takeBodyApprove.Send();
				base.state.Body2 = null;
				multiHolder._contentActual = 2;
				multiHolder._bodyTypes[2] = EnemyType.regularMale;
			}
			if (flag)
			{
				body.GetState<IMutantState>().Transform.SetTransforms(body.transform);
			}
		}

		public void AddBodyMP(BoltEntity body)
		{
			Debug.Log("addbodymp1");
			if (!body)
			{
				return;
			}
			Debug.Log("addbodymp2");
			dummyTypeSetup componentInChildren = body.gameObject.GetComponentInChildren<dummyTypeSetup>();
			IMultiHolderState state;
			MultiHolder multiHolder;
			if (base.state.IsReal)
			{
				state = base.state;
				multiHolder = this;
				Debug.Log("addbodymp3-1");
			}
			else
			{
				state = base.state.Replaces.GetState<IMultiHolderState>();
				multiHolder = base.state.Replaces.GetComponentsInChildren<MultiHolder>(true)[0];
				Debug.Log("addbodymp3-2");
			}
			if (multiHolder._bodyTypes == null || multiHolder._bodyTypes.Length == 0)
			{
				Debug.Log("addbodymp4");
				multiHolder._bodyTypes = new EnemyType[3];
			}
			if (state.Body0 == null)
			{
				Debug.Log("addbodymp5");
				state.Body0 = body;
				state.Body0.GetState<IMutantState>().Transform.SetTransforms(null, null);
				multiHolder._bodyTypes[0] = componentInChildren._type;
				multiHolder._contentActual = 1;
				multiHolder._contentTypeActual = MultiHolder.ContentTypes.Body;
				MultiHolder.GetTriggerChild(body.transform).gameObject.SetActive(false);
			}
			else if (state.Body1 == null)
			{
				Debug.Log("addbodymp6");
				state.Body1 = body;
				state.Body1.GetState<IMutantState>().Transform.SetTransforms(null, null);
				multiHolder._bodyTypes[1] = componentInChildren._type;
				multiHolder._contentActual = 2;
				multiHolder._contentTypeActual = MultiHolder.ContentTypes.Body;
				MultiHolder.GetTriggerChild(body.transform).gameObject.SetActive(false);
			}
			else if (state.Body2 == null)
			{
				Debug.Log("addbodymp7");
				state.Body2 = body;
				state.Body2.GetState<IMutantState>().Transform.SetTransforms(null, null);
				multiHolder._bodyTypes[2] = componentInChildren._type;
				multiHolder._contentActual = 3;
				multiHolder._contentTypeActual = MultiHolder.ContentTypes.Body;
				MultiHolder.GetTriggerChild(body.transform).gameObject.SetActive(false);
			}
		}

		public static Transform GetTriggerChild(Transform t)
		{
			Transform transform = t.transform.Find("Trigger");
			if (!transform)
			{
				int num = LayerMask.NameToLayer("PickUp");
				foreach (Transform transform2 in t)
				{
					foreach (Transform transform3 in transform2)
					{
						if (transform3.gameObject.layer == num)
						{
							transform = transform3;
							break;
						}
					}
					if (transform)
					{
						break;
					}
				}
			}
			return transform;
		}

		private void SpawnBody(int bodyId)
		{
			Debug.Log("SPAWN BODY:" + bodyId);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Prefabs.Instance._deadMutantBodies[(int)this._bodyTypes[bodyId]]);
			gameObject.SetActive(true);
			gameObject.transform.Find("Trigger").gameObject.SetActive(false);
			gameObject.transform.position = this.MutantBodySlots[bodyId].transform.position;
			gameObject.transform.rotation = this.MutantBodySlots[bodyId].transform.rotation;
			gameObject.transform.parent = base.transform.root;
			gameObject.SendMessage("dropFromCarry", SendMessageOptions.DontRequireReceiver);
			gameObject.GetComponent<dummyAnimatorControl>().enabled = false;
			this._bodies[bodyId] = gameObject;
		}

		private GameObject PickUpBody()
		{
			this._contentActual--;
			if (this._contentActual == 0)
			{
				this._contentTypeActual = MultiHolder.ContentTypes.None;
			}
			GameObject gameObject = this._bodies[this._contentActual];
			this._bodies[this._contentActual] = null;
			gameObject.transform.parent = null;
			MultiHolder.GetTriggerChild(gameObject.transform).gameObject.SetActive(true);
			dummyAnimatorControl dummyAnimatorControl = gameObject.GetComponent<dummyAnimatorControl>() ?? gameObject.GetComponentInChildren<dummyAnimatorControl>();
			dummyAnimatorControl.enabled = true;
			return gameObject;
		}

		public override void Attached()
		{
			if (!BoltNetwork.isServer || this.entity.isOwner)
			{
			}
			base.state.AddCallback("LogCount", new PropertyCallbackSimple(this.ItemCountChangedMP));
			if (BoltNetwork.isServer && this.entity.isOwner && this._content == MultiHolder.ContentTypes.Body)
			{
				int contentAmount = this._contentAmount;
				for (int i = 0; i < contentAmount; i++)
				{
					base.state.IsReal = true;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Prefabs.Instance._deadMutantBodies[(int)this._bodyTypes[i]]);
					gameObject.SetActive(true);
					BoltNetwork.Attach(gameObject);
					gameObject.SendMessage("dropFromCarry", SendMessageOptions.DontRequireReceiver);
					gameObject.GetComponent<dummyAnimatorControl>().enabled = false;
					this.AddBodyMP(gameObject.GetComponent<BoltEntity>());
				}
			}
		}

		public void TakeItemMP(BoltEntity targetPlayer, MultiHolder.ContentTypes type)
		{
			if (type != this._contentTypeActual)
			{
				return;
			}
			if (this._contentActual > 0)
			{
				if (BoltNetwork.isServer)
				{
					PlayerAddItem playerAddItem;
					if (targetPlayer.isOwner)
					{
						playerAddItem = PlayerAddItem.Create(GlobalTargets.OnlySelf);
					}
					else
					{
						playerAddItem = PlayerAddItem.Create(targetPlayer.source);
					}
					if (this._contentTypeActual == MultiHolder.ContentTypes.Rock)
					{
						playerAddItem.ItemId = this.RockItemId;
					}
					else
					{
						playerAddItem.ItemId = LocalPlayer.Inventory.Logs._logItemId;
					}
					playerAddItem.Send();
				}
				if (this.entity.isOwner)
				{
					this._contentActual = Mathf.Max(this._contentActual - 1, 0);
					if (this._contentActual == 0)
					{
						this._contentTypeActual = MultiHolder.ContentTypes.None;
					}
					this.RefreshMassAndDrag();
				}
				else
				{
					ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(this.entity.source);
					itemHolderTakeItem.Target = this.entity;
					itemHolderTakeItem.ContentType = (int)this._contentTypeActual;
					itemHolderTakeItem.Send();
				}
				return;
			}
		}

		public void AddItemMP(MultiHolder.ContentTypes type, BoltConnection source)
		{
			if ((this._contentTypeActual == MultiHolder.ContentTypes.Log || this._contentTypeActual == MultiHolder.ContentTypes.None) && type == MultiHolder.ContentTypes.Log)
			{
				if (type == MultiHolder.ContentTypes.None)
				{
					this._contentActual = 0;
				}
				if (this.entity.isOwner)
				{
					this._contentTypeActual = MultiHolder.ContentTypes.Log;
					if (this._contentActual < this.LogRender.Length)
					{
						this._contentActual = Mathf.Min(this._contentActual + 1, this.LogRender.Length);
					}
					else
					{
						PlayerAddItem playerAddItem = PlayerAddItem.Create(source);
						playerAddItem.ItemId = LocalPlayer.Inventory.Logs._logItemId;
						playerAddItem.Send();
					}
					this.RefreshMassAndDrag();
				}
				else
				{
					ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(this.entity.source);
					itemHolderAddItem.Target = this.entity;
					itemHolderAddItem.ContentType = 1;
					itemHolderAddItem.Send();
				}
			}
			if ((this._contentTypeActual == MultiHolder.ContentTypes.Rock || this._contentTypeActual == MultiHolder.ContentTypes.None) && type == MultiHolder.ContentTypes.Rock)
			{
				if (type == MultiHolder.ContentTypes.None)
				{
					this._contentActual = 0;
				}
				if (this.entity.isOwner)
				{
					this._contentTypeActual = MultiHolder.ContentTypes.Rock;
					if (this._contentActual < this.LogRender.Length)
					{
						this._contentActual = Mathf.Min(this._contentActual + 1, this.RockRender.Length);
					}
					else
					{
						PlayerAddItem playerAddItem2 = PlayerAddItem.Create(source);
						playerAddItem2.ItemId = this.RockItemId;
						playerAddItem2.Send();
					}
					this.RefreshMassAndDrag();
				}
				else
				{
					ItemHolderAddItem itemHolderAddItem2 = ItemHolderAddItem.Create(this.entity.source);
					itemHolderAddItem2.ContentType = 3;
					itemHolderAddItem2.Target = this.entity;
					itemHolderAddItem2.Send();
				}
			}
		}

		public void ItemCountChangedMP()
		{
			this._contentAmount = base.state.LogCount;
			if (base.state.GrabbedBy)
			{
				return;
			}
			for (int i = 0; i < this.RockRender.Length; i++)
			{
				this.RockRender[i].SetActive(false);
			}
			for (int j = 0; j < this.LogRender.Length; j++)
			{
				this.LogRender[j].SetActive(false);
			}
			if (this._contentTypeActual == MultiHolder.ContentTypes.Log)
			{
				for (int k = 0; k < this._contentActual; k++)
				{
					this.LogRender[k].SetActive(true);
				}
			}
			if (this._contentTypeActual == MultiHolder.ContentTypes.Rock)
			{
				for (int l = 0; l < this._contentActual; l++)
				{
					this.RockRender[l].SetActive(true);
				}
			}
		}
	}
}

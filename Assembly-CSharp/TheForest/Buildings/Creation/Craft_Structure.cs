using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Buildings.World;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Tools;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Craft Structure")]
	public class Craft_Structure : EntityBehaviour
	{
		[Serializable]
		public class BuildIngredients : ReceipeIngredient
		{
			public enum TextOptions
			{
				PresentOverTotal,
				Present,
				Total
			}

			public GameObject[] _renderers;

			public GameObject _icon;

			public GameObject _text;

			public Craft_Structure.BuildIngredients.TextOptions _textDisplayMode;

			public Craft_Structure.PickupAxis _pickupAxis;
		}

		public enum PickupAxis
		{
			X = 1,
			Y,
			Z = 0
		}

		public bool manualLoading;

		public Color ColorGrey;

		public Color ColorRed;

		public GameObject Built;

		public Create.BuildingTypes _type;

		public List<Craft_Structure.BuildIngredients> _requiredIngredients;

		public bool _playTwinkle = true;

		[SerializeThis]
		private ReceipeIngredient[] _presentIngredients;

		[SerializeThis]
		private int _presentIngredientsCount;

		private bool _initialized;

		private bool _grabbed;

		private GameObject _ghost;

		public Action<GameObject> OnBuilt = delegate
		{
		};

		public bool WasLoaded
		{
			get;
			set;
		}

		public bool IsInitialized
		{
			get
			{
				return this._initialized;
			}
			set
			{
				this._initialized = value;
			}
		}

		private void OnSerializing()
		{
			this._presentIngredientsCount = this._presentIngredients.Length;
		}

		private void OnDeserialized()
		{
			base.enabled = false;
			if (this._presentIngredients != null && this._presentIngredients.Length > this._presentIngredientsCount)
			{
				ReceipeIngredient[] array = new ReceipeIngredient[this._presentIngredientsCount];
				for (int i = 0; i < this._presentIngredientsCount; i++)
				{
					array[i] = this._presentIngredients[i];
				}
				this._presentIngredients = array;
			}
			if (this.manualLoading)
			{
				this.WasLoaded = true;
			}
			else
			{
				this.Initialize();
			}
		}

		public void Initialize()
		{
			if (!this._initialized)
			{
				if (this._presentIngredients == null)
				{
					this._presentIngredients = new ReceipeIngredient[this._requiredIngredients.Count];
				}
				this.SetUpIcons();
				if (this._presentIngredients.Length != this._requiredIngredients.Count)
				{
					this._presentIngredients = new ReceipeIngredient[this._requiredIngredients.Count];
				}
				for (int i = 0; i < this._requiredIngredients.Count; i++)
				{
					Craft_Structure.BuildIngredients buildIngredients = this._requiredIngredients[i];
					if (this._presentIngredients[i] == null)
					{
						this._presentIngredients[i] = new ReceipeIngredient
						{
							_itemID = buildIngredients._itemID
						};
					}
					ReceipeIngredient receipeIngredient = this._presentIngredients[i];
					int amount = buildIngredients._amount - receipeIngredient._amount;
					BuildMission.AddNeededToBuildMission(buildIngredients._itemID, amount);
					for (int j = 0; j < receipeIngredient._amount; j++)
					{
						if (j >= buildIngredients._renderers.Length)
						{
							break;
						}
						buildIngredients._renderers[j].SetActive(true);
					}
				}
				this._initialized = true;
				if (BoltNetwork.isRunning)
				{
					base.gameObject.AddComponent<CoopConstruction>();
					if (BoltNetwork.isServer && this.entity.isAttached)
					{
						this.UpdateNetworkIngredients();
					}
				}
				if (!BoltNetwork.isClient)
				{
					this.CheckNeeded();
				}
			}
		}

		private void Start()
		{
			if (this._presentIngredients == null)
			{
				this._presentIngredients = new ReceipeIngredient[this._requiredIngredients.Count];
			}
			base.enabled = (this._initialized || this._grabbed);
			if (!this.manualLoading && base.transform.root != LocalPlayer.Transform)
			{
				if (!LevelSerializer.IsDeserializing)
				{
					this.Initialize();
				}
				else
				{
					this.SetUpIcons();
				}
			}
			this._ghost = base.transform.parent.gameObject;
		}

		private void Update()
		{
			if (this._initialized)
			{
				this.CheckText();
				this.CheckNeeded();
				Scene.HudGui.DestroyIcon.gameObject.SetActive(true);
				if (TheForest.Utils.Input.GetButtonAfterDelay("Craft", 0.5f))
				{
					this.CancelBlueprint();
					return;
				}
				for (int i = 0; i < this._requiredIngredients.Count; i++)
				{
					if (this._requiredIngredients[i]._amount != this._presentIngredients[i]._amount)
					{
						Craft_Structure.BuildIngredients buildIngredients = this._requiredIngredients[i];
						ReceipeIngredient receipeIngredient = this._presentIngredients[i];
						if (buildIngredients._amount > receipeIngredient._amount)
						{
							if (!LocalPlayer.Inventory.Owns(this._requiredIngredients[i]._itemID) && !Cheats.Creative)
							{
								buildIngredients._icon.GetComponent<GUITexture>().color = this.ColorRed;
							}
							else
							{
								buildIngredients._icon.GetComponent<GUITexture>().color = this.ColorGrey;
								Scene.HudGui.AddIcon.SetActive(true);
								if (TheForest.Utils.Input.GetButtonDown("Take") || (Cheats.Creative && TheForest.Utils.Input.GetButton("Take")))
								{
									this.AddIngredient(i);
									break;
								}
							}
						}
					}
				}
			}
		}

		private void OnDestroy()
		{
			this.AllOff();
		}

		public void GrabEnter()
		{
			LocalPlayer.Inventory.DontShowDrop = true;
			base.enabled = true;
			this._grabbed = true;
		}

		public void GrabExit()
		{
			this.AllOff();
			base.enabled = false;
			this._grabbed = false;
		}

		public override void Attached()
		{
			if (BoltNetwork.isServer && this._initialized)
			{
				this.UpdateNetworkIngredients();
			}
		}

		public override void Detached()
		{
			this.AllOff();
		}

		public ReceipeIngredient[] GetPresentIngredients()
		{
			return this._presentIngredients;
		}

		private void SetUpIcons()
		{
			if (Application.isPlaying)
			{
				if (this._requiredIngredients != null)
				{
					float num = 1f / ((float)Screen.width / 70f);
					Vector3 position = new Vector3(0.5f - num * ((float)this._requiredIngredients.Count - 0.5f), 0.15f, 0f);
					for (int i = 0; i < this._requiredIngredients.Count; i++)
					{
						Craft_Structure.BuildIngredients buildIngredients = this._requiredIngredients[i];
						if (buildIngredients._icon.transform.parent != base.transform)
						{
							buildIngredients._icon = UnityEngine.Object.Instantiate<GameObject>(buildIngredients._icon);
							buildIngredients._icon.transform.parent = base.transform;
							buildIngredients._icon.transform.position = position;
							buildIngredients._text = UnityEngine.Object.Instantiate<GameObject>(buildIngredients._text);
							buildIngredients._text.transform.parent = base.transform;
							position.z += 1f;
							buildIngredients._text.transform.position = position;
							position.z -= 1f;
							position.x += num;
						}
						buildIngredients._icon.SetActive(false);
						buildIngredients._text.SetActive(false);
					}
				}
				else
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		private void AllOff()
		{
			try
			{
				Scene.HudGui.AddIcon.SetActive(false);
				Scene.HudGui.DestroyIcon.SetActive(false);
				if (Application.isPlaying)
				{
					for (int i = 0; i < this._requiredIngredients.Count; i++)
					{
						Craft_Structure.BuildIngredients buildIngredients = this._requiredIngredients[i];
						buildIngredients._text.SetActive(false);
						buildIngredients._icon.SetActive(false);
					}
				}
				LocalPlayer.Inventory.DontShowDrop = false;
			}
			catch
			{
			}
		}

		private void CheckNeeded()
		{
			for (int i = 0; i < this._requiredIngredients.Count; i++)
			{
				if (this._presentIngredients[i]._amount < this._requiredIngredients[i]._amount)
				{
					return;
				}
			}
			this.Build();
		}

		private void CheckText()
		{
			if (Application.isPlaying)
			{
				for (int i = 0; i < this._requiredIngredients.Count; i++)
				{
					Craft_Structure.BuildIngredients buildIngredients = this._requiredIngredients[i];
					ReceipeIngredient receipeIngredient = this._presentIngredients[i];
					if (buildIngredients != null && receipeIngredient != null && receipeIngredient._amount != buildIngredients._amount)
					{
						if (!buildIngredients._text.activeSelf)
						{
							buildIngredients._text.SetActive(true);
							buildIngredients._icon.SetActive(true);
						}
						switch (buildIngredients._textDisplayMode)
						{
						case Craft_Structure.BuildIngredients.TextOptions.PresentOverTotal:
							buildIngredients._text.GetComponent<GUIText>().text = receipeIngredient._amount + "/" + buildIngredients._amount;
							break;
						case Craft_Structure.BuildIngredients.TextOptions.Present:
							buildIngredients._text.GetComponent<GUIText>().text = receipeIngredient._amount.ToString();
							break;
						case Craft_Structure.BuildIngredients.TextOptions.Total:
							buildIngredients._text.GetComponent<GUIText>().text = buildIngredients._amount.ToString();
							break;
						}
					}
					else if (buildIngredients._text.activeSelf)
					{
						buildIngredients._text.SetActive(false);
						buildIngredients._icon.SetActive(false);
					}
				}
			}
		}

		private void AddIngredient(int ingredientNum)
		{
			Craft_Structure.BuildIngredients buildIngredients = this._requiredIngredients[ingredientNum];
			if (LocalPlayer.Inventory.RemoveItem(buildIngredients._itemID, 1, false) || Cheats.Creative)
			{
				LocalPlayer.Sfx.PlayHammer();
				Scene.HudGui.AddIcon.SetActive(false);
				if (BoltNetwork.isRunning)
				{
					AddIngredient addIngredient = global::AddIngredient.Raise(GlobalTargets.OnlyServer);
					addIngredient.IngredientNum = ingredientNum;
					addIngredient.Construction = base.GetComponentInParent<BoltEntity>();
					addIngredient.Send();
				}
				else
				{
					this.AddIngrendient_Actual(ingredientNum, true);
				}
			}
		}

		public void UpdateNeededRenderers()
		{
			for (int i = 0; i < this._requiredIngredients.Count; i++)
			{
				Craft_Structure.BuildIngredients buildIngredients = this._requiredIngredients[i];
				ReceipeIngredient receipeIngredient = this._presentIngredients[i];
				int num = 0;
				while (num < receipeIngredient._amount && num < buildIngredients._renderers.Length)
				{
					if (!buildIngredients._renderers[num].activeSelf)
					{
						buildIngredients._renderers[num].SetActive(true);
					}
					num++;
				}
			}
		}

		public void AddIngrendient_Actual(int ingredientNum, bool local)
		{
			Craft_Structure.BuildIngredients buildIngredients = this._requiredIngredients[ingredientNum];
			ReceipeIngredient receipeIngredient = this._presentIngredients[ingredientNum];
			if (receipeIngredient._amount >= buildIngredients._amount)
			{
				this.UpdateNetworkIngredients();
				return;
			}
			receipeIngredient._amount++;
			this.UpdateNeededRenderers();
			BuildMission.AddNeededToBuildMission(receipeIngredient._itemID, -1);
			if (BoltNetwork.isRunning)
			{
				this.UpdateNetworkIngredients();
			}
			this.CheckNeeded();
		}

		public void UpdateNetworkIngredients()
		{
			if (BoltNetwork.isRunning)
			{
				BoltEntity componentInParent = base.GetComponentInParent<BoltEntity>();
				if (componentInParent.isOwner && this._presentIngredients != null)
				{
					IConstructionState state = componentInParent.GetState<IConstructionState>();
					for (int i = 0; i < this._presentIngredients.Length; i++)
					{
						state.Ingredients[i].Count = this._presentIngredients[i]._amount;
					}
				}
			}
		}

		public void CancelBlueprintSafe()
		{
			GameStats.CancelledStructure.Invoke();
			for (int i = 0; i < this._requiredIngredients.Count; i++)
			{
				Craft_Structure.BuildIngredients buildIngredients = this._requiredIngredients[i];
				ReceipeIngredient receipeIngredient = this._presentIngredients[i];
				if (buildIngredients != null && receipeIngredient != null)
				{
					int num = buildIngredients._amount - receipeIngredient._amount;
					BuildMission.AddNeededToBuildMission(receipeIngredient._itemID, -num);
					int amount = this._presentIngredients[i]._amount;
					if (amount > 0)
					{
						Transform transform = BoltNetwork.isRunning ? ItemDatabase.ItemById(this._presentIngredients[i]._itemID)._pickupPrefabMP : ItemDatabase.ItemById(this._presentIngredients[i]._itemID)._pickupPrefab;
						if (transform)
						{
							Craft_Structure.PickupAxis pickupAxis = this._requiredIngredients[i]._pickupAxis;
							float f = (float)amount * 0.428571433f + 1f;
							int num2 = Mathf.Min(Mathf.RoundToInt(f), 10);
							for (int j = 0; j < num2; j++)
							{
								int num3 = Mathf.RoundToInt((float)j / (float)num2 * (float)amount);
								if (this._requiredIngredients[i]._renderers.Length <= num3)
								{
									break;
								}
								Transform transform2 = this._requiredIngredients[i]._renderers[num3].transform;
								Transform transform3 = BoltNetwork.isRunning ? BoltNetwork.Instantiate(transform.gameObject).transform : UnityEngine.Object.Instantiate<Transform>(transform);
								transform3.position = transform2.position;
								switch (pickupAxis)
								{
								case Craft_Structure.PickupAxis.Z:
									transform3.rotation = transform2.rotation;
									break;
								case Craft_Structure.PickupAxis.X:
									transform3.rotation = Quaternion.LookRotation(transform2.right);
									break;
								case Craft_Structure.PickupAxis.Y:
									transform3.rotation = Quaternion.LookRotation(transform2.up);
									break;
								}
							}
						}
					}
				}
			}
			this.CheckText();
			this.AllOff();
			if (BoltNetwork.isRunning && this.entity.isAttached)
			{
				BoltNetwork.Destroy(this.entity);
			}
			else
			{
				UnityEngine.Object.Destroy(this._ghost);
			}
			base.enabled = false;
		}

		public void CancelBlueprint()
		{
			if (BoltNetwork.isRunning)
			{
				CancelBluePrint cancelBluePrint = CancelBluePrint.Raise(GlobalTargets.OnlyServer);
				cancelBluePrint.BluePrint = this.entity;
				cancelBluePrint.Send();
			}
			else
			{
				this.CancelBlueprintSafe();
			}
			LocalPlayer.Sfx.PlayRemove();
		}

		private void BuildFx()
		{
		}

		private void Build()
		{
			if (this._type != Create.BuildingTypes.None)
			{
				EventRegistry.Player.Publish(TfEvent.BuiltStructure, this._type);
				this._type = Create.BuildingTypes.None;
			}
			if (BoltNetwork.isClient)
			{
				if (base.enabled)
				{
					base.enabled = false;
					this.AllOff();
				}
				return;
			}
			if (!this._ghost)
			{
				this._ghost = base.transform.parent.gameObject;
			}
			GameObject gameObject;
			if (BoltNetwork.isServer)
			{
				if (this.entity.attachToken != null)
				{
					if (this.entity.attachToken is CoopWallChunkToken)
					{
						(this.entity.attachToken as CoopWallChunkToken).Additions = this.entity.GetComponent<WallChunkArchitect>().Addition;
					}
					gameObject = BoltNetwork.Instantiate(this.Built, this.entity.attachToken, this._ghost.transform.position, this._ghost.transform.rotation).gameObject;
				}
				else
				{
					gameObject = BoltNetwork.Instantiate(this.Built, this.entity.attachToken, this._ghost.transform.position, this._ghost.transform.rotation).gameObject;
					BoltEntity component = gameObject.GetComponent<BoltEntity>();
					if (component && component.isAttached && component.StateIs<IMultiHolderState>())
					{
						component.GetState<IMultiHolderState>().IsReal = true;
					}
					BoltEntity component2 = gameObject.GetComponent<BoltEntity>();
					if (component2 && component.isAttached && component2.StateIs<IRaftState>())
					{
						component2.GetState<IRaftState>().IsReal = true;
					}
				}
			}
			else
			{
				gameObject = (GameObject)UnityEngine.Object.Instantiate(this.Built, this._ghost.transform.position, this._ghost.transform.rotation);
			}
			TreeStructure component3 = this._ghost.GetComponent<TreeStructure>();
			if (component3)
			{
				TreeStructure treeStructure = gameObject.GetComponent<TreeStructure>();
				if (!treeStructure)
				{
					treeStructure = gameObject.AddComponent<TreeStructure>();
				}
				treeStructure.TreeId = component3.TreeId;
			}
			ropeSetGroundHeight component4 = gameObject.GetComponent<ropeSetGroundHeight>();
			if (component4)
			{
				gameObject.SendMessage("setGroundTriggerHeight", SendMessageOptions.DontRequireReceiver);
			}
			if (this._ghost.transform.parent != null)
			{
				gameObject.transform.parent = this._ghost.transform.parent;
			}
			this.OnBuilt(gameObject);
			this.OnBuilt = null;
			base.enabled = false;
			this._initialized = false;
			if (this._ghost)
			{
				base.StartCoroutine(this.DelayedDestroy());
			}
			else
			{
				this.AllOff();
			}
			if (this._playTwinkle && LocalPlayer.Sfx)
			{
				LocalPlayer.Sfx.PlayBuildingComplete(gameObject, true);
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedDestroy()
		{
			Craft_Structure.<DelayedDestroy>c__Iterator131 <DelayedDestroy>c__Iterator = new Craft_Structure.<DelayedDestroy>c__Iterator131();
			<DelayedDestroy>c__Iterator.<>f__this = this;
			return <DelayedDestroy>c__Iterator;
		}
	}
}

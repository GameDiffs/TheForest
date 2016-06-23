using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TheForest.Player
{
	public class PlayerVariationSelector : MonoBehaviour
	{
		public CoopPlayerVariations _baseModelPrefab;

		private Animator _animator;

		private CoopPlayerVariations _instanceModel;

		private int _bodyVariation;

		private int _headVariation;

		private int _hairVariation;

		private int _skinVariation;

		public bool nextbodyVariation;

		public bool nextheadVariation;

		public bool nexthairVariation;

		public bool nextskinVariation;

		private void Awake()
		{
			this._instanceModel = UnityEngine.Object.Instantiate<CoopPlayerVariations>(this._baseModelPrefab);
			for (int i = this._instanceModel.transform.childCount - 1; i >= 0; i--)
			{
				Transform child = this._instanceModel.transform.GetChild(i);
				if (child.name != "player_BASE")
				{
					UnityEngine.Object.DestroyImmediate(child.gameObject);
				}
				else
				{
					MonoBehaviour[] components = child.GetComponents<MonoBehaviour>();
					MonoBehaviour[] array = components;
					for (int j = 0; j < array.Length; j++)
					{
						MonoBehaviour obj = array[j];
						UnityEngine.Object.DestroyImmediate(obj);
					}
					this._animator = child.GetComponent<Animator>();
				}
			}
			MonoBehaviour[] components2 = this._instanceModel.GetComponents<MonoBehaviour>();
			MonoBehaviour[] array2 = components2;
			for (int k = 0; k < array2.Length; k++)
			{
				MonoBehaviour monoBehaviour = array2[k];
				if (!(monoBehaviour is CoopPlayerVariations))
				{
					UnityEngine.Object.DestroyImmediate(monoBehaviour);
				}
			}
			itemConstrainToHand[] componentsInChildren = this._instanceModel.GetComponentsInChildren<itemConstrainToHand>();
			itemConstrainToHand[] array3 = componentsInChildren;
			for (int l = 0; l < array3.Length; l++)
			{
				itemConstrainToHand itemConstrainToHand = array3[l];
				if (!itemConstrainToHand.fixedItems)
				{
					UnityEngine.Object.DestroyImmediate(itemConstrainToHand.gameObject);
				}
			}
			this._instanceModel.transform.parent = base.transform;
			this._instanceModel.transform.localPosition = Vector3.zero;
			this._instanceModel.transform.localRotation = Quaternion.identity;
		}

		private void Update()
		{
			if (this.nextbodyVariation)
			{
				this.nextbodyVariation = false;
				this.NextBodyVariation();
			}
			if (this.nextheadVariation)
			{
				this.nextheadVariation = false;
				this.NextHeadVariation();
			}
			if (this.nexthairVariation)
			{
				this.nexthairVariation = false;
				this.NextHairVariation();
			}
			if (this.nextskinVariation)
			{
				this.nextskinVariation = false;
				this.NextSkinVariation();
			}
		}

		private void SetBodyVariation(int variationNumber)
		{
			if (this._instanceModel)
			{
				this._bodyVariation = variationNumber;
				this._instanceModel.Body.sharedMaterial = this._instanceModel.BodyMaterials[variationNumber];
			}
		}

		private void NextBodyVariation()
		{
			if (this._instanceModel)
			{
				this.SetBodyVariation((int)Mathf.Repeat((float)(this._bodyVariation + 1), (float)this._instanceModel.BodyMaterials.Length));
			}
		}

		private void SetHeadVariation(int variationNumber)
		{
			if (this._instanceModel)
			{
				this._headVariation = variationNumber;
				CoopPlayerVariation[] variations = this._instanceModel.Variations;
				for (int i = 0; i < variations.Length; i++)
				{
					if (variations[i].Head)
					{
						if (i == variationNumber)
						{
							variations[i].Head.gameObject.SetActive(true);
							variations[i].Head.sharedMaterial = variations[this._skinVariation].Materialhead;
						}
						else
						{
							variations[i].Head.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		private void NextHeadVariation()
		{
			if (this._instanceModel)
			{
				this.SetHeadVariation((int)Mathf.Repeat((float)(this._headVariation + 1), (float)this._instanceModel.Variations.Length));
			}
		}

		private void SetHairVariation(int variationNumber)
		{
			if (this._instanceModel)
			{
				this._hairVariation = variationNumber;
				CoopPlayerVariation[] variations = this._instanceModel.Variations;
				for (int i = 0; i < variations.Length; i++)
				{
					if (variations[i].Hair)
					{
						variations[i].Hair.gameObject.SetActive(i == variationNumber);
					}
				}
			}
		}

		private void NextHairVariation()
		{
			if (this._instanceModel)
			{
				this.SetHairVariation((int)Mathf.Repeat((float)(this._hairVariation + 1), (float)this._instanceModel.Variations.Length));
			}
		}

		private void SetSkinVariation(int variationNumber)
		{
			if (this._instanceModel)
			{
				this._skinVariation = variationNumber;
				CoopPlayerVariation[] variations = this._instanceModel.Variations;
				this._instanceModel.Arms.sharedMaterials = new Material[]
				{
					variations[variationNumber].MaterialArms
				};
				variations[this._headVariation].Head.sharedMaterial = variations[variationNumber].Materialhead;
				base.StartCoroutine(this.DoCheckArms());
			}
		}

		private void NextSkinVariation()
		{
			if (this._instanceModel)
			{
				this.SetSkinVariation((int)Mathf.Repeat((float)(this._skinVariation + 1), (float)this._instanceModel.Variations.Length));
			}
		}

		[DebuggerHidden]
		private IEnumerator DoCheckArms()
		{
			PlayerVariationSelector.<DoCheckArms>c__Iterator187 <DoCheckArms>c__Iterator = new PlayerVariationSelector.<DoCheckArms>c__Iterator187();
			<DoCheckArms>c__Iterator.<>f__this = this;
			return <DoCheckArms>c__Iterator;
		}
	}
}

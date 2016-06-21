using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class AnimationLink : NodeLink2
	{
		[Serializable]
		public class LinkClip
		{
			public AnimationClip clip;

			public Vector3 velocity;

			public int loopCount = 1;

			public string name
			{
				get
				{
					return (!(this.clip != null)) ? string.Empty : this.clip.name;
				}
			}
		}

		public string clip;

		public float animSpeed = 1f;

		public bool reverseAnim = true;

		public GameObject referenceMesh;

		public AnimationLink.LinkClip[] sequence;

		public string boneRoot = "bn_COG_Root";

		private static Transform SearchRec(Transform tr, string name)
		{
			int childCount = tr.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = tr.GetChild(i);
				if (child.name == name)
				{
					return child;
				}
				Transform transform = AnimationLink.SearchRec(child, name);
				if (transform != null)
				{
					return transform;
				}
			}
			return null;
		}

		public void CalculateOffsets(List<Vector3> trace, out Vector3 endPosition)
		{
			endPosition = base.transform.position;
			if (this.referenceMesh == null)
			{
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(this.referenceMesh, base.transform.position, base.transform.rotation) as GameObject;
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			Transform transform = AnimationLink.SearchRec(gameObject.transform, this.boneRoot);
			if (transform == null)
			{
				throw new Exception("Could not find root transform");
			}
			Animation animation = gameObject.GetComponent<Animation>();
			if (animation == null)
			{
				animation = gameObject.AddComponent<Animation>();
			}
			for (int i = 0; i < this.sequence.Length; i++)
			{
				animation.AddClip(this.sequence[i].clip, this.sequence[i].clip.name);
			}
			Vector3 a = Vector3.zero;
			Vector3 vector = base.transform.position;
			Vector3 b = Vector3.zero;
			for (int j = 0; j < this.sequence.Length; j++)
			{
				AnimationLink.LinkClip linkClip = this.sequence[j];
				if (linkClip == null)
				{
					endPosition = vector;
					return;
				}
				animation[linkClip.clip.name].enabled = true;
				animation[linkClip.clip.name].weight = 1f;
				for (int k = 0; k < linkClip.loopCount; k++)
				{
					animation[linkClip.clip.name].normalizedTime = 0f;
					animation.Sample();
					Vector3 vector2 = transform.position - base.transform.position;
					if (j > 0)
					{
						vector += a - vector2;
					}
					else
					{
						b = vector2;
					}
					for (int l = 0; l <= 20; l++)
					{
						float num = (float)l / 20f;
						animation[linkClip.clip.name].normalizedTime = num;
						animation.Sample();
						Vector3 item = vector + (transform.position - base.transform.position) + linkClip.velocity * num * linkClip.clip.length;
						trace.Add(item);
					}
					vector += linkClip.velocity * 1f * linkClip.clip.length;
					animation[linkClip.clip.name].normalizedTime = 1f;
					animation.Sample();
					Vector3 vector3 = transform.position - base.transform.position;
					a = vector3;
				}
				animation[linkClip.clip.name].enabled = false;
				animation[linkClip.clip.name].weight = 0f;
			}
			vector += a - b;
			UnityEngine.Object.DestroyImmediate(gameObject);
			endPosition = vector;
		}

		public override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();
			List<Vector3> list = ListPool<Vector3>.Claim();
			Vector3 zero = Vector3.zero;
			this.CalculateOffsets(list, out zero);
			Gizmos.color = Color.blue;
			for (int i = 0; i < list.Count - 1; i++)
			{
				Gizmos.DrawLine(list[i], list[i + 1]);
			}
		}
	}
}

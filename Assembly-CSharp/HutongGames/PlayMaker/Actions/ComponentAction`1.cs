using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class ComponentAction<T> : FsmStateAction where T : Component
	{
		private GameObject cachedGameObject;

		private T component;

		protected Rigidbody rigidbody
		{
			get
			{
				return this.component as Rigidbody;
			}
		}

		protected Renderer renderer
		{
			get
			{
				return this.component as Renderer;
			}
		}

		protected Animation animation
		{
			get
			{
				return this.component as Animation;
			}
		}

		protected AudioSource audio
		{
			get
			{
				return this.component as AudioSource;
			}
		}

		protected Camera camera
		{
			get
			{
				return this.component as Camera;
			}
		}

		protected GUIText guiText
		{
			get
			{
				return this.component as GUIText;
			}
		}

		protected GUITexture guiTexture
		{
			get
			{
				return this.component as GUITexture;
			}
		}

		protected Light light
		{
			get
			{
				return this.component as Light;
			}
		}

		protected NetworkView networkView
		{
			get
			{
				return this.component as NetworkView;
			}
		}

		protected bool UpdateCache(GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			if (this.component == null || this.cachedGameObject != go)
			{
				this.component = go.GetComponent<T>();
				this.cachedGameObject = go;
				if (this.component == null)
				{
					this.LogWarning("Missing component: " + typeof(T).FullName + " on: " + go.name);
				}
			}
			return this.component != null;
		}
	}
}

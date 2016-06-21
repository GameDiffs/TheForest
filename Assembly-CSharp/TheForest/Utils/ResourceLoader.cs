using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheForest.Utils
{
	public class ResourceLoader : MonoBehaviour
	{
		public enum AssetTypes
		{
			Mesh,
			Texture
		}

		public ResourceLoader.AssetTypes _type;

		public string _assetPath;

		public UnityEngine.Object _target;

		private UnityEngine.Object _asset;

		private static Dictionary<string, int> InUseAssetsCounters = new Dictionary<string, int>();

		private void OnEnable()
		{
			if (ResourceLoader.InUseAssetsCounters.ContainsKey(this._assetPath))
			{
				Dictionary<string, int> inUseAssetsCounters;
				Dictionary<string, int> expr_1A = inUseAssetsCounters = ResourceLoader.InUseAssetsCounters;
				string assetPath;
				string expr_22 = assetPath = this._assetPath;
				int num = inUseAssetsCounters[assetPath];
				expr_1A[expr_22] = num + 1;
			}
			else
			{
				ResourceLoader.InUseAssetsCounters[this._assetPath] = 1;
			}
			this.AssetLoad();
		}

		private void OnDisable()
		{
			Dictionary<string, int> inUseAssetsCounters;
			Dictionary<string, int> expr_05 = inUseAssetsCounters = ResourceLoader.InUseAssetsCounters;
			string assetPath;
			string expr_0D = assetPath = this._assetPath;
			int num = inUseAssetsCounters[assetPath];
			if ((expr_05[expr_0D] = num - 1) == 0)
			{
				UnityEngine.Object asset = this._asset;
				this._asset = null;
				ResourceLoader.AssetTypes type = this._type;
				if (type == ResourceLoader.AssetTypes.Mesh)
				{
					((MeshFilter)this._target).mesh = null;
					Resources.UnloadAsset(asset);
				}
				ResourceLoader.InUseAssetsCounters.Remove(this._assetPath);
			}
		}

		private void AssetLoad()
		{
			this._asset = Resources.Load(this._assetPath);
			ResourceLoader.AssetTypes type = this._type;
			if (type != ResourceLoader.AssetTypes.Mesh)
			{
				if (type == ResourceLoader.AssetTypes.Texture)
				{
					((Material)this._target).mainTexture = (Texture)this._asset;
				}
			}
			else
			{
				((MeshFilter)this._target).mesh = (Mesh)this._asset;
			}
		}
	}
}

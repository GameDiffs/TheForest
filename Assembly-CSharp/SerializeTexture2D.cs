using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializer(typeof(Texture2D))]
public class SerializeTexture2D : SerializerExtensionBase<Texture2D>
{
	public override IEnumerable<object> Save(Texture2D target)
	{
		if (target.GetInstanceID() >= 0)
		{
			return new object[]
			{
				true,
				SaveGameManager.Instance.GetAssetId(target)
			};
		}
		return new object[]
		{
			false,
			target.anisoLevel,
			target.filterMode,
			target.format,
			target.mipMapBias,
			target.mipmapCount,
			target.name,
			target.texelSize,
			target.wrapMode,
			target.EncodeToPNG()
		};
	}

	public override object Load(object[] data, object instance)
	{
		if ((bool)data[0])
		{
			return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[1]);
		}
		Texture2D texture2D;
		if (data.Length == 10)
		{
			texture2D = new Texture2D(1, 1, (TextureFormat)((int)data[3]), (int)data[5] > 0);
			texture2D.LoadImage((byte[])data[9]);
			texture2D.anisoLevel = (int)data[1];
			texture2D.filterMode = (FilterMode)((int)data[2]);
			texture2D.mipMapBias = (float)data[4];
			texture2D.name = (string)data[6];
			texture2D.wrapMode = (TextureWrapMode)((int)data[8]);
			texture2D.Apply();
			return texture2D;
		}
		texture2D = new Texture2D((int)data[9], (int)data[4], (TextureFormat)((int)data[3]), (int)data[6] > 0);
		texture2D.anisoLevel = (int)data[1];
		texture2D.filterMode = (FilterMode)((int)data[2]);
		texture2D.mipMapBias = (float)data[5];
		texture2D.name = (string)data[7];
		texture2D.wrapMode = (TextureWrapMode)((int)data[10]);
		texture2D.SetPixels32((Color32[])data[11]);
		texture2D.Apply();
		return texture2D;
	}

	public override bool CanBeSerialized(Type targetType, object instance)
	{
		Texture2D texture2D = instance as Texture2D;
		return texture2D && (texture2D.GetInstanceID() < 0 || SaveGameManager.Instance.GetAssetId(instance as UnityEngine.Object).index != -1);
	}
}

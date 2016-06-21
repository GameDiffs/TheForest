using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public interface MB2_EditorMethodsInterface
	{
		void Clear();

		void SetReadFlags(ProgressUpdateDelegate progressInfo);

		void SetReadWriteFlag(Texture2D tx, bool isReadable, bool addToList);

		void AddTextureFormat(Texture2D tx, bool isNormalMap);

		void SaveAtlasToAssetDatabase(Texture2D atlas, ShaderTextureProperty texPropertyName, int atlasNum, Material resMat);

		void SetMaterialTextureProperty(Material target, ShaderTextureProperty texPropName, string texturePath);

		void SetNormalMap(Texture2D tx);

		bool IsNormalMap(Texture2D tx);

		string GetPlatformString();

		void SetTextureSize(Texture2D tx, int size);

		bool IsCompressed(Texture2D tx);

		void CheckBuildSettings(long estimatedAtlasSize);

		bool CheckPrefabTypes(MB_ObjsToCombineTypes prefabType, List<GameObject> gos);

		bool ValidateSkinnedMeshes(List<GameObject> mom);

		void CommitChangesToAssets();

		void Destroy(UnityEngine.Object o);
	}
}

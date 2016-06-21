using DigitalOpus.MB.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MB3_MeshBakerRoot : MonoBehaviour
{
	[HideInInspector]
	public abstract MB2_TextureBakeResults textureBakeResults
	{
		get;
		set;
	}

	public virtual List<GameObject> GetObjectsToCombine()
	{
		return null;
	}

	public static bool DoCombinedValidate(MB3_MeshBakerRoot mom, MB_ObjsToCombineTypes objToCombineType, MB2_EditorMethodsInterface editorMethods, MB2_ValidationLevel validationLevel)
	{
		if (mom.textureBakeResults == null)
		{
			Debug.LogError("Need to set Material Bake Result on " + mom);
			return false;
		}
		if (mom is MB3_MeshBakerCommon)
		{
			MB3_MeshBakerCommon mB3_MeshBakerCommon = (MB3_MeshBakerCommon)mom;
			MB3_TextureBaker textureBaker = mB3_MeshBakerCommon.GetTextureBaker();
			if (textureBaker != null && textureBaker.textureBakeResults != mom.textureBakeResults)
			{
				Debug.LogWarning("Material Bake Result on this component is not the same as the Material Bake Result on the MB3_TextureBaker.");
			}
		}
		Dictionary<int, MB_Utility.MeshAnalysisResult> dictionary = null;
		if (validationLevel == MB2_ValidationLevel.robust)
		{
			dictionary = new Dictionary<int, MB_Utility.MeshAnalysisResult>();
		}
		List<GameObject> objectsToCombine = mom.GetObjectsToCombine();
		for (int i = 0; i < objectsToCombine.Count; i++)
		{
			GameObject gameObject = objectsToCombine[i];
			if (gameObject == null)
			{
				Debug.LogError("The list of objects to combine contains a null at position." + i + " Select and use [shift] delete to remove");
				return false;
			}
			for (int j = i + 1; j < objectsToCombine.Count; j++)
			{
				if (objectsToCombine[i] == objectsToCombine[j])
				{
					Debug.LogError(string.Concat(new object[]
					{
						"The list of objects to combine contains duplicates at ",
						i,
						" and ",
						j
					}));
					return false;
				}
			}
			if (MB_Utility.GetGOMaterials(gameObject) == null)
			{
				Debug.LogError("Object " + gameObject + " in the list of objects to be combined does not have a material");
				return false;
			}
			Mesh mesh = MB_Utility.GetMesh(gameObject);
			if (mesh == null)
			{
				Debug.LogError("Object " + gameObject + " in the list of objects to be combined does not have a mesh");
				return false;
			}
			if (mesh != null && !Application.isEditor && Application.isPlaying && mom.textureBakeResults.doMultiMaterial && validationLevel >= MB2_ValidationLevel.robust)
			{
				MB_Utility.MeshAnalysisResult value;
				if (!dictionary.TryGetValue(mesh.GetInstanceID(), out value))
				{
					MB_Utility.doSubmeshesShareVertsOrTris(mesh, ref value);
					dictionary.Add(mesh.GetInstanceID(), value);
				}
				if (value.hasOverlappingSubmeshVerts)
				{
					Debug.LogWarning("Object " + objectsToCombine[i] + " in the list of objects to combine has overlapping submeshes (submeshes share vertices). If the UVs associated with the shared vertices are important then this bake may not work. If you are using multiple materials then this object can only be combined with objects that use the exact same set of textures (each atlas contains one texture). There may be other undesirable side affects as well. Mesh Master, available in the asset store can fix overlapping submeshes.");
				}
			}
		}
		if (mom is MB3_MeshBaker)
		{
			List<GameObject> objectsToCombine2 = mom.GetObjectsToCombine();
			if (objectsToCombine2 == null || objectsToCombine2.Count == 0)
			{
				Debug.LogError("No meshes to combine. Please assign some meshes to combine.");
				return false;
			}
			if (mom is MB3_MeshBaker && ((MB3_MeshBaker)mom).meshCombiner.renderType == MB_RenderType.skinnedMeshRenderer && !editorMethods.ValidateSkinnedMeshes(objectsToCombine2))
			{
				return false;
			}
		}
		if (editorMethods != null)
		{
			editorMethods.CheckPrefabTypes(objToCombineType, objectsToCombine);
		}
		return true;
	}
}

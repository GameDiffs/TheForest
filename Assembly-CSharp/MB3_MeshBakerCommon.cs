using DigitalOpus.MB.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MB3_MeshBakerCommon : MB3_MeshBakerRoot
{
	public List<GameObject> objsToMesh;

	public bool useObjsToMeshFromTexBaker = true;

	public bool clearBuffersAfterBake = true;

	public string bakeAssetsInPlaceFolderPath;

	[HideInInspector]
	public GameObject resultPrefab;

	public abstract MB3_MeshCombiner meshCombiner
	{
		get;
	}

	public override MB2_TextureBakeResults textureBakeResults
	{
		get
		{
			return this.meshCombiner.textureBakeResults;
		}
		set
		{
			this.meshCombiner.textureBakeResults = value;
		}
	}

	public override List<GameObject> GetObjectsToCombine()
	{
		if (!this.useObjsToMeshFromTexBaker)
		{
			if (this.objsToMesh == null)
			{
				this.objsToMesh = new List<GameObject>();
			}
			return this.objsToMesh;
		}
		MB3_TextureBaker component = base.gameObject.GetComponent<MB3_TextureBaker>();
		if (component == null)
		{
			component = base.gameObject.transform.parent.GetComponent<MB3_TextureBaker>();
		}
		if (component != null)
		{
			return component.GetObjectsToCombine();
		}
		Debug.LogWarning("Use Objects To Mesh From Texture Baker was checked but no texture baker");
		return new List<GameObject>();
	}

	public void EnableDisableSourceObjectRenderers(bool show)
	{
		for (int i = 0; i < this.GetObjectsToCombine().Count; i++)
		{
			GameObject gameObject = this.GetObjectsToCombine()[i];
			if (gameObject != null)
			{
				Renderer renderer = MB_Utility.GetRenderer(gameObject);
				if (renderer != null)
				{
					renderer.enabled = show;
				}
			}
		}
	}

	public virtual void ClearMesh()
	{
		this.meshCombiner.ClearMesh();
	}

	public virtual void DestroyMesh()
	{
		this.meshCombiner.DestroyMesh();
	}

	public virtual void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods)
	{
		this.meshCombiner.DestroyMeshEditor(editorMethods);
	}

	public virtual int GetNumObjectsInCombined()
	{
		return this.meshCombiner.GetNumObjectsInCombined();
	}

	public virtual int GetNumVerticesFor(GameObject go)
	{
		return this.meshCombiner.GetNumVerticesFor(go);
	}

	public MB3_TextureBaker GetTextureBaker()
	{
		MB3_TextureBaker component = base.GetComponent<MB3_TextureBaker>();
		if (component != null)
		{
			return component;
		}
		if (base.transform.parent != null)
		{
			return base.transform.parent.GetComponent<MB3_TextureBaker>();
		}
		return null;
	}

	public abstract bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true);

	public abstract bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource = true);

	public virtual void Apply(MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
	{
		this.meshCombiner.name = base.name + "-mesh";
		this.meshCombiner.Apply(uv2GenerationMethod);
	}

	public virtual void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
	{
		this.meshCombiner.name = base.name + "-mesh";
		this.meshCombiner.Apply(triangles, vertices, normals, tangents, uvs, uv2, uv3, uv4, colors, bones, uv2GenerationMethod);
	}

	public virtual bool CombinedMeshContains(GameObject go)
	{
		return this.meshCombiner.CombinedMeshContains(go);
	}

	public virtual void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true, bool updateVertices = true, bool updateNormals = true, bool updateTangents = true, bool updateUV = false, bool updateUV1 = false, bool updateUV2 = false, bool updateColors = false, bool updateSkinningInfo = false)
	{
		this.meshCombiner.name = base.name + "-mesh";
		this.meshCombiner.UpdateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV1, updateUV2, updateColors, updateSkinningInfo, false);
	}

	public virtual void UpdateSkinnedMeshApproximateBounds()
	{
		if (this._ValidateForUpdateSkinnedMeshBounds())
		{
			this.meshCombiner.UpdateSkinnedMeshApproximateBounds();
		}
	}

	public virtual void UpdateSkinnedMeshApproximateBoundsFromBones()
	{
		if (this._ValidateForUpdateSkinnedMeshBounds())
		{
			this.meshCombiner.UpdateSkinnedMeshApproximateBoundsFromBones();
		}
	}

	public virtual void UpdateSkinnedMeshApproximateBoundsFromBounds()
	{
		if (this._ValidateForUpdateSkinnedMeshBounds())
		{
			this.meshCombiner.UpdateSkinnedMeshApproximateBoundsFromBounds();
		}
	}

	protected virtual bool _ValidateForUpdateSkinnedMeshBounds()
	{
		if (this.meshCombiner.outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
		{
			Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
			return false;
		}
		if (this.meshCombiner.resultSceneObject == null)
		{
			Debug.LogWarning("Result Scene Object does not exist. No point in calling UpdateSkinnedMeshApproximateBounds.");
			return false;
		}
		SkinnedMeshRenderer componentInChildren = this.meshCombiner.resultSceneObject.GetComponentInChildren<SkinnedMeshRenderer>();
		if (componentInChildren == null)
		{
			Debug.LogWarning("No SkinnedMeshRenderer on result scene object.");
			return false;
		}
		return true;
	}
}

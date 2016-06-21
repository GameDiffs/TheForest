using System;
using System.Collections.Generic;
using UnityEngine;

namespace U_r_g_utils
{
	public static class clsurgutils
	{
		public enum enumparttypes
		{
			head,
			spine,
			arm_left,
			arm_right,
			leg_left,
			leg_right
		}

		public static void metsummaryplaceholder()
		{
		}

		public static int metcrossfadetransitionanimation(Transform varpcharacter, string varpdestinationanimname, float varptransitiontime, Transform varpcontroller = null, string varpstateanimationname = "", bool varpgokinematic = true, string varpnewanimname = "transition", Animation varpanimationsystem = null, SkinnedMeshRenderer varprenderer = null, clsurganimationstatesmanager varpstatesmanager = null)
		{
			if (varpcharacter == null)
			{
				return -1;
			}
			if (varptransitiontime == 0f)
			{
				return -2;
			}
			if (varpanimationsystem == null)
			{
				varpanimationsystem = varpcharacter.root.GetComponentInChildren<Animation>();
				if (varpanimationsystem == null)
				{
					return -3;
				}
				varpanimationsystem.Stop();
			}
			if (varprenderer == null)
			{
				varprenderer = varpcharacter.root.GetComponentInChildren<SkinnedMeshRenderer>();
				if (varprenderer == null)
				{
					return -4;
				}
			}
			if (varpgokinematic)
			{
				clsurgutils.metgodriven(varpcharacter, true);
			}
			Vector3 localPosition = default(Vector3);
			if (varpcontroller != null)
			{
				Vector3 position = varpcharacter.position;
				varpcontroller.position = position;
				varpcharacter.localPosition = localPosition;
			}
			AnimationClip animationClip = clsurgutils.metcreatetransitionanimation(varpcharacter, varpdestinationanimname, varptransitiontime, varpnewanimname, ref localPosition, varpstatesmanager, false);
			if (animationClip != null)
			{
				varpanimationsystem.Stop();
				AnimationState animationState = varpanimationsystem[varpdestinationanimname];
				if (animationState != null && animationState.name != varpdestinationanimname)
				{
					varpanimationsystem.RemoveClip(varpnewanimname);
				}
				varpanimationsystem.AddClip(animationClip, varpnewanimname);
				if (varpstateanimationname != string.Empty)
				{
					varpanimationsystem[varpnewanimname].wrapMode = WrapMode.Once;
					varpanimationsystem.CrossFade(varpnewanimname);
					varpanimationsystem.CrossFadeQueued(varpstateanimationname);
				}
				else
				{
					varpanimationsystem.CrossFade(varpnewanimname);
				}
				return 1;
			}
			Debug.LogError("Could not create transition");
			return -5;
		}

		public static AnimationClip metcreatetransitionanimation(Transform varpsource, string varptargetanimname, float varptransitiontime, string varpnewanimationname, ref Vector3 varprootnormalizer, clsurganimationstatesmanager varpanimationstatemanager = null, bool varppositioncurves = false)
		{
			if (varpanimationstatemanager == null)
			{
				varpanimationstatemanager = varpsource.root.GetComponentInChildren<clsurganimationstatesmanager>();
				if (varpanimationstatemanager == null)
				{
					Debug.Log("No animation states manager in the source");
					return null;
				}
			}
			int num = varpanimationstatemanager.vargamstatenames.IndexOf(varptargetanimname);
			if (num < 0)
			{
				Debug.LogError("Animation state not memorized in manager");
				return null;
			}
			if (varpanimationstatemanager.vargamrootname != varpsource.name)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"The animation states root is different than the source passed in. Unexpected behavior may occur [passed ",
					varpsource.name,
					" expected ",
					varpanimationstatemanager.vargamrootname,
					"]"
				}));
			}
			AnimationClip animationClip = new AnimationClip();
			AnimationCurve curve = new AnimationCurve();
			AnimationCurve curve2 = new AnimationCurve();
			AnimationCurve curve3 = new AnimationCurve();
			AnimationCurve curve4 = new AnimationCurve();
			AnimationCurve curve5 = new AnimationCurve();
			AnimationCurve curve6 = new AnimationCurve();
			AnimationCurve curve7 = new AnimationCurve();
			animationClip.name = varpnewanimationname;
			varprootnormalizer = varpanimationstatemanager.vargamrootoriginallocalposition;
			Transform[] componentsInChildren = varpsource.GetComponentsInChildren<Transform>();
			if (componentsInChildren.Length != varpanimationstatemanager.vargamanimationstates[num].propanimationstate.Length)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Source and state body parts length missmatch. Can't continue [",
					componentsInChildren.Length,
					" - ",
					varpanimationstatemanager.vargamanimationstates[num].propanimationstate.Length,
					"]"
				}));
				return null;
			}
			varptransitiontime -= varptransitiontime / 24f;
			for (int i = 0; i < varpanimationstatemanager.vargamanimationstates[num].propanimationstate.Length; i++)
			{
				Vector3 localPosition = componentsInChildren[i].localPosition;
				Vector3 propposition = varpanimationstatemanager.vargamanimationstates[num].propanimationstate[i].propposition;
				if (localPosition != propposition)
				{
					curve = AnimationCurve.EaseInOut(0f, localPosition.x, varptransitiontime, propposition.x);
					curve2 = AnimationCurve.EaseInOut(0f, localPosition.y, varptransitiontime, propposition.y);
					curve3 = AnimationCurve.EaseInOut(0f, localPosition.z, varptransitiontime, propposition.z);
					animationClip.SetCurve(varpanimationstatemanager.vargamanimationstates[num].propanimationstate[i].proppath, typeof(Transform), "localPosition.x", curve);
					animationClip.SetCurve(varpanimationstatemanager.vargamanimationstates[num].propanimationstate[i].proppath, typeof(Transform), "localPosition.y", curve2);
					animationClip.SetCurve(varpanimationstatemanager.vargamanimationstates[num].propanimationstate[i].proppath, typeof(Transform), "localPosition.z", curve3);
				}
				Quaternion localRotation = componentsInChildren[i].localRotation;
				Quaternion proprotation = varpanimationstatemanager.vargamanimationstates[num].propanimationstate[i].proprotation;
				if (localRotation != proprotation)
				{
					curve4 = AnimationCurve.Linear(0f, localRotation.x, varptransitiontime, proprotation.x);
					curve5 = AnimationCurve.Linear(0f, localRotation.y, varptransitiontime, proprotation.y);
					curve6 = AnimationCurve.Linear(0f, localRotation.z, varptransitiontime, proprotation.z);
					curve7 = AnimationCurve.Linear(0f, localRotation.w, varptransitiontime, proprotation.w);
					animationClip.SetCurve(varpanimationstatemanager.vargamanimationstates[num].propanimationstate[i].proppath, typeof(Transform), "localRotation.x", curve4);
					animationClip.SetCurve(varpanimationstatemanager.vargamanimationstates[num].propanimationstate[i].proppath, typeof(Transform), "localRotation.y", curve5);
					animationClip.SetCurve(varpanimationstatemanager.vargamanimationstates[num].propanimationstate[i].proppath, typeof(Transform), "localRotation.z", curve6);
					animationClip.SetCurve(varpanimationstatemanager.vargamanimationstates[num].propanimationstate[i].proppath, typeof(Transform), "localRotation.w", curve7);
				}
			}
			animationClip.EnsureQuaternionContinuity();
			animationClip.wrapMode = WrapMode.ClampForever;
			return animationClip;
		}

		public static int metconnectbodies(Transform varpsource, Transform varptarget, bool varpreplace)
		{
			int result = 0;
			if (varpsource != null && varptarget != null)
			{
				Joint joint = varpsource.gameObject.GetComponent<Joint>();
				Rigidbody rigidbody = varptarget.gameObject.GetComponent<Rigidbody>();
				if (joint == null)
				{
					joint = varpsource.gameObject.AddComponent<CharacterJoint>();
				}
				if (rigidbody == null)
				{
					rigidbody = varptarget.gameObject.AddComponent<Rigidbody>();
				}
				if (varpreplace || (!varpreplace && (joint.connectedBody == null || joint.connectedBody == rigidbody)))
				{
					if (joint.connectedBody != rigidbody)
					{
						joint.connectedBody = rigidbody;
					}
					result = 1;
				}
			}
			return result;
		}

		public static void metgodriven(Transform varpsource, bool varpkinematic = false)
		{
			Rigidbody[] componentsInChildren = varpsource.GetComponentsInChildren<Rigidbody>();
			Rigidbody[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody rigidbody = array[i];
				rigidbody.WakeUp();
				rigidbody.isKinematic = varpkinematic;
			}
		}

		public static void metgodriven(Transform varpsource, Vector3 varpvelocity)
		{
			Rigidbody[] componentsInChildren = varpsource.GetComponentsInChildren<Rigidbody>();
			Rigidbody[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody rigidbody = array[i];
				rigidbody.WakeUp();
				rigidbody.isKinematic = false;
				rigidbody.velocity = varpvelocity;
			}
			if (varpsource.GetComponent<Rigidbody>() != null)
			{
				varpsource.GetComponent<Rigidbody>().WakeUp();
				varpsource.GetComponent<Rigidbody>().isKinematic = false;
				varpsource.GetComponent<Rigidbody>().velocity = varpvelocity;
			}
		}

		public static void metgotangible(Transform varpsource, bool varptangible = false)
		{
			Collider[] componentsInChildren = varpsource.gameObject.GetComponentsInChildren<Collider>();
			Collider[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Collider collider = array[i];
				collider.enabled = varptangible;
			}
		}

		public static void metdriveurgent(clsurgent varpsource, clsurgentactuator varpactuator = null)
		{
			if (varpsource == null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Received a null parameter: ",
					varpsource,
					" - ",
					varpactuator
				}));
				return;
			}
			if (varpactuator == null || varpactuator.transform == varpsource.vargamnodes.vargamspine[0])
			{
				clsurgutils.metdrivebodypart(varpsource, clsurgutils.enumparttypes.head, 0);
				clsurgutils.metdrivebodypart(varpsource, clsurgutils.enumparttypes.arm_left, 0);
				clsurgutils.metdrivebodypart(varpsource, clsurgutils.enumparttypes.arm_right, 0);
				clsurgutils.metdrivebodypart(varpsource, clsurgutils.enumparttypes.leg_left, 0);
				clsurgutils.metdrivebodypart(varpsource, clsurgutils.enumparttypes.leg_right, 0);
				clsurgutils.metdrivebodypart(varpsource, clsurgutils.enumparttypes.spine, 0);
			}
			else
			{
				clsurgutils.metdrivebodypart(varpsource, varpactuator.vargamparttype, varpactuator.vargampartindex);
			}
		}

		public static void metdrivebodypart(clsurgent varpsource, clsurgutils.enumparttypes varppart, int varppartindex)
		{
			clsurgutils.metdriveanimatebodypart(varpsource, varppart, varppartindex, false);
		}

		public static void metdriveanimatebodypart(clsurgent varpsource, clsurgutils.enumparttypes varppart, int varppartindex, bool varpanimate)
		{
			if (varpsource != null)
			{
				Transform[] array = new Transform[0];
				switch (varppart)
				{
				case clsurgutils.enumparttypes.head:
					array = varpsource.vargamnodes.vargamhead;
					break;
				case clsurgutils.enumparttypes.spine:
					array = varpsource.vargamnodes.vargamspine;
					break;
				case clsurgutils.enumparttypes.arm_left:
					array = varpsource.vargamnodes.vargamarmleft;
					break;
				case clsurgutils.enumparttypes.arm_right:
					array = varpsource.vargamnodes.vargamarmright;
					break;
				case clsurgutils.enumparttypes.leg_left:
					array = varpsource.vargamnodes.vargamlegleft;
					break;
				case clsurgutils.enumparttypes.leg_right:
					array = varpsource.vargamnodes.vargamlegright;
					break;
				default:
					Debug.LogError("Unmanaged part type");
					break;
				}
				for (int i = varppartindex; i < array.Length; i++)
				{
					if (array[i] != null && array[i].GetComponent<Rigidbody>() != null)
					{
						array[i].GetComponent<Rigidbody>().isKinematic = varpanimate;
					}
				}
			}
			else
			{
				Debug.LogError("Received a request to URG drive a null source");
			}
		}

		public static string metbuildpartpath(Transform varpsource)
		{
			Transform transform = varpsource;
			string text = string.Empty;
			string text2 = string.Empty;
			while (transform != null && transform.parent != null)
			{
				text = transform.name + text2 + text;
				transform = transform.parent;
				if (text2 == string.Empty)
				{
					text2 = "/";
				}
			}
			return text;
		}

		private static bool metsetsegmentplaneintersection(Plane varpplane, Vector3 varpplaneposition, Vector3 varpsegmentpoint1, Vector3 varpsegmentpoint2, ref Vector3 varpintersection)
		{
			float distanceToPoint = varpplane.GetDistanceToPoint(varpsegmentpoint1);
			float distanceToPoint2 = varpplane.GetDistanceToPoint(varpsegmentpoint2);
			if (distanceToPoint * distanceToPoint2 > 0f)
			{
				return false;
			}
			Vector3 vector = varpsegmentpoint2 - varpsegmentpoint1;
			float d = Vector3.Dot(varpplane.normal, varpplaneposition - varpsegmentpoint1) / Vector3.Dot(varpplane.normal, vector);
			varpintersection = varpsegmentpoint1 + d * vector;
			return true;
		}

		private static float metthreepointangle(Vector3 varpa, Vector3 varpb, Vector3 varpc)
		{
			Vector2 vector;
			vector.x = varpb.x - varpa.x;
			vector.y = varpb.y - varpa.y;
			Vector2 vector2;
			vector2.x = varpb.x - varpc.x;
			vector2.y = varpb.y - varpc.y;
			float num = Mathf.Atan2(vector.y, vector.x);
			float num2 = Mathf.Atan2(vector2.y, vector2.x);
			float num3 = num - num2;
			float num4 = num3 * 180f / 3.14159274f;
			return (num4 <= 0f) ? (-num4) : (360f - num4);
		}

		public static void metcreateplaceholder(float varpradius, Color varpcolor, Vector3 varpposition, string varpname = "Placeholder", Transform varpparent = null)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject.name = varpname;
			gameObject.transform.position = varpposition;
			gameObject.transform.localScale = Vector3.one * varpradius;
			Material material = new Material(Shader.Find("Diffuse"));
			material.color = varpcolor;
			gameObject.GetComponent<Renderer>().material = material;
			if (varpparent != null)
			{
				gameObject.transform.parent = varpparent;
			}
		}

		public static void metcreatetriangle(Color varpcolor, Vector3 varpp1, Vector3 varpp2, Vector3 varpp3, string varpname = "Placeholder", bool varpdouble = true, Transform varpparent = null)
		{
			GameObject gameObject = new GameObject(varpname);
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[]
			{
				varpp1,
				varpp2,
				varpp3
			};
			mesh.uv = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f)
			};
			mesh.normals = new Vector3[]
			{
				Vector3.up,
				Vector3.up,
				Vector3.up
			};
			if (varpdouble)
			{
				mesh.triangles = new int[]
				{
					0,
					1,
					2,
					0,
					2,
					1
				};
			}
			else
			{
				mesh.triangles = new int[]
				{
					0,
					1,
					2
				};
			}
			meshFilter.sharedMesh = mesh;
			gameObject.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Self-Illumin/Diffuse"));
			gameObject.GetComponent<Renderer>().sharedMaterial.color = varpcolor;
			if (varpparent != null)
			{
				gameObject.transform.parent = varpparent;
			}
		}

		public static GameObject metcreateplane(string varpname, float varpwidth = 1f, float varpheight = 1f)
		{
			GameObject gameObject = new GameObject(varpname);
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
			Mesh mesh = meshFilter.mesh;
			mesh.Clear();
			float num = varpwidth / 2f;
			float num2 = varpheight / 2f;
			mesh.vertices = new Vector3[]
			{
				new Vector3(num, num2, 0f),
				new Vector3(num, -num2, 0f),
				new Vector3(-num, -num2, 0f),
				new Vector3(-num, num2, 0f)
			};
			mesh.uv = new Vector2[]
			{
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f),
				new Vector2(0f, 1f)
			};
			mesh.normals = new Vector3[]
			{
				new Vector3(0f, -1f, 0f),
				new Vector3(0f, -1f, 0f),
				new Vector3(0f, -1f, 0f),
				new Vector3(0f, -1f, 0f)
			};
			mesh.triangles = new int[]
			{
				2,
				1,
				0,
				0,
				3,
				2
			};
			gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));
			gameObject.GetComponent<Renderer>().material.color = Color.red;
			return gameObject;
		}

		public static bool metdismember(Transform varpbonetodetach, Material varpstumpmaterial, clsdismemberator varpdismemberator = null, GameObject varpparentparticle = null, GameObject varpchildparticle = null, bool varpcinematiccut = true, bool varplastcut = true)
		{
			bool result = true;
			Transform x = clsurgutils.metdismemberpart(varpbonetodetach, varpstumpmaterial, varpdismemberator, varpparentparticle, varpchildparticle, varpcinematiccut, true);
			if (x == null)
			{
				result = false;
			}
			return result;
		}

		public static Transform metdismemberpart(Transform varpbonetodetach, Material varpstumpmaterial, clsdismemberator varpdismemberator = null, GameObject varpparentparticle = null, GameObject varpchildparticle = null, bool varpcinematiccut = true, bool varplastcut = true)
		{
			if (varpbonetodetach == null)
			{
				Debug.LogError("Null body part. Aborting.");
				return null;
			}
			if (varpdismemberator == null)
			{
				varpdismemberator = varpbonetodetach.root.GetComponentInChildren<clsdismemberator>();
			}
			if (varpdismemberator == null)
			{
				Debug.LogError("Null DM. Aborting.");
				return null;
			}
			int num = varpdismemberator.vargamboneindexes.IndexOf(varpbonetodetach);
			if (num < 0)
			{
				Debug.LogError("Bone not indexed. Aborting.");
				return null;
			}
			if (num == 0)
			{
				Debug.LogWarning("Can't separate the ragdoll root bone. Aborting.");
				return null;
			}
			int num2 = varpdismemberator.vargamboneindexes.IndexOf(varpbonetodetach.parent);
			if (num2 < 0)
			{
				return null;
			}
			if (varpdismemberator.vargamcutparts[num])
			{
				return null;
			}
			bool flag = false;
			if (varpstumpmaterial != null)
			{
				flag = true;
			}
			varpdismemberator.vargamcutparts[num] = true;
			varpdismemberator.vargamparallelcutcounter++;
			int num3 = varpdismemberator.vargambonerelationsindexes[num].propchildrenside.propindexes.Length;
			for (int i = 0; i < num3; i++)
			{
				varpdismemberator.vargamcutparts[varpdismemberator.vargambonerelationsindexes[num].propchildrenside.propindexes[i]] = true;
			}
			GameObject gameObject = new GameObject(varpbonetodetach.name + "_stump");
			GameObject gameObject2 = null;
			Transform transform = gameObject.transform;
			Transform transform2 = null;
			SkinnedMeshRenderer vargamskinnedmeshrenderer = varpdismemberator.vargamskinnedmeshrenderer;
			SkinnedMeshRenderer skinnedMeshRenderer = null;
			Mesh sharedMesh = vargamskinnedmeshrenderer.sharedMesh;
			Transform[] bones = vargamskinnedmeshrenderer.bones;
			Mesh mesh = new Mesh();
			Mesh mesh2 = null;
			Vector3[] vertices = sharedMesh.vertices;
			Vector3[] normals = sharedMesh.normals;
			Vector4[] tangents = sharedMesh.tangents;
			Vector2[] uv = sharedMesh.uv;
			Vector2[] uv2 = sharedMesh.uv2;
			BoneWeight[] boneWeights = sharedMesh.boneWeights;
			int[] triangles = sharedMesh.triangles;
			Matrix4x4[] bindposes = sharedMesh.bindposes;
			Material[] materials = vargamskinnedmeshrenderer.materials;
			Transform[] array = new Transform[bones.Length];
			bones.CopyTo(array, 0);
			Matrix4x4[] array2 = new Matrix4x4[bindposes.Length];
			bindposes.CopyTo(array2, 0);
			gameObject.layer = bones[num2].gameObject.layer;
			transform.position = varpbonetodetach.position;
			transform.rotation = varpbonetodetach.rotation;
			transform.localScale = varpbonetodetach.lossyScale;
			int[] propindexes = varpdismemberator.vargambonerelationsindexes[num].propchildrenside.propindexes;
			int num4 = propindexes.Length;
			List<int> list = new List<int>();
			int j = -1;
			while (j < num4)
			{
				GameObject gameObject3 = new GameObject(varpbonetodetach.name + "_clone");
				Transform transform3 = gameObject3.transform;
				CharacterJoint characterJoint = null;
				int num5;
				Vector3 localScale;
				if (j == -1)
				{
					num5 = num;
					gameObject2 = gameObject3;
					transform2 = transform3;
					skinnedMeshRenderer = gameObject3.AddComponent<SkinnedMeshRenderer>();
					mesh2 = new Mesh();
					localScale = bones[num5].lossyScale;
					goto IL_313;
				}
				num5 = propindexes[j];
				if (num5 != 0)
				{
					int num6 = varpdismemberator.vargamboneindexesparents[num5];
					transform3.parent = array[num6];
					characterJoint = bones[num5].GetComponent<CharacterJoint>();
					localScale = bones[num5].localScale;
					goto IL_313;
				}
				Debug.LogError("Interrupting " + varpbonetodetach.name + " cut since it was not compiled properly.This normally leads to a broken cut.\nPlease fix the model and rerun the dismemberator tool on it.");
				IL_72F:
				j++;
				continue;
				IL_313:
				clsurgentactuator component = bones[num5].GetComponent<clsurgentactuator>();
				if (component != null)
				{
					component.vargamactuatorenabled = false;
				}
				gameObject3.name = bones[num5].name;
				gameObject3.layer = bones[num5].gameObject.layer;
				transform3.position = bones[num5].position;
				transform3.rotation = bones[num5].rotation;
				transform3.localScale = localScale;
				Rigidbody component2 = bones[num5].GetComponent<Rigidbody>();
				if (component2 != null)
				{
					Rigidbody rigidbody = gameObject3.AddComponent<Rigidbody>();
					rigidbody.drag = 1f;
					rigidbody.angularDrag = 1f;
					rigidbody.mass = component2.mass / 2f;
					rigidbody.isKinematic = true;
					rigidbody.constraints = component2.constraints;
				}
				Collider component3 = bones[num5].GetComponent<Collider>();
				if (component3 != null)
				{
					Type type = component3.GetType();
					string name = type.Name;
					if (name == typeof(SphereCollider).Name)
					{
						SphereCollider component4 = bones[num5].GetComponent<SphereCollider>();
						SphereCollider sphereCollider = gameObject3.AddComponent<SphereCollider>();
						sphereCollider.radius = component4.radius;
						sphereCollider.center = component4.center;
						component4.enabled = false;
					}
					else if (name == typeof(CapsuleCollider).Name)
					{
						CapsuleCollider component5 = bones[num5].GetComponent<CapsuleCollider>();
						CapsuleCollider capsuleCollider = gameObject3.AddComponent<CapsuleCollider>();
						capsuleCollider.height = component5.height;
						capsuleCollider.direction = component5.direction;
						capsuleCollider.radius = component5.radius;
						capsuleCollider.center = component5.center;
						component5.enabled = false;
					}
					else if (name == typeof(BoxCollider).Name)
					{
						BoxCollider component6 = bones[num5].GetComponent<BoxCollider>();
						BoxCollider boxCollider = gameObject3.AddComponent<BoxCollider>();
						boxCollider.center = component6.center;
						boxCollider.size = component6.size;
						component6.enabled = false;
					}
					else if (name == typeof(MeshCollider).Name)
					{
						MeshCollider component7 = bones[num5].GetComponent<MeshCollider>();
						MeshCollider meshCollider = gameObject3.AddComponent<MeshCollider>();
						meshCollider.sharedMesh = component7.sharedMesh;
						meshCollider.convex = component7.convex;
						component7.enabled = false;
					}
					else if (name == typeof(WheelCollider).Name)
					{
						WheelCollider component8 = bones[num5].GetComponent<WheelCollider>();
						BoxCollider boxCollider2 = gameObject3.AddComponent<BoxCollider>();
						boxCollider2.center = component8.center;
						boxCollider2.size = new Vector3(component8.radius / 3f, component8.radius * 2f, component8.radius * 2f);
						if (component2 == null)
						{
							Rigidbody rigidbody = gameObject3.AddComponent<Rigidbody>();
							rigidbody.mass = component8.mass;
							rigidbody.isKinematic = true;
						}
						component8.enabled = false;
					}
					else
					{
						if (name == typeof(Collider).Name)
						{
							Debug.LogError("Missing collider on cut.");
							return null;
						}
						Debug.LogError("Can't manage collider type " + bones[num5].GetComponent<Collider>().GetType().ToString());
						return null;
					}
				}
				if (characterJoint != null)
				{
					if (varpdismemberator.vargamboneindexescharacterjointconnect[num5] > -1)
					{
						characterJoint = gameObject3.AddComponent<CharacterJoint>();
						Rigidbody component9 = array[varpdismemberator.vargamboneindexescharacterjointconnect[num5]].GetComponent<Rigidbody>();
						if (component9 == null)
						{
							characterJoint.connectedBody = array[num].GetComponent<Rigidbody>();
						}
						else
						{
							characterJoint.connectedBody = array[varpdismemberator.vargamboneindexescharacterjointconnect[num5]].GetComponent<Rigidbody>();
						}
					}
					characterJoint.axis = Vector3.forward;
				}
				varpdismemberator.vargamcutpartscache.Add(bones[num5]);
				array[num5] = transform3;
				goto IL_72F;
			}
			int num7 = varpdismemberator.vargamrigidbodyconnections[num].propindexes.Length;
			if (num7 > 1)
			{
				int num8 = varpdismemberator.vargamrigidbodyconnections[num].propindexes[0];
				if (num8 > -1)
				{
					Rigidbody component2 = bones[num8].GetComponent<Rigidbody>();
					if (component2 != null)
					{
						for (int k = 1; k < varpdismemberator.vargamrigidbodyconnections[num].propindexes.Length; k++)
						{
							num8 = varpdismemberator.vargamrigidbodyconnections[num].propindexes[k];
							if (bones[num8] != null)
							{
								CharacterJoint characterJoint = bones[num8].GetComponent<CharacterJoint>();
								if (characterJoint != null)
								{
									characterJoint.connectedBody = component2;
									characterJoint.axis = Vector3.forward;
								}
							}
						}
					}
				}
			}
			int num9 = varpdismemberator.vargamboneseparationvertices[num].propindexes.Length;
			int num10 = 0;
			int[] array3 = new int[0];
			int[] array4 = new int[0];
			if (num9 > 0)
			{
				num10 = (num9 - 2) * 3;
			}
			int num11 = triangles.Length;
			int num12 = varpdismemberator.vargambonefulltrianglesindexes[num].propindexes.Length * 3;
			array3 = new int[num11 + num10];
			array4 = new int[num12 + num10];
			triangles.CopyTo(array3, 0);
			for (int l = 0; l < varpdismemberator.vargambonefulltrianglesindexes[num].propindexes.Length; l++)
			{
				int num13 = varpdismemberator.vargambonefulltrianglesindexes[num].propindexes[l];
				int num14 = num13 + 1;
				int num15 = num13 + 2;
				array3[num13] = 0;
				array3[num14] = 0;
				array3[num15] = 0;
				int num16 = l * 3;
				array4[num16] = triangles[num13];
				array4[num16 + 1] = triangles[num14];
				array4[num16 + 2] = triangles[num15];
			}
			int num17 = materials.Length;
			int num18 = -1;
			Material[] array5 = new Material[0];
			bool flag2 = false;
			int[] array6 = new int[0];
			if (varpstumpmaterial != null)
			{
				for (int m = 0; m < num17; m++)
				{
					if (materials[m].name == varpstumpmaterial.name || materials[m].name == varpstumpmaterial.name + " (Instance)")
					{
						num18 = m;
						array6 = sharedMesh.GetTriangles(num18);
						break;
					}
				}
				if (num18 < 0)
				{
					array5 = new Material[num17 + 1];
					materials.CopyTo(array5, 0);
					array5[num17] = varpstumpmaterial;
					num18 = num17;
					flag2 = true;
				}
				else
				{
					array5 = materials;
				}
			}
			else
			{
				array5 = materials;
			}
			Material[] array7 = new Material[array5.Length];
			array5.CopyTo(array7, 0);
			int[] propindexes2 = varpdismemberator.vargambonerelationsindexes[num].propparentside.propindexes;
			for (int n = 0; n < propindexes2.Length; n++)
			{
				int num19 = propindexes2[n];
				array[num19] = transform2;
				array2[num19] = bindposes[num];
			}
			int num20 = varpdismemberator.vargamboneseparationsubmeshhelper.Length * 3;
			propindexes2 = varpdismemberator.vargambonerelationsindexes[num].propchildrenside.propindexes;
			for (int num21 = 0; num21 < propindexes2.Length; num21++)
			{
				int num19 = propindexes2[num21];
				bones[num19] = transform;
				bindposes[num19] = bindposes[num];
				int num22 = varpdismemberator.vargamboneseparationpatchtriangleindexes[num19].propindexes.Length;
				if (num22 > 0)
				{
					int[] array8 = new int[num22 * 3];
					int num23 = 0;
					for (int num24 = 0; num24 < num22; num24++)
					{
						int num25 = varpdismemberator.vargamboneseparationpatchtriangleindexes[num19].propindexes[num24];
						array8[num23] = array6[num25];
						array8[num23 + 1] = array6[num25 + 1];
						array8[num23 + 2] = array6[num25 + 2];
						array6[num25] = 0;
						array6[num25 + 1] = 0;
						array6[num25 + 2] = 0;
						num23 += 3;
					}
					list.AddRange(array8);
					varpdismemberator.vargamboneseparationpatchtriangleindexes[num19].propindexes = new int[0];
				}
			}
			gameObject.transform.parent = bones[num2];
			int[] array9 = new int[0];
			int[] array10 = new int[0];
			if (flag)
			{
				int num26 = varpdismemberator.vargamboneseparationvertices[num].propindexes.Length;
				int num27 = vertices.Length;
				int num28 = num26;
				int num29 = num27 + num28;
				Vector3 vector = varpdismemberator.vargamoriginalbonepositions[num] - varpdismemberator.vargamoriginalbonepositions[num2];
				Vector3 vector2 = varpdismemberator.vargamoriginalbonepositions[num2] - varpdismemberator.vargamoriginalbonepositions[num];
				Vector3[] array11 = new Vector3[num29];
				Vector2[] array12 = new Vector2[num29];
				Vector3[] array13 = new Vector3[num29];
				Vector4[] array14 = new Vector4[num29];
				Vector3[] array15 = new Vector3[num29];
				BoneWeight[] array16 = new BoneWeight[num29];
				int[] array17 = new int[num26];
				vertices.CopyTo(array11, 0);
				uv.CopyTo(array12, 0);
				normals.CopyTo(array13, 0);
				normals.CopyTo(array15, 0);
				tangents.CopyTo(array14, 0);
				boneWeights.CopyTo(array16, 0);
				for (int num30 = 0; num30 < num26; num30++)
				{
					int num31 = varpdismemberator.vargamboneseparationvertices[num].propindexes[num30];
					int num32 = num30 + num27;
					array11[num32] = vertices[num31];
					array16[num32] = boneWeights[num31];
					array13[num32] = vector;
					array14[num32] = tangents[num31];
					array15[num32] = vector2;
					array17[num30] = num32;
				}
				varpdismemberator.vargamboneseparationverticesuvhelper[num].propuvcoordinates.CopyTo(array12, num27);
				array9 = new int[num10];
				array10 = new int[num10];
				if (num26 > 0)
				{
					int num33 = array17[0];
					int num34 = 0;
					for (int num35 = 2; num35 < num9; num35++)
					{
						int num36 = num35 - 2 + num34;
						int num37 = num35 - 1 + num34;
						int num38 = num35 + num34;
						int num39 = array17[num35 - 1];
						int num40 = array17[num35];
						array10[num36] = num33;
						array10[num37] = num39;
						array10[num38] = num40;
						array9[num36] = num33;
						array9[num37] = num40;
						array9[num38] = num39;
						num34 += 2;
					}
				}
				array10.CopyTo(array3, num11);
				array9.CopyTo(array4, num12);
				int[] destinationArray = new int[num20];
				Array.Copy(array3, destinationArray, num20);
				bones[num] = gameObject.transform;
				mesh.vertices = array11;
				mesh.normals = array13;
				mesh.tangents = array14;
				mesh.uv = array12;
				mesh.boneWeights = array16;
				mesh.triangles = array3;
				mesh.bindposes = bindposes;
				mesh2.vertices = array11;
				mesh2.normals = array15;
				mesh2.tangents = array14;
				mesh2.uv = array12;
				mesh2.boneWeights = array16;
				mesh2.triangles = array4;
				mesh2.bindposes = array2;
			}
			else
			{
				bones[num] = gameObject.transform;
				mesh.vertices = vertices;
				mesh.normals = normals;
				mesh.tangents = tangents;
				mesh.uv = uv;
				mesh.uv2 = uv2;
				mesh.boneWeights = boneWeights;
				mesh.triangles = array3;
				mesh.bindposes = bindposes;
				mesh2.vertices = vertices;
				mesh2.normals = normals;
				mesh2.tangents = tangents;
				mesh2.uv = uv;
				mesh2.uv2 = uv2;
				mesh2.boneWeights = boneWeights;
				mesh2.triangles = array4;
				mesh2.bindposes = array2;
			}
			int num41 = sharedMesh.subMeshCount;
			if (flag2)
			{
				num41++;
			}
			int[][] array18 = new int[num41][];
			int[][] array19 = new int[num41][];
			for (int num42 = 0; num42 < num41; num42++)
			{
				if (num42 == num18)
				{
					array18[num42] = new int[array6.Length + array10.Length];
					array6.CopyTo(array18[num42], 0);
					array10.CopyTo(array18[num42], array6.Length);
					list.AddRange(array9);
					array19[num42] = list.ToArray();
				}
				else
				{
					array18[num42] = sharedMesh.GetTriangles(num42);
					array19[num42] = new int[array18[num42].Length];
				}
			}
			if (array3.Length > 0)
			{
				for (int num43 = 0; num43 < num20; num43 += 3)
				{
					if (array3[num43] == 0 && array3[num43 + 1] == 0)
					{
						int num44 = (int)varpdismemberator.vargamboneseparationsubmeshhelper[num43 / 3].x;
						int num45 = (int)varpdismemberator.vargamboneseparationsubmeshhelper[num43 / 3].y * 3;
						array18[num44][num45] = 0;
						array18[num44][num45 + 1] = 0;
						array18[num44][num45 + 2] = 0;
						array19[num44][num45] = triangles[num43];
						array19[num44][num45 + 1] = triangles[num43 + 1];
						array19[num44][num45 + 2] = triangles[num43 + 2];
					}
				}
			}
			mesh.subMeshCount = num41;
			mesh2.subMeshCount = num41;
			for (int num46 = 0; num46 < num41; num46++)
			{
				mesh.SetTriangles(array18[num46], num46);
				mesh2.SetTriangles(array19[num46], num46);
			}
			int num47 = array6.Length;
			varpdismemberator.vargamboneseparationpatchtriangleindexes[num].propindexes = new int[array10.Length / 3];
			for (int num48 = 0; num48 < varpdismemberator.vargamboneseparationpatchtriangleindexes[num].propindexes.Length; num48++)
			{
				varpdismemberator.vargamboneseparationpatchtriangleindexes[num].propindexes[num48] = num47;
				num47 += 3;
			}
			varpdismemberator.vargamskinnedmeshrenderer.bones = bones;
			varpdismemberator.vargamskinnedmeshrenderer.sharedMesh = mesh;
			varpdismemberator.vargamskinnedmeshrenderer.materials = array5;
			skinnedMeshRenderer.bones = array;
			skinnedMeshRenderer.sharedMesh = mesh2;
			skinnedMeshRenderer.materials = array7;
			varpdismemberator.vargamparallelcutcounter--;
			if (varpdismemberator.vargamparallelcutcounter == 0)
			{
				for (int num49 = 0; num49 < varpdismemberator.vargamcutpartscache.Count; num49++)
				{
					if (varpdismemberator.vargamcutpartscache[num49].gameObject != null)
					{
						UnityEngine.Object.Destroy(varpdismemberator.vargamcutpartscache[num49].gameObject);
					}
				}
				varpdismemberator.vargamcutpartscache.Clear();
			}
			if (varpparentparticle != null)
			{
				GameObject gameObject4 = UnityEngine.Object.Instantiate(varpparentparticle, transform.position, transform.rotation) as GameObject;
				gameObject4.transform.parent = transform;
			}
			if (varpchildparticle != null)
			{
				GameObject gameObject5 = UnityEngine.Object.Instantiate(varpchildparticle, transform2.position, Quaternion.Inverse(transform2.rotation)) as GameObject;
				gameObject5.transform.parent = transform2;
			}
			if (varpcinematiccut)
			{
				Rigidbody[] componentsInChildren = gameObject2.GetComponentsInChildren<Rigidbody>();
				for (int num50 = 0; num50 < componentsInChildren.Length; num50++)
				{
					componentsInChildren[num50].isKinematic = false;
					componentsInChildren[num50].velocity = Vector3.zero;
					componentsInChildren[num50].angularVelocity = Vector3.zero;
				}
				if (gameObject2.GetComponent<Rigidbody>() != null)
				{
					gameObject2.GetComponent<Rigidbody>().velocity = Vector3.zero;
					gameObject2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				}
			}
			return transform2;
		}

		public static bool metdismemberrigged(GameObject varptarget, Transform varpbonetodetach, Material varpstumpmaterial, Vector3 varpcontactpoint, Vector3 varpslashdirection, float varpdistance = 1f, bool varpcinematiccut = true)
		{
			float num = varpdistance * varpdistance;
			Plane varpplane = new Plane(varpslashdirection, varpcontactpoint);
			Transform[] array = new Transform[0];
			if (varptarget == null)
			{
				Debug.LogError("Need a target to be able to cut");
			}
			clsdismemberator clsdismemberator = varptarget.GetComponent<clsdismemberator>();
			if (clsdismemberator == null)
			{
				clsdismemberator = varptarget.transform.root.GetComponentInChildren<clsdismemberator>();
				if (clsdismemberator == null)
				{
					Debug.LogWarning("No dismemberator class found in target. Unable to cut.");
					return false;
				}
			}
			Transform vargamfixer = clsdismemberator.vargamfixer;
			if (vargamfixer == null)
			{
				Debug.LogError("No fixer");
				return false;
			}
			if (clsdismemberator.vargamskinnedmeshrenderer.sharedMesh == null)
			{
				Debug.LogError("No original mesh");
				return false;
			}
			array = clsdismemberator.vargamskinnedmeshrenderer.bones;
			int num2 = 0;
			int num3 = array.Length;
			if (varpbonetodetach)
			{
				num2 = clsdismemberator.vargamboneindexes.IndexOf(varpbonetodetach);
				if (num2 < 0)
				{
					Debug.LogError("No index for parameter bone");
					return false;
				}
				num3 = num2 + 1;
			}
			Vector3 position = default(Vector3);
			for (int i = num2; i < num3; i++)
			{
				if (!(array[i].parent == null))
				{
					if ((array[i].position - varpcontactpoint).sqrMagnitude <= num || !(varpbonetodetach == null))
					{
						if (!(array[i].GetComponent<Rigidbody>() == null))
						{
							if (varpbonetodetach == null)
							{
								if (!clsurgutils.metsetsegmentplaneintersection(varpplane, varpcontactpoint, array[i].position, array[i].parent.position, ref position))
								{
									goto IL_23B9;
								}
							}
							else
							{
								bool flag = array[i];
							}
							int num4 = clsdismemberator.vargamboneindexes.IndexOf(array[i].parent);
							if (num4 >= 0)
							{
								Debug.LogWarning(string.Concat(new object[]
								{
									"Good candidate n.",
									i,
									" [",
									array[i].name,
									" parent ",
									array[i].parent.name,
									"] parent bone n.",
									num4,
									" [",
									array[num4].name,
									"]"
								}));
								if (array[i].position == array[i].parent.position)
								{
									Debug.LogWarning("Skipped candidate [" + array[i].name + "] because it shared position with its parent");
								}
								else
								{
									Mesh sharedMesh = clsdismemberator.vargamskinnedmeshrenderer.sharedMesh;
									Vector3 vector = array[i].InverseTransformPoint(position);
									vector = sharedMesh.bindposes[i].inverse.MultiplyPoint3x4(vector);
									Vector3 vector2 = array[i].InverseTransformDirection(varpslashdirection);
									vector2 = sharedMesh.bindposes[i].inverse.MultiplyVector(vector2);
									GameObject gameObject = new GameObject(array[i].parent.name + "_stump");
									float mass;
									if (array[num4].GetComponent<Rigidbody>() != null)
									{
										mass = array[num4].GetComponent<Rigidbody>().mass / 2f;
										array[num4].GetComponent<Rigidbody>().mass = mass;
									}
									else
									{
										mass = array[i].GetComponent<Rigidbody>().mass / 2f;
									}
									gameObject.layer = array[num4].gameObject.layer;
									gameObject.transform.position = array[i].position;
									gameObject.transform.rotation = array[i].rotation;
									GameObject gameObject2 = new GameObject(array[i].name + "_stump");
									gameObject2.layer = array[num4].gameObject.layer;
									Rigidbody rigidbody = gameObject2.AddComponent<Rigidbody>();
									rigidbody.drag = 1f;
									rigidbody.angularDrag = 1f;
									rigidbody.mass = mass;
									gameObject2.transform.position = array[num4].position;
									gameObject2.transform.rotation = array[num4].rotation;
									gameObject2.transform.localScale = array[num4].lossyScale;
									SkinnedMeshRenderer skinnedMeshRenderer = gameObject2.AddComponent<SkinnedMeshRenderer>();
									skinnedMeshRenderer.bones = clsdismemberator.vargamskinnedmeshrenderer.bones;
									int num5 = -1;
									int num6 = clsdismemberator.vargamskinnedmeshrenderer.materials.Length;
									bool flag2 = false;
									if (varpstumpmaterial != null)
									{
										for (int j = 0; j < clsdismemberator.vargamskinnedmeshrenderer.materials.Length; j++)
										{
											if (clsdismemberator.vargamskinnedmeshrenderer.materials[j].name == varpstumpmaterial.name + " (Instance)" || clsdismemberator.vargamskinnedmeshrenderer.materials[j].name == varpstumpmaterial.name)
											{
												flag2 = true;
												num5 = j;
												break;
											}
										}
										if (!flag2)
										{
											Material[] array2 = new Material[num6 + 1];
											clsdismemberator.vargamskinnedmeshrenderer.materials.CopyTo(array2, 0);
											array2[num6] = varpstumpmaterial;
											clsdismemberator.vargamskinnedmeshrenderer.materials = array2;
										}
									}
									Type type = array[num4].GetComponent<Collider>().GetType();
									if (type == typeof(SphereCollider))
									{
										SphereCollider component = array[num4].GetComponent<SphereCollider>();
										SphereCollider sphereCollider = gameObject2.AddComponent<SphereCollider>();
										sphereCollider.radius = component.radius;
										sphereCollider.center = component.center;
									}
									else if (type == typeof(CapsuleCollider))
									{
										CapsuleCollider component2 = array[num4].GetComponent<CapsuleCollider>();
										CapsuleCollider capsuleCollider = gameObject2.AddComponent<CapsuleCollider>();
										capsuleCollider.height = component2.height;
										capsuleCollider.direction = component2.direction;
										capsuleCollider.radius = component2.radius;
										capsuleCollider.center = component2.center;
									}
									else if (type == typeof(BoxCollider))
									{
										BoxCollider component3 = array[num4].GetComponent<BoxCollider>();
										BoxCollider boxCollider = gameObject2.AddComponent<BoxCollider>();
										boxCollider.center = component3.center;
										boxCollider.size = component3.size;
									}
									else if (type == typeof(MeshCollider))
									{
										MeshCollider component4 = array[num4].GetComponent<MeshCollider>();
										MeshCollider meshCollider = gameObject2.AddComponent<MeshCollider>();
										meshCollider.sharedMesh = component4.sharedMesh;
										meshCollider.convex = component4.convex;
									}
									else if (type != typeof(Collider))
									{
										Debug.LogError("Can't manage collider type " + array[i].parent.GetComponent<Collider>().GetType().ToString());
										return false;
									}
									GameObject gameObject3 = new GameObject("plocalplane");
									gameObject3.transform.position = vector;
									gameObject3.transform.LookAt(vector + vector2);
									Vector3[] vertices = sharedMesh.vertices;
									int[] triangles = sharedMesh.triangles;
									BoneWeight[] boneWeights = sharedMesh.boneWeights;
									Vector3[] normals = sharedMesh.normals;
									Vector2[] uv = sharedMesh.uv;
									Vector4[] tangents = sharedMesh.tangents;
									float num7 = vector2.x * clsdismemberator.vargamoriginalbonepositions[i].x + vector2.y * clsdismemberator.vargamoriginalbonepositions[i].y + vector2.z * clsdismemberator.vargamoriginalbonepositions[i].z + (-vector2.x * vector.x - vector2.y * vector.y - vector2.z * vector.z);
									if (num7 < 0f)
									{
										num7 = 1f;
									}
									else
									{
										num7 = -1f;
									}
									int num8 = vertices.Length;
									int num9 = 0;
									int[] array3 = new int[0];
									int num10 = clsdismemberator.vargambonefulltrianglesindexes[num4].propindexes.Length;
									int num13;
									int num14;
									if (num5 > -1)
									{
										int[] triangles2 = sharedMesh.GetTriangles(num5);
										int num11 = triangles2.Length - 1;
										num9 = triangles2[num11];
										for (int k = 1; k < 3; k++)
										{
											if (triangles2[num11 - k] < num9)
											{
												num9 = triangles2[num11 - k];
											}
										}
										int num12 = triangles2.Length / 3;
										num13 = num10 + num12;
										array3 = new int[num13];
										clsdismemberator.vargambonefulltrianglesindexes[num4].propindexes.CopyTo(array3, 0);
										num14 = triangles.Length - triangles2.Length;
										for (int l = 0; l < num12; l++)
										{
											array3[num10 + l] = num14;
											num14 += 3;
										}
										num14 = triangles.Length - triangles2.Length;
									}
									else
									{
										num13 = num10;
										array3 = new int[num13];
										clsdismemberator.vargambonefulltrianglesindexes[num4].propindexes.CopyTo(array3, 0);
										num14 = triangles.Length;
									}
									int[] array4 = new int[triangles.Length];
									List<int> list = new List<int>();
									triangles.CopyTo(array4, 0);
									List<int> list2 = new List<int>();
									List<int> list3 = new List<int>();
									if (varpbonetodetach)
									{
										int num15 = clsdismemberator.vargambonefulltrianglesindexes[i].propindexes.Length;
										int[] array5 = new int[3];
										array3 = new int[num15];
										clsdismemberator.vargambonefulltrianglesindexes[i].propindexes.CopyTo(array3, 0);
										for (int m = 0; m < num15; m++)
										{
											int num16 = array3[m];
											int num17 = triangles[num16];
											int num18 = triangles[num16 + 1];
											int num19 = triangles[num16 + 2];
											array4[num16] = 0;
											array4[num16 + 1] = 0;
											array4[num16 + 2] = 0;
											array5[0] = num17;
											array5[1] = num18;
											array5[2] = num19;
											list.AddRange(array5);
										}
										list3.AddRange(clsdismemberator.vargamboneseparationvertices[i].propindexes);
										list2.AddRange(clsdismemberator.vargamboneseparationvertices[i].propindexes);
										for (int n = 0; n < list3.Count; n++)
										{
											clsurgutils.metcreateplaceholder(0.02f, Color.red, vertices[list3[n]], "xxxchildpatchloop" + n, null);
										}
									}
									else
									{
										int num20 = 0;
										for (int num21 = 0; num21 < num13; num21++)
										{
											int num16 = array3[num21];
											int num17 = triangles[num16];
											int num18 = triangles[num16 + 1];
											int num19 = triangles[num16 + 2];
											float num22 = (vector2.x * vertices[num17].x + vector2.y * vertices[num17].y + vector2.z * vertices[num17].z + (-vector2.x * vector.x - vector2.y * vector.y - vector2.z * vector.z)) * num7;
											float num23 = (vector2.x * vertices[num18].x + vector2.y * vertices[num18].y + vector2.z * vertices[num18].z + (-vector2.x * vector.x - vector2.y * vector.y - vector2.z * vector.z)) * num7;
											float num24 = (vector2.x * vertices[num19].x + vector2.y * vertices[num19].y + vector2.z * vertices[num19].z + (-vector2.x * vector.x - vector2.y * vector.y - vector2.z * vector.z)) * num7;
											int item = num17;
											int item2 = num18;
											int num25 = 0;
											if (num22 <= 0f || num23 <= 0f || num24 <= 0f || num16 >= num14)
											{
												if (num22 <= 0f && num23 <= 0f && num24 > 0f)
												{
													item = num17;
													item2 = num18;
													num25 = -2;
												}
												else if (num22 <= 0f && num23 > 0f && num24 <= 0f)
												{
													item = num17;
													item2 = num19;
													num25 = -3;
												}
												else if (num22 <= 0f && num23 > 0f && num24 > 0f)
												{
													item = num18;
													item2 = num19;
													num25 = 2;
												}
												else if (num22 > 0f && num23 <= 0f && num24 <= 0f)
												{
													item = num18;
													item2 = num19;
													num25 = -4;
												}
												else if (num22 > 0f && num23 <= 0f && num24 > 0f)
												{
													item = num17;
													item2 = num19;
													num25 = 3;
												}
												else if (num22 > 0f && num23 > 0f && num24 <= 0f)
												{
													item = num17;
													item2 = num18;
													num25 = 4;
												}
												else if (num22 > 0f && num23 > 0f && num24 > 0f)
												{
													num25 = 5;
												}
												if (num25 > 1)
												{
													switch (num25)
													{
													case 2:
														list2.Add(num8);
														list2.Add(item);
														list2.Add(item2);
														break;
													case 3:
														list2.Add(item);
														list2.Add(num8);
														list2.Add(item2);
														break;
													case 4:
														list2.Add(item);
														list2.Add(item2);
														list2.Add(num8);
														break;
													case 5:
														list2.Add(num17);
														list2.Add(num18);
														list2.Add(num19);
														break;
													}
												}
												else if (num25 < -1)
												{
													int num26 = num25;
													switch (num26 + 4)
													{
													case 0:
														list3.Add(num8);
														list3.Add(item);
														list3.Add(item2);
														break;
													case 1:
														list3.Add(item);
														list3.Add(num8);
														list3.Add(item2);
														break;
													case 2:
														list3.Add(item);
														list3.Add(item2);
														list3.Add(num8);
														break;
													}
												}
												else if (num25 == 0)
												{
													list.AddRange(new int[]
													{
														num17,
														num18,
														num19
													});
												}
												array4[num16] = 0;
												array4[num16 + 1] = 0;
												array4[num16 + 2] = 0;
												num20++;
											}
										}
									}
									if (skinnedMeshRenderer.bones[i].GetComponent<Rigidbody>())
									{
										skinnedMeshRenderer.bones[i].GetComponent<Rigidbody>().drag = 1f;
										skinnedMeshRenderer.bones[i].GetComponent<Rigidbody>().angularDrag = 1f;
										if (rigidbody)
										{
											for (int num27 = 0; num27 < clsdismemberator.vargambonerelationsindexes[num4].propchildrenside.propindexes.Length; num27++)
											{
												int num28 = clsdismemberator.vargambonerelationsindexes[num4].propchildrenside.propindexes[num27];
												CharacterJoint component5 = array[num28].GetComponent<CharacterJoint>();
												float num29 = (varpslashdirection.x * array[num28].position.x + varpslashdirection.y * array[num28].position.y + varpslashdirection.z * array[num28].position.z + (-varpslashdirection.x * varpcontactpoint.x - varpslashdirection.y * varpcontactpoint.y - varpslashdirection.z * varpcontactpoint.z)) * num7;
												if (component5 != null && component5.connectedBody != null)
												{
													Transform transform = component5.connectedBody.transform;
													float num30 = (varpslashdirection.x * transform.transform.position.x + varpslashdirection.y * transform.transform.position.y + varpslashdirection.z * transform.transform.position.z + (-varpslashdirection.x * varpcontactpoint.x - varpslashdirection.y * varpcontactpoint.y - varpslashdirection.z * varpcontactpoint.z)) * num7;
													if (num30 > 0f && num29 <= 0f)
													{
														component5.connectedBody = rigidbody;
													}
													else if (num30 <= 0f && num29 > 0f)
													{
														component5.connectedBody = array[num4].GetComponent<Rigidbody>();
													}
												}
												if (array[num28].parent != null && num29 <= 0f)
												{
													if (array[num28].parent == array[num4])
													{
														array[num28].parent = gameObject2.transform;
													}
													else
													{
														num29 = (varpslashdirection.x * array[num28].parent.position.x + varpslashdirection.y * array[num28].parent.position.y + varpslashdirection.z * array[num28].parent.position.z + (-varpslashdirection.x * varpcontactpoint.x - varpslashdirection.y * varpcontactpoint.y - varpslashdirection.z * varpcontactpoint.z)) * num7;
														if (num29 > 0f)
														{
															array[num28].parent = gameObject2.transform;
														}
													}
												}
											}
										}
									}
									Transform[] array6 = new Transform[0];
									Transform[] array7 = new Transform[0];
									List<Transform> list4 = new List<Transform>();
									List<Transform> list5 = new List<Transform>();
									for (int num31 = 0; num31 < clsdismemberator.vargambonerelationsindexes[num4].propchildrenside.propindexes.Length; num31++)
									{
										int num32 = clsdismemberator.vargambonerelationsindexes[num4].propchildrenside.propindexes[num31];
										float num33 = (varpslashdirection.x * array[num32].position.x + varpslashdirection.y * array[num32].position.y + varpslashdirection.z * array[num32].position.z + (-varpslashdirection.x * varpcontactpoint.x - varpslashdirection.y * varpcontactpoint.y - varpslashdirection.z * varpcontactpoint.z)) * num7;
										if (num33 < 0f)
										{
											list4.Add(array[num32]);
										}
										else
										{
											list5.Add(array[num32]);
										}
									}
									if (list4.Count > 0)
									{
										array6 = new Transform[list4.Count];
										list4.CopyTo(array6);
									}
									if (list5.Count > 0)
									{
										array7 = new Transform[list5.Count];
										list5.CopyTo(array7);
									}
									Mesh mesh = new Mesh();
									Mesh mesh2 = new Mesh();
									if (varpstumpmaterial != null)
									{
										HashSet<int> hashSet = new HashSet<int>();
										if (num9 == 0)
										{
											hashSet.UnionWith(list2);
										}
										else
										{
											for (int num34 = 0; num34 < list2.Count; num34++)
											{
												if (list2[num34] < num9 || list2[num34] == num8)
												{
													hashSet.Add(list2[num34]);
												}
											}
										}
										HashSet<int> hashSet2 = new HashSet<int>();
										hashSet2.UnionWith(list3);
										int num35 = vertices.Length + hashSet.Count + 1;
										int num36 = vertices.Length + hashSet2.Count + 1;
										Vector3[] array8 = new Vector3[num35];
										Vector3[] array9 = new Vector3[num36];
										vertices.CopyTo(array8, 0);
										vertices.CopyTo(array9, 0);
										array8[num8] = vector;
										array9[num8] = vector;
										int[] array10 = new int[list.Count];
										list.CopyTo(array10, 0);
										Vector3 vector3 = Vector3.zero;
										vector3 = vector;
										vector3.y += 1f;
										vector3 = Vector3.up;
										float num37 = (float)((hashSet.Count <= 0) ? 0 : (360 / hashSet.Count));
										float x = 0f;
										float y = 0f;
										Vector2[] array11 = new Vector2[num35];
										Vector2[] array12 = new Vector2[num36];
										uv.CopyTo(array11, 0);
										uv.CopyTo(array12, 0);
										List<int> list6 = new List<int>();
										array11[num8] = new Vector2(0.5f, 0.5f);
										array12[num8] = new Vector2(0.5f, 0.5f);
										Vector4[] array13 = new Vector4[num35];
										Vector4[] array14 = new Vector4[num36];
										tangents.CopyTo(array13, 0);
										tangents.CopyTo(array14, 0);
										array13[num8] = new Vector4(1f, 1f, 1f, 1f);
										array14[num8] = new Vector4(1f, 1f, 1f, 1f);
										BoneWeight[] array15 = new BoneWeight[num35];
										BoneWeight[] array16 = new BoneWeight[num36];
										boneWeights.CopyTo(array15, 0);
										boneWeights.CopyTo(array16, 0);
										array15[num8].boneIndex0 = num4;
										array15[num8].weight0 = 1f;
										array16[num8].boneIndex0 = num4;
										array16[num8].weight0 = 1f;
										Vector3[] array17 = new Vector3[num35];
										Vector3[] array18 = new Vector3[num36];
										normals.CopyTo(array17, 0);
										normals.CopyTo(array18, 0);
										Vector3 normalized = (clsdismemberator.vargamoriginalbonepositions[i] - clsdismemberator.vargamoriginalbonepositions[num4]).normalized;
										Vector3 normalized2 = (clsdismemberator.vargamoriginalbonepositions[num4] - clsdismemberator.vargamoriginalbonepositions[i]).normalized;
										array17[num8] = normalized;
										array18[num8] = normalized2;
										int[] array19 = new int[0];
										int[] array20 = new int[0];
										int num38 = 0;
										float[] array21 = new float[hashSet.Count];
										Vector2 vector5;
										Vector2 vector6;
										foreach (int current in hashSet)
										{
											if (current == num8)
											{
												array21[num38] = 0f;
												list6.Insert(0, num38);
												num38++;
											}
											else
											{
												Vector3 position2 = vertices[current];
												Vector3 vector4 = gameObject3.transform.InverseTransformPoint(position2) - Vector3.Dot(gameObject3.transform.InverseTransformPoint(position2), gameObject3.transform.InverseTransformDirection(vector2.normalized)) * gameObject3.transform.InverseTransformDirection(vector2.normalized);
												vector5.x = -vector3.x;
												vector5.y = -vector3.y;
												vector6.x = -vector4.x;
												vector6.y = -vector4.y;
												float num39 = Mathf.Atan2(vector5.y, vector5.x);
												float num40 = Mathf.Atan2(vector6.y, vector6.x);
												float num41 = num39 - num40;
												float num42 = num41 * 180f / 3.14159274f;
												float num43 = (num42 <= 0f) ? (-num42) : (360f - num42);
												array21[num38] = num43;
												int count = list6.Count;
												int num44 = -1;
												for (int num45 = 0; num45 < count; num45++)
												{
													if (num43 < array21[list6[num45]])
													{
														num44 = num45;
														break;
													}
												}
												if (num44 > -1)
												{
													list6.Insert(num44, num38);
												}
												else
												{
													list6.Add(num38);
												}
												num38++;
											}
										}
										array19 = new int[hashSet.Count];
										array20 = new int[hashSet.Count];
										hashSet.CopyTo(array20, 0);
										for (int num46 = 0; num46 < array19.Length; num46++)
										{
											array19[num46] = array20[list6[num46]];
										}
										hashSet.Clear();
										hashSet.UnionWith(array19);
										int num47 = num8 + 1;
										int num48 = 0;
										num38 = 0;
										foreach (int current2 in hashSet)
										{
											if (current2 == num8)
											{
												num48 = current2;
											}
											else
											{
												array8[num47] = vertices[current2];
												for (int num49 = 0; num49 < list2.Count; num49++)
												{
													if (list2[num49] == current2)
													{
														list2[num49] = num47;
													}
												}
												float num43 = (float)num38 * num37;
												if (num43 <= 90f)
												{
													x = num43 / 90f;
													y = 0f;
												}
												else if (num43 <= 180f)
												{
													x = 1f;
													y = (num43 - 90f) / 90f;
												}
												else if (num43 <= 270f)
												{
													x = (270f - num43) / 90f;
													y = 1f;
												}
												else if (num43 <= 360f)
												{
													x = 0f;
													y = (360f - num43) / 90f;
												}
												if (num48 != num8 && vertices[current2] == vertices[num48])
												{
													array11[num47] = array11[num47 - 1];
												}
												else
												{
													array11[num47] = new Vector2(x, y);
												}
												array15[num47] = boneWeights[current2];
												array17[num47] = normalized;
												num48 = current2;
												num47++;
												num38++;
											}
										}
										num38 = 0;
										list6.Clear();
										array21 = new float[hashSet2.Count];
										foreach (int current3 in hashSet2)
										{
											if (current3 == num8)
											{
												array21[num38] = 0f;
												list6.Insert(0, num38);
												num38++;
											}
											else
											{
												Vector3 position2 = vertices[current3];
												Vector3 vector4 = gameObject3.transform.InverseTransformPoint(position2) - Vector3.Dot(gameObject3.transform.InverseTransformPoint(position2), gameObject3.transform.InverseTransformDirection(vector2.normalized)) * gameObject3.transform.InverseTransformDirection(vector2.normalized);
												vector5.x = -vector3.x;
												vector5.y = -vector3.y;
												vector6.x = -vector4.x;
												vector6.y = -vector4.y;
												float num39 = Mathf.Atan2(vector5.y, vector5.x);
												float num40 = Mathf.Atan2(vector6.y, vector6.x);
												float num41 = num39 - num40;
												float num42 = num41 * 180f / 3.14159274f;
												float num43 = (num42 <= 0f) ? (-num42) : (360f - num42);
												array21[num38] = num43;
												int count = list6.Count;
												int num44 = -1;
												for (int num50 = 0; num50 < count; num50++)
												{
													if (num43 < array21[list6[num50]])
													{
														num44 = num50;
														break;
													}
												}
												if (num44 > -1)
												{
													list6.Insert(num44, num38);
												}
												else
												{
													list6.Add(num38);
												}
												num38++;
											}
										}
										array19 = new int[hashSet2.Count];
										array20 = new int[hashSet2.Count];
										hashSet2.CopyTo(array20, 0);
										for (int num51 = 0; num51 < array19.Length; num51++)
										{
											array19[num51] = array20[list6[num51]];
										}
										hashSet2.Clear();
										hashSet2.UnionWith(array19);
										num47 = num8 + 1;
										num48 = 0;
										num38 = 0;
										foreach (int current4 in hashSet2)
										{
											if (current4 == num8)
											{
												num48 = current4;
											}
											else
											{
												array9[num47] = vertices[current4];
												for (int num52 = 0; num52 < list3.Count; num52++)
												{
													if (list3[num52] == current4)
													{
														list3[num52] = num47;
													}
												}
												float num43 = (float)num38 * num37;
												if (num43 <= 90f)
												{
													x = num43 / 90f;
													y = 0f;
												}
												else if (num43 <= 180f)
												{
													x = 1f;
													y = (num43 - 90f) / 90f;
												}
												else if (num43 <= 270f)
												{
													x = (270f - num43) / 90f;
													y = 1f;
												}
												else if (num43 <= 360f)
												{
													x = 0f;
													y = (360f - num43) / 90f;
												}
												if (num48 != num8 && vertices[current4] == vertices[num48])
												{
													array12[num47] = array12[num47 - 1];
												}
												else
												{
													array12[num47] = new Vector2(x, y);
												}
												array16[num47] = boneWeights[current4];
												array18[num47] = normalized2;
												num48 = current4;
												num47++;
												num38++;
											}
										}
										mesh.vertices = array8;
										mesh.normals = array17;
										mesh.uv = array11;
										mesh.tangents = array13;
										mesh.boneWeights = array15;
										mesh.triangles = array4;
										skinnedMeshRenderer.materials = clsdismemberator.vargamskinnedmeshrenderer.materials;
										mesh2.vertices = array9;
										mesh2.normals = array18;
										mesh2.uv = array12;
										mesh2.tangents = array14;
										mesh2.boneWeights = array16;
										mesh2.triangles = array10;
										if (num5 > -1)
										{
											mesh.subMeshCount = sharedMesh.subMeshCount;
											mesh2.subMeshCount = sharedMesh.subMeshCount;
										}
										else
										{
											num5 = sharedMesh.subMeshCount;
											mesh.subMeshCount = num5 + 1;
											mesh2.subMeshCount = num5 + 1;
										}
										int num53 = 0;
										for (int num54 = 0; num54 < num5; num54++)
										{
											int num55 = sharedMesh.GetTriangles(num54).Length;
											int[] array22 = new int[num55];
											for (int num56 = 0; num56 < num55; num56++)
											{
												array22[num56] = array4[num56 + num53];
											}
											mesh.SetTriangles(array22, num54);
											num53 = num55;
										}
										mesh.SetTriangles(list2.ToArray(), num5);
										mesh2.SetTriangles(list3.ToArray(), num5);
									}
									else
									{
										mesh.vertices = sharedMesh.vertices;
										mesh.normals = sharedMesh.normals;
										mesh.tangents = sharedMesh.tangents;
										mesh.uv = sharedMesh.uv;
										mesh.uv2 = sharedMesh.uv2;
										mesh.boneWeights = sharedMesh.boneWeights;
										mesh.triangles = array4;
										skinnedMeshRenderer.materials = clsdismemberator.vargamskinnedmeshrenderer.materials;
										int[] array10 = new int[list.Count];
										list.CopyTo(array10, 0);
										mesh2.vertices = sharedMesh.vertices;
										mesh2.normals = sharedMesh.normals;
										mesh2.tangents = sharedMesh.tangents;
										mesh2.uv = sharedMesh.uv;
										mesh2.uv2 = sharedMesh.uv2;
										mesh2.boneWeights = sharedMesh.boneWeights;
										mesh2.triangles = array10;
										int subMeshCount = sharedMesh.subMeshCount;
										mesh.subMeshCount = subMeshCount;
										mesh2.subMeshCount = subMeshCount;
										int num57 = 0;
										for (int num58 = 0; num58 < subMeshCount; num58++)
										{
											int num59 = sharedMesh.GetTriangles(num58).Length;
											int[] array23 = new int[num59];
											for (int num60 = 0; num60 < num59; num60++)
											{
												array23[num60] = array4[num60 + num57];
											}
											mesh.SetTriangles(array23, num58);
											num57 = num59;
										}
									}
									Transform[] array24 = new Transform[clsdismemberator.vargamskinnedmeshrenderer.bones.Length];
									Matrix4x4[] array25 = new Matrix4x4[sharedMesh.bindposes.Length];
									clsdismemberator.vargamskinnedmeshrenderer.bones.CopyTo(array24, 0);
									sharedMesh.bindposes.CopyTo(array25, 0);
									Transform[] bones = clsdismemberator.vargamskinnedmeshrenderer.bones;
									Matrix4x4[] bindposes = sharedMesh.bindposes;
									for (int num61 = 0; num61 < array24.Length; num61++)
									{
										array24[num61] = gameObject2.transform;
										array25[num61] = array25[num4];
									}
									for (int num62 = 0; num62 < array6.Length; num62++)
									{
										int num63 = clsdismemberator.vargamboneindexes.IndexOf(array6[num62]);
										if (num63 != -1)
										{
											array24[num63] = array6[num62];
											array25[num63] = bindposes[num63];
											bones[num63] = gameObject.transform;
											bindposes[num63] = bindposes[num4];
										}
									}
									skinnedMeshRenderer.bones = array24;
									mesh2.bindposes = array25;
									clsdismemberator.vargamskinnedmeshrenderer.bones = bones;
									mesh.bindposes = bindposes;
									clsdismemberator.vargamskinnedmeshrenderer.sharedMesh = mesh;
									skinnedMeshRenderer.sharedMesh = mesh2;
									if (varpcinematiccut)
									{
										Rigidbody[] componentsInChildren = gameObject2.GetComponentsInChildren<Rigidbody>();
										for (int num64 = 0; num64 < componentsInChildren.Length; num64++)
										{
											componentsInChildren[num64].isKinematic = false;
										}
									}
								}
							}
						}
					}
				}
				IL_23B9:;
			}
			return true;
		}

		public static void metprint(string varpmessage, int varplevel, bool varpverbose = false, UnityEngine.Object varptarget = null)
		{
			switch (varplevel)
			{
			case 0:
				if (varpverbose)
				{
					Debug.Log(varpmessage, varptarget);
				}
				return;
			case 1:
				Debug.LogWarning(varpmessage, varptarget);
				return;
			case 2:
				Debug.LogError(varpmessage, varptarget);
				return;
			case 4:
				if (varpverbose)
				{
					Debug.LogWarning(varpmessage, varptarget);
				}
				return;
			}
			Debug.Log(varpmessage, varptarget);
		}
	}
}

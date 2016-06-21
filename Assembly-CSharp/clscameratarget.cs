using System;
using UnityEngine;

public class clscameratarget : MonoBehaviour
{
	private const int cnsbuttonwidthmenu = 250;

	private const int cnsbuttonwidth = 100;

	private const string cnsdemostagename = "__URG_Demo";

	private const string cnsdemodismemberatorstage = "__URG_Dismemberator Tester";

	private const string cnsdemoresetname = "__URG_Empty_scene";

	private const string cnssoldier = "Soldier";

	public Transform vargamtarget;

	public clscameratargetdata[] vargamscenarios;

	public float vargamtrackingspeed = 0.3f;

	public int vargamscene;

	public bool vargamtimezero;

	public int vargamcurrentscenario = -2;

	private bool vardisplaymoreinfo;

	private GameObject varsoldier;

	private Vector3 varcurrenttargetposition;

	private Vector3 varsmoothtargetposition;

	private void Awake()
	{
		Time.fixedDeltaTime = 0.01f;
		Physics.defaultContactOffset = 0.01f;
		Physics.IgnoreLayerCollision(2, 0);
		this.metwatchscenario(0);
		if (this.vargamtimezero)
		{
			Time.timeScale = 0f;
		}
	}

	private void metwatchscenario(int varpscenario)
	{
		this.vargamtarget = null;
		this.vargamcurrentscenario = varpscenario;
		Time.timeScale = 1f;
		if (this.vargamscenarios[varpscenario] == null || this.vargamscenarios[varpscenario].propviewport == null)
		{
			Debug.LogError("Scenario [" + varpscenario + "] is null. Please assign a scenario to the manager to proceed.");
			return;
		}
		base.transform.position = this.vargamscenarios[varpscenario].propviewport.position;
		base.transform.rotation = this.vargamscenarios[varpscenario].propviewport.rotation;
		this.varsmoothtargetposition = base.transform.position + base.transform.forward;
		if (this.vargamscenarios[varpscenario].proptarget != null)
		{
			this.vargamtarget = this.vargamscenarios[varpscenario].proptarget;
		}
		this.vardisplaymoreinfo = false;
	}

	private void metresetlevel()
	{
		Application.LoadLevel("__URG_Demo");
	}

	private void metresetlevel(string varplevelname)
	{
		Application.LoadLevel(varplevelname);
	}

	private void OnGUI()
	{
		GUI.skin.box.alignment = TextAnchor.UpperLeft;
		int num = this.vargamscene;
		if (num != 1)
		{
			switch (this.vargamcurrentscenario)
			{
			case 1:
				GUILayout.Box("Simple ragdoll functionality.\r\n\t\tThe soldier game model was ragdolled into a prefab in editor mode. At the press of the 'Go ragdoll' button\r\n\t\tbelow, the soldier will run until it starts to fall, which causes the ragdolled prefab to be spawned and\r\n\t\tposed like the original gameobject, which is destroyed.\r\n\t\tThis approach uses two different game objects for game character and ragdoll, allowing easy differentiation\r\n\t\tbetween the two. This way, the ragdoll can be spawned without controller nor other important scripts that\r\n\t\tare instead needed on the character.\r\n\t\tInstructions:\r\n\t\t- In Edit mode, Start URG with Gameobject menu=> Create Other=> Ultimate ragdoll\r\n\t\t- Drag the desired game character into the source (topmost) slot\r\n\t\t- Press create ragdoll\r\n\t\t- Manually add colliders to the ragdoll parts", new GUILayoutOption[0]);
				if (GUILayout.Button("<= Back", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				}))
				{
					this.metresetlevel();
				}
				if (!this.vardisplaymoreinfo && GUILayout.Button("More Information", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.vardisplaymoreinfo = true;
				}
				if (this.vardisplaymoreinfo)
				{
					GUILayout.Box("Implementation information:\r\n\t\t- The soldier_ragdollified_controller gameobject moves thanks to the clsactorcontroller component, which\r\n\t\t\tuses Physics.Raycast to determine if it's falling beyond its allowed limit.\r\n\t\t- If the soldier is falling, the gameobject uses the clsragdollhelper component to instantiate its\r\n\t\t\tragdoll, with the Transform varragdoll = varhelper.metgoragdoll instruction. This instruction in turn\r\n\t\t\tcalls the clsragdollify component, which hosts the actual ragdoll prefab to instance.\r\n\t\t- The metgoragdoll function poses the spawned ragdoll like its parent, adds velocity to the bodyparts\r\n\t\t\tto preserve the original momentum, and destroys the host.\r\n\t\t- The weapon unparents when the ragdoll spawns, thanks to the clsdrop component.", new GUILayoutOption[0]);
					GUILayout.Box("Setup information:\r\n\t\t- Dragged the soldier model from the project into the scene, and created a non kinematic ragdoll for it\r\n\t\t\tusing URG Developer edition.\r\n\t\t- Added rigidbody, box collider and clsdrop components to the gun of the soldier (m4mb gameobject)\r\n\t\t- Created a prefab (Soldier_ragdoll). Deleted the scene gameobject, and dragged the soldier model into\r\n\t\t\tthe scene again, to add charactercontroller, clsragdollhelper, clsragdollify and\r\n\t\t\tclsactorcontroller components to it. In particular, dragged the Soldier_ragdoll prefab into the\r\n\t\t\tclsragdollify component slot.\r\n\t\t- Named Solider_ragdollified_controller the soldier gameobject, and proceeded with the implementation\r\n\t\t\tabove.", new GUILayoutOption[0]);
					if (this.vardisplaymoreinfo && GUILayout.Button("Less Information", new GUILayoutOption[]
					{
						GUILayout.Width(250f)
					}))
					{
						this.vardisplaymoreinfo = false;
					}
				}
				break;
			case 2:
				GUILayout.Box("Basic ragdoll functionality.\r\n\t\tThe character was ragdolled in edit mode, with the 'Kinematic' checkbox flagged in URG! options.\r\n\t\tAdditionally, a trigger script was added to it. When the desired event occurs (in this case a simple time\r\n\t\twait), the collider script turns the character rigidbody components from 'Kinematic' to 'Physic driven',\r\n\t\tand the character turns into a ragdoll.\r\n\t\tThis method requires more work than ragdoll instancing, but is necessary when ragdoll to animation transition\r\n\t\tis needed afterwards.\r\n\t\tSince game character and ragdoll are the same, this method is normally used when the two character states\r\n\t\tshare most of the logic. For example, if the ragdoll differs from the character because of the controller\r\n\t\talone, deactivating it via script is more maintenable than creating a separated ragdoll without it.\r\n\t\tInstructions:\r\n\t\t- Start URG with Gameobject menu=> Create Other=> Ultimate ragdoll\r\n\t\t- Drag the desired game character into the source (topmost) slot\r\n\t\t- Expand options and make sure the 'Kinematic ragdoll' option is checked\r\n\t\t- Press create ragdoll\r\n\t\t- Manually add colliders to the ragdoll parts", new GUILayoutOption[0]);
				if (GUILayout.Button("<= Back", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				}))
				{
					this.metresetlevel();
				}
				if (!this.vardisplaymoreinfo && GUILayout.Button("More Information", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.vardisplaymoreinfo = true;
				}
				if (this.vardisplaymoreinfo)
				{
					GUILayout.Box("Implementation information:\r\n\t\tThe character simply transitions from its normal, game state into a ragdoll state, by means of a script.\r\n\t\t- the clsshowcasehelper script receives its activation message from the showcase manager.\r\n\t\t- after a little wait, animations are stopped, since otherwise the rigidbodies don't become physic.\r\n\t\t- the clsurgutils.metgodriven call is made and the character is preserved, but becomes a ragdoll.", new GUILayoutOption[0]);
					GUILayout.Box("Setup information:\r\n\t\t- The character was simply dragged into the scene, and ragdolled with URG Developer version, with the\r\n\t\t\t'Kinematic' parameter checked.\r\n\t\t- The clsshowcasehelper script was added to the Lerpz_kinematic gameobject, case 2 was added to the\r\n\t\t\tshowcase manager clscameratarget, and case 5 was added to the clsshowcasehelper script.", new GUILayoutOption[0]);
					if (this.vardisplaymoreinfo && GUILayout.Button("Less Information", new GUILayoutOption[]
					{
						GUILayout.Width(250f)
					}))
					{
						this.vardisplaymoreinfo = false;
					}
				}
				break;
			case 3:
				GUILayout.Box("Advanced ragdoll:\r\n\t\tThe character was ragdolled in edit mode, with the 'URGent' checkbox flagged in URG! options.\r\n\t\tThen, thanks to the urgent actuators and the urgent state manager, the collision event generated from the\r\n\t\tweight that hits the character, issues a call that turns all its rigidbodies into physic driven in a single\r\n\t\tline of code.\r\n\t\tThe URGent manager offers a convenient approach to manage ragdoll cases and implement different states based\r\n\t\ton character data. For example, the URGent manager can greatly simplify ragdoll behavior when any ragdolled\r\n\t\tcharacter is killed, or needs be dismembered, or needs to transition from ragdoll back to animation, all\r\n\t\twithin of a single control script, that can serve any number of ragdoll templates.", new GUILayoutOption[0]);
				if (GUILayout.Button("<= Back", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				}))
				{
					this.metresetlevel();
				}
				if (!this.vardisplaymoreinfo && GUILayout.Button("More Information", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.vardisplaymoreinfo = true;
				}
				if (this.vardisplaymoreinfo)
				{
					GUILayout.Box("Implementation information:\r\n\t\t- Thanks to the URGent actuators, all the ragdoll logic of this example resides in the clsurgentactuator class.\r\n\t\t- The scripts uses the vargamcasemanager value of the vargamurgentsource attribute to determine which 'kind' of\r\n\t\t\tragdoll is acting, and responds consequently thanks to a simple Switch command on the OnCollisionEnter event.\r\n\t\t- In this particular case, actuators are used to call clsurgutils.metdriveurgent and transition the character\r\n\t\t\tinto a ragdoll, but in more elaborate cases the user can choose to decrease character bodypart hitpoints,\r\n\t\t\tdismember, break individual limbs, etc.\r\n\t\t- Lastly, the weapon is detached with a simple GetComponent call for clsdrop.", new GUILayoutOption[0]);
					GUILayout.Box("Setup information:\r\n\t\t- The soldier character was instanced and t-posed, and ragdolled with URG Developer with the 'Add URG scripts'\r\n\t\t\tand 'Kinematic ragdoll' options enabled.\r\n\t\t- Rigidbody, Collider and clsdrop components are added to the soldier weapon gameobject. The rigidbody is setup\r\n\t\t\tas Kinematic.\r\n\t\t- The soldier was positioned and posed for the scene.", new GUILayoutOption[0]);
					if (this.vardisplaymoreinfo && GUILayout.Button("Less Information", new GUILayoutOption[]
					{
						GUILayout.Width(250f)
					}))
					{
						this.vardisplaymoreinfo = false;
					}
				}
				break;
			case 4:
				GUILayout.Box("Advanced ragdoll to animation:\r\n\t\tThis feature is very sought after, since a ragdoll can become animated again, but there's no way to create a fluid,\r\n\t\tanimation like transition without it.\r\n\t\tAny standard or URGent ragdoll can receive the URG Animation states class. In edit mode, the ASM compiler creates\r\n\t\tdata structures that are used in play mode, to create ragdoll to animation or animation to animation transitions.\r\n\t\tIn this example, there's a simple script that checks if the character has fallen, and transitions to one 'rise'\r\n\t\tanimation, which is consequently played.\r\n\t\tThe transition call is extremely fast and requires only a line of code, and the quality of the final effect is just\r\n\t\tlimited by the quality and number of the landing animations (i.e. one when the character has fallen on its back,\r\n\t\tor on its belly or side, etc.)", new GUILayoutOption[0]);
				if (GUILayout.Button("<= Back", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				}))
				{
					this.metresetlevel();
				}
				if (!this.vardisplaymoreinfo && GUILayout.Button("More Information", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.vardisplaymoreinfo = true;
				}
				if (this.vardisplaymoreinfo)
				{
					GUILayout.Box("Implementation information:\r\n\t\t- This functionality is very flexible and can be used to also perform animation to animation transitions,\r\n\t\t\tbut in this particular case, it allows the ragdoll to play as an animated character again, after becoming\r\n\t\t\ta ragdoll.\r\n\t\t- The character is setup to play the 'balance' animation automatically, so the clsshowcasehelper is used\r\n\t\t\tto simply time the call to the 'metAsm' function, which wraps the 'metAsmRoutine' function in a loop.\r\n\t\t- Inside the 'metAsmRoutine' the animations are stopped, the character becomes ragdoll, and finally the\r\n\t\t\ttransition is made with a single line call to metcrossfadetransitionanimation, which automatically takes\r\n\t\t\tcare of restoring the animation ability of the ragdoll.", new GUILayoutOption[0]);
					GUILayout.Box("Setup information:\r\n\t\t- The character was dragged t-posed into the scene, and ragdolled as kinematic with URG Developer edition.\r\n\t\t- The URG animation states was then used, dragging the character into its slot and clicking the 'Memorize'\r\n\t\t\tbutton to State all animations.\r\n\t\t- The character was posed, and the clsshowcasehelper script was added to it with switch 6", new GUILayoutOption[0]);
				}
				break;
			case 5:
				GUILayout.Box("Advanced dismemberable character:\r\n\t\t\tThe 'Big D' is a next-gen utility and URG exclusive that allows separation of any 'Transform',\r\n\t\talong with its mesh triangles, from the main gameobject.\r\n\t\tThe compiled class is installed in edit mode with the Dismemberator utility\r\n\t\tand is used afterwards in a call with a single line of code.\r\n\t\tSeparation can optionally instance cut triangles with an user defined\r\n\t\tmaterial, and parent and child separation gameobjects (for example particles) if so desired.\r\n\t\tAdditionally, this feature doesn't affect animations, so it becomes possible\r\n\t\tto keep an animation running, and seamlessly detach any gameobject part.\r\n\t\tNOTE: this feature is CPU intensive. To flawlessly perform multiple single frame cuts we recommend\r\n\t\tchoosing or producing an optimized 3d model (Consult Unity guidelines for more information regarding\r\n\t\tnumber of bones and triangles in optimal game characters)", new GUILayoutOption[0]);
				if (GUILayout.Button("<= Back", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				}))
				{
					this.metresetlevel();
				}
				if (!this.vardisplaymoreinfo && GUILayout.Button("More Information", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.vardisplaymoreinfo = true;
				}
				if (this.vardisplaymoreinfo)
				{
					GUILayout.Box("Implementation information:\r\n\t\t- The scenario manager (this script), sends an activate message to the /__Scenery/_Bombspawn gameobject.\r\n\t\t- The bombspawn gameobject activates thanks to the clsdismemberatorhelper script, spawns an explosion and\r\n\t\t\tlooks for the /Zombie_D gameobject.\r\n\t\t- The Zombie_D parts are processed by a random selection, to determine which part is going to be cut.\r\n\t\t- Parts to cut receive the 'Transform varcurrentcut = clsurgutils.metdismemberpart' command, followed by a\r\n\t\t\tforce push\r\n\t\t- IMPORTANT: please keep the clsdismemberator component FOLDED in the inspector, to work around an unity bug\r\n\t\t\tthat causes a slowdown with a skinned mesh renderer in a public script slot.", new GUILayoutOption[0]);
					GUILayout.Box("Setup information:\r\n\t\t- Dragged the zombie model from the project into the scene.\r\n\t\t- Duplicated (CTRL D) the idle animation from the project into the /_models/_animations folder, and assigned\r\n\t\t\tit to the instanced zombie (this is a necessary step to allow animation after separation).\r\n\t\t- Ran URG Developer edition on the model and created a kinematic ragdoll\r\n\t\t- Ran URG Dismemberator on the model, which added the clsdismemberator component to the zombie.\r\n\t\t\tNOTE: please keep the clsdismemberator component FOLDED, otherwise it'll slow down the scene camera, due\r\n\t\t\t\t  to a bug with Unity's inspector.\r\n\t\t- Created a prefab for the zombie, and proceeded with the implementation above.", new GUILayoutOption[0]);
					if (this.vardisplaymoreinfo && GUILayout.Button("Less Information", new GUILayoutOption[]
					{
						GUILayout.Width(250f)
					}))
					{
						this.vardisplaymoreinfo = false;
					}
				}
				break;
			case 6:
				GUILayout.Box("Linear ragdoll for partial characters:\r\n\t\tWith certain unusual ragdolls (multiple limbs like spiders, or linear like ropes, fences, chains, etc.), the normal\r\n\t\tragdolling procedure might not complete properly. Thanks to the 'Fake Limbs' and 'Connect' URG! functions\r\n\t\tit becomes possible to create separate linear ragdolls, and connect them with a single click, to make the\r\n\t\tfinal gameobject perform as close to a 'full' ragdoll as possible.\r\n\t\tInstructions:\r\n\t\t- Create a ragdoll as per previous instructions\r\n\t\t- Locate the non ragdolled part (for example a tail) by exploring the ragdoll parts\r\n\t\t- Use the 3d editor to rotate the limb and point it upwards\r\n\t\t- Drag the limb into URG source slot and press the 'Create ragdoll' button\r\n\t\t    This will assign a 'spine' behavior to the part.\r\n\t\t- Drag the parent part of the recently ragdolled part (for example the back) into the\r\n\t\t    connection slot\r\n\t\t- Press the 'Connect part' button. Repeat until all parts are ragdolled.", new GUILayoutOption[0]);
				if (GUILayout.Button("<= Back", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				}))
				{
					this.metresetlevel();
				}
				if (!this.vardisplaymoreinfo && GUILayout.Button("More Information", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.vardisplaymoreinfo = true;
				}
				if (this.vardisplaymoreinfo)
				{
					GUILayout.Box("Implementation information:\r\n\t\t- Linear ragdolls can be any type of ragdoll (Instanced, Kinematic, URGent, etc.) so they can be created with\r\n\t\t\tany design in mind.\r\n\t\t- The difference from normal ragdolls is that the user will use the 'Fake Limbs' feature to provide URG with \r\n\t\t\tdelimiters, that will shape the Colliders of the 'spine' of the ragdoll to be.\r\n\t\t- Thanks to this behavior, it is just enough to select a different source object each time, and all 'extra'\r\n\t\t\tlimbs can be ragdolled as a Spine, to eventually connect all of the ragdolled parts together, with the\r\n\t\t\t'Connect ragdoll' feature.", new GUILayoutOption[0]);
					GUILayout.Box("Setup information:\r\n\t\t- Tree, Rope and Weight models are dragged into the scene. Tree and Rope are created using the 'Fake Limbs'\r\n\t\t\tfeature. Since their bones are linear, a 'Spine' setup is created for their ragdolling process.\r\n\t\t- In particular, the Tree was created with an uniform flexibility of 1.6, while the Rope with an u.f. of 4.5\r\n\t\t- The weight received a Collider and Rigidbody, and the three objects were setup in their Intended positions,\r\n\t\t\tin Cinematic mode (Kinematic flag unchecked on all rigidbodies)\r\n\t\t- Using the 'Connect ragdoll' feature, the Weight was connected to the Rope, and then the Rope was connected\r\n\t\t\tto the Tree. Note that actual 3d world position is important during connection, since the Joints will pivot\r\n\t\t\taround their creation or connection center.", new GUILayoutOption[0]);
					if (this.vardisplaymoreinfo && GUILayout.Button("Less Information", new GUILayoutOption[]
					{
						GUILayout.Width(250f)
					}))
					{
						this.vardisplaymoreinfo = false;
					}
				}
				break;
			case 7:
				GUILayout.Box("Dismember for arbitrary game models:\r\n\t\tDismemberator functionality is handily available for any sort of SkinnedMeshRendered gameobject.\r\n\t\tTanks to the 'Big D', as long as an object has 3D bones and is properly skinned,it will be possible to\r\n\t\tdynamically detach any of its parts in realtime.\r\n\t\tIn this example, the bike hosts the Big D class, and when it impacts the ground with a simple sphere trigger,\r\n\t\ta basic routine iterates through the bike parts to disconnect them, issuing a single instruction call to the\r\n\t\tdismemberator utility.\r\n\t\tThe main power of this feature is that the target gameobject can just be a single mesh, designed and rigged\r\n\t\tpart by part, so it's perfect to disassemble objects that are subject to animations (turrets, furniture, coffers,\r\n\t\tvehicles, etc.).", new GUILayoutOption[0]);
				if (GUILayout.Button("<= Back", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				}))
				{
					this.metresetlevel();
				}
				if (!this.vardisplaymoreinfo && GUILayout.Button("More Information", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.vardisplaymoreinfo = true;
				}
				if (this.vardisplaymoreinfo)
				{
					GUILayout.Box("Implementation information:\r\n\t\t- The fulcrum of arbitrary separation is that for each separation target bone, there's a closed loop in the mesh,\r\n\t\t\tfor example the bike wheels, or a turret cannon, or a table leg.\r\n\t\t- The power of arbitrary separation is that there's no triangulation involved. As long as the 3D artist creates a\r\n\t\t\tbone for the particular game model component, Big D will be able to detach that bone, and all the triangles\r\n\t\t\tskinned to it.\r\n\t\t- Functionality is identical to standard dismember, so separation can instance child and parent special effects,\r\n\t\t\tand compilation is the same as for regular skinned gameobjects.\r\n\t\t- Mayhem potential of this feature is limitless. Even if an object is not animated, as long as its rigged and\r\n\t\t\tskinned, it can be separated part by part. Disassembling objects apart has suddenly become easy, with just basic\r\n\t\t\t3D gamemodel design.", new GUILayoutOption[0]);
					GUILayout.Box("Setup information:\r\n\t\t- The bike was rigged in Blender in a matter of minutes, assigning bones to each of the parts that we wanted to\r\n\t\t\tdisassemble. These parts are closed loops, so that there's no need for triangulation when they are separated.\r\n\t\t- Once imported, Big D was run on the model, and the clsbike and the clsshowcase helper scripts were added to assign\r\n\t\t\tthe logic.\r\n\t\t- Physical objects were added to the bike: a kinematic rigidbody and a collider for all bones, and a main collider\r\n\t\t\tand a non kinematic rigidbody for the bike root.\r\n\t\t- The Lerpz gamemodel was ragdolled, and made non kinematic and non tangible with the Kinetifier utility, and then\r\n\t\t\tposed and parented to the bike.\r\n\t\t- When the scene runs, the bike uses wheel colliders to propel itself, and when its sphere trigger collides with the\r\n\t\t\tground, it iterates through its bones and separates each part. Additionally, it detects the ground below with a\r\n\t\t\traycast, and calls the metfalling routine to detach the passenger.", new GUILayoutOption[0]);
					if (this.vardisplaymoreinfo && GUILayout.Button("Less Information", new GUILayoutOption[]
					{
						GUILayout.Width(250f)
					}))
					{
						this.vardisplaymoreinfo = false;
					}
				}
				break;
			case 8:
				GUILayout.Box("Animated game character part break:\r\n\t\tThis is one often sought after feature, which can be easily achieved thanks to the URGent classes and manager. The\r\n\t\tragdoll is created by adding the URG entity scripts, and whenever needed, a simple function call is made and desired\r\n\t\tlimbs become fully physical, ignoring animation curves.\r\n\t\tNOTE: this behavior relies on the 'Animate physics' feature of the skinned mesh renderer, and thus does not work with\r\n\t\tMecanim rigged characters automatically.", new GUILayoutOption[0]);
				if (GUILayout.Button("<= Back", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				}))
				{
					this.metresetlevel();
				}
				if (!this.vardisplaymoreinfo && GUILayout.Button("More Information", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.vardisplaymoreinfo = true;
				}
				if (this.vardisplaymoreinfo)
				{
					GUILayout.Box("Implementation information:\r\n\t\t- Limbified ragdolls are URGent compiled ragdolls with the 'Animate physics' flag checked in their Skinned Mesh\r\n\t\t\tRenderer component.\r\n\t\t- Part breaking is based on kinematic functionality of individual bodyparts, which are transformed thanks to the direct\r\n\t\t\taccess allowed by the URGent class.\r\n\t\t- A broken part can be repaired by a later call to the breaking routine, with a different parameter.\r\n\t\t- This feature can also be used to simulate animation states. For example an animated flower can be broken, and it will\r\n\t\t\tfall as if dead when limbified. To later bring the flower back to animation. Transition is instantaneous, but result\r\n\t\t\tcan be satisfying in many cases, and without the extra work involved in ASM preparation.", new GUILayoutOption[0]);
					GUILayout.Box("Setup information:\r\n\t\t- The 'Soldier_Urgent_controller' character was ragdolled normally, but checking 'Add URG entity scripts', and adding a\r\n\t\t\tcontroller and the clsactorcontroller to it afterwards. The '_Tooncannon' model was prepared with the specific\r\n\t\t\tclscannon script, and in particular the 'cannonball' gameobject was prefabbed with the 'missile' tag.\r\n\t\t- The soldier character parts are using the 'stock' clsurgentactuator class, that simply runs the 'OnCollisionEnter'\r\n\t\t\tevent, monitoring the '-3' case from a 'missile' tagged gameobject.\r\n\t\t- When the cannonball hits any clsurgentactuator host, the collision event is triggered, and the part is driven with a\r\n\t\t\tcall to the metdrivebodypart function.", new GUILayoutOption[0]);
					if (this.vardisplaymoreinfo && GUILayout.Button("Less Information", new GUILayoutOption[]
					{
						GUILayout.Width(250f)
					}))
					{
						this.vardisplaymoreinfo = false;
					}
				}
				break;
			default:
				GUILayout.Box("Good day, and welcome to URG! demo scene and feature showcase.\r\n\t\tThanks for your interest in the U.R.G. Please press the buttons for a demonstration of the available functions.\r\n\t\tFor usage instructions, please see the reference file.", new GUILayoutOption[0]);
				GUILayout.Box("This version includes:\r\n\t\t- The Ultimate Ragdoll Generator.\r\n\t\t   > Can instantly create fully configured ragdolls for any suitable game character.\r\n\t\t   > Start the URG from the Gameobject=> Create Other=> Ultimate Ragdoll menu.\r\n\t\t- The Dismemberator.\r\n\t\t   > A next get utility that installs a component that allows real time game character dismemberment.\r\n\t\t   > Start the Dismemberator from the Gameobject=> Create Other=> URG Dismemberator menu.\r\n\t\t- Animation states manager.\r\n\t\t   > This utility deals with the otherwise difficult and time consuming process of managing transition animations\r\n\t\t\t\twhen dealing with ragdolls.\r\n\t\t   > Start the ASM from the Gameobject=> Create Other=> Ultimate Animation States menu.\r\n\t\t- Kinetifier.\r\n\t\t   > A convenient editor to set physic driven or kinematic state for all rigidbodies in a gameobject, and to\r\n\t\t\t\ttoggle activation state of colliders.\r\n\t\t   > Start the Kinetifier from the Gameobject=> Create Other=> Ultimate Ragdoll Kinetifier menu.", new GUILayoutOption[0]);
				GUILayout.Space(10f);
				if (GUILayout.Button("Simple use: ragdoll prefab", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					if (this.varsoldier == null)
					{
						this.varsoldier = GameObject.Find("Soldier");
					}
					this.metwatchscenario(1);
				}
				if (GUILayout.Button("Basic use: kinematic ragdoll", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.metwatchscenario(2);
					GameObject.Find("/Lerpz_kinematic").SendMessage("metactivate");
				}
				if (GUILayout.Button("Advanced use: URGent ragdoll", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.metwatchscenario(3);
				}
				if (GUILayout.Button("Advanced use: Animation States", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.metwatchscenario(4);
					GameObject.Find("/Alien_ASM").SendMessage("metactivate");
				}
				if (GUILayout.Button("Advanced use: Dismemberator", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.metwatchscenario(5);
					GameObject.Find("/__Scenery/_Bombspawn").SendMessage("metactivate");
				}
				if (GUILayout.Button("Special use: Linear ragdoll", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.metwatchscenario(6);
				}
				if (GUILayout.Button("Special use: Arbitrary separation", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.metwatchscenario(7);
				}
				if (GUILayout.Button("Special use: Limb break", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.metwatchscenario(8);
					GameObject.Find("/__Scenery/_Tooncannon").SendMessage("metactivate");
					GameObject.Find("/Soldier_Urgent_controller").SendMessage("metactivate");
				}
				if (!this.vardisplaymoreinfo && GUILayout.Button("More Information", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.vardisplaymoreinfo = true;
				}
				if (this.vardisplaymoreinfo)
				{
					GUILayout.Box("Implementation information:\r\n\t\t- Demo scene requires at least a 1024x768 viewport.", new GUILayoutOption[0]);
					GUILayout.Box("Setup information:\r\n\t\t- This script is hosted in the Camera_Menu_Timestepmanager gameobject.", new GUILayoutOption[0]);
				}
				break;
			}
		}
		else
		{
			int num2 = this.vargamcurrentscenario;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(" ", new GUILayoutOption[]
			{
				GUILayout.Width(200f)
			});
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Box("Welcome to the dismemberator tester scene.\r\n\t\tThis utility is meant to provide a way to verify that any model responds well to dismemberment,\r\n\t\tand to check if all parts separate correctly. Thanks to D.T. it becomes fast and easy to check\r\n\t\tif a certain bone is not properly skinned. With this information, it becomes possible to decide\r\n\t\twether to avoid that bone dismemberment, or to fix the issue with a 3D editor application.\r\n\t\tFor a start, it's important to know that the dismemberator tester script can be installed\r\n\t\tin any existing scene, and that it is presented here for ease of understanding.\r\n\t\tThe D.T. is a simple script that iterates through a ragdoll's physical parts, and presents\r\n\t\tthem as buttons.\r\n\t\tPressing the button of a ragdoll part will detach it using material and particles specified\r\n\t\ton the ragdoll's clsdismemberator script in the inspector.\r\n\t\tTo test a ragdoll dismemberment, drop it in the vargammodel slot of the clsdismemberator\r\n\t\tscript on the scene (attached to the main camera), and go into Play mode.\r\n\t\tNOTE: the ragdoll can be both a prefab from the project, or a scene model. Use the\r\n\t\tvargamspawnposition Vector3 of the dismemberator script to determine its spawning position.", new GUILayoutOption[0]);
			if (GUILayout.Button("Reload scene", new GUILayoutOption[]
			{
				GUILayout.Width(250f)
			}))
			{
				this.metresetlevel("__URG_Dismemberator Tester");
			}
			if (!this.vardisplaymoreinfo && GUILayout.Button("More Information", new GUILayoutOption[]
			{
				GUILayout.Width(250f)
			}))
			{
				this.vardisplaymoreinfo = true;
			}
			if (this.vardisplaymoreinfo)
			{
				GUILayout.Box("Implementation information:\r\n\t\t- The dismemberator tester is hosted on the main camera, and detects if a model has been\r\n\t\t\tassigned to it\r\n\t\t- When the editor goes into Play mode, the D.T. instantiates the model at the specified\r\n\t\t\tvargamspawnposition coordinates indicated in the inspector, and displays a list of the\r\n\t\t\tmodel parts that have physical elements.\r\n\t\t- Pressing the GUI buttons displayed issues a call to metdismember, to separate the specified\r\n\t\t\tpart.", new GUILayoutOption[0]);
				GUILayout.Box("Setup information:\r\n\t\t- Added the clsdismemberatortester script to the camera\r\n\t\t- Dragged the ratman model from the project into the scene, and created a kinematic\r\n\t\t\tragdoll for it using URG standard edition.\r\n\t\t- Added the dismemberator component using the URG Dismemberator option from the Gameobject menu.\r\n\t\t- Dragged the ratman from the scene into the vargammodel slot of the clsdismemberatortester\r\n\t\t\tscript hosted in the scene's main camera.\r\n\t\t- Assigned material and particles to the relevant ratman clsdismemberator slots.", new GUILayoutOption[0]);
				if (this.vardisplaymoreinfo && GUILayout.Button("Less Information", new GUILayoutOption[]
				{
					GUILayout.Width(250f)
				}))
				{
					this.vardisplaymoreinfo = false;
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
	}

	private void LateUpdate()
	{
		if (this.vargamtarget != null)
		{
			this.varcurrenttargetposition = this.vargamtarget.position;
			this.varsmoothtargetposition = Vector3.Lerp(this.varsmoothtargetposition, this.varcurrenttargetposition, Time.deltaTime * this.vargamtrackingspeed);
			base.transform.LookAt(this.varsmoothtargetposition);
		}
	}
}

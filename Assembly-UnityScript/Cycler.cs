using System;
using UnityEngine;

[Serializable]
public class Cycler : MonoBehaviour
{
	public GameObject[] childObject;

	public int changeObj;

	public GameObject currObjAlive;

	public override void onScrollChangeObject()
	{
		if (this.currObjAlive)
		{
			UnityEngine.Object.Destroy(this.currObjAlive);
		}
		this.currObjAlive = (GameObject)UnityEngine.Object.Instantiate(this.childObject[this.changeObj], this.transform.position, this.transform.rotation);
		this.currObjAlive.name = this.childObject[this.changeObj].name;
		this.currObjAlive.transform.parent = this.transform;
	}

	public override void Update()
	{
		if (Input.GetKeyDown("up"))
		{
			if (this.changeObj == 12)
			{
				this.changeObj = 0;
				this.onScrollChangeObject();
			}
			else
			{
				this.changeObj++;
				this.onScrollChangeObject();
			}
		}
		if (Input.GetKeyDown("down"))
		{
			if (this.changeObj == 0)
			{
				this.changeObj = 12;
				this.onScrollChangeObject();
			}
			else
			{
				this.changeObj--;
				this.onScrollChangeObject();
			}
		}
	}

	public override void OnGUI()
	{
		GUI.Label(new Rect((float)(Screen.width / 2 - 200), (float)(Screen.height / 2 - 50), (float)Screen.width, (float)Screen.height), this.currObjAlive.name);
		GUI.Label(new Rect((float)50, (float)50, (float)Screen.width, (float)Screen.height), "Use the Up and Down arrow keys to change FX!");
	}

	public override void Main()
	{
		this.onScrollChangeObject();
	}
}

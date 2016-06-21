using System;
using UnityEngine;

public class ClickMeAndDie : MonoBehaviour
{
	private int id;

	private static int _id;

	private void Start()
	{
		this.id = ClickMeAndDie._id++;
	}

	private void Update()
	{
		RaycastHit raycastHit;
		if (Input.GetMouseButtonDown(0) && base.GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 1000f))
		{
			JSONLevelSerializer.SaveObjectTreeToServer("ftp://whydoidoit.net/testspider" + this.id.ToString() + ".json", base.gameObject, "testserializer", "T3sts3rializer", delegate(Exception e)
			{
				if (e == null)
				{
					Loom.QueueOnMainThread(delegate
					{
						JSONLevelSerializer.LoadObjectTreeFromServer("http://whydoidoit.net/testserializer/testspider" + this.id, null, null);
					}, 2f);
				}
				else
				{
					Debug.Log(e.ToString());
				}
			});
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}

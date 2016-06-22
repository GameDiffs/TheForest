using System;
using UnityEngine;

[Serializable]
public class CastWeb : MonoBehaviour
{
	public GameObject Web;

	private Vector3 distance;

	private Vector3 fwd;

	public override void Awake()
	{
		this.Cast();
	}

	public override void Cast()
	{
		this.fwd = this.transform.TransformDirection(Vector3.forward);
		RaycastHit raycastHit = default(RaycastHit);
		if (Physics.Raycast(this.transform.position, this.fwd, out raycastHit) && raycastHit.collider.tag == "Tree")
		{
			float num = Vector3.Distance(this.transform.position, raycastHit.transform.position);
			if (num < (float)30)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.Web, this.transform.position, this.transform.rotation);
				gameObject.transform.position = 0.5f * (this.transform.position + raycastHit.transform.position);
				gameObject.transform.localScale = gameObject.transform.localScale * num / (float)2;
			}
		}
	}

	public override void Main()
	{
	}
}

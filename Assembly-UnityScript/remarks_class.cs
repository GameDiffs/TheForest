using System;

[Serializable]
public class remarks_class
{
	public bool textfield_foldout;

	public int textfield_length;

	public string textfield;

	public remarks_class()
	{
		this.textfield_length = 1;
		this.textfield = string.Empty;
	}
}

using System;
using System.IO;
using UnityEngine;

[Serializable]
public class auto_search_class
{
	public string path_full;

	public string path;

	public bool foldout;

	public bool custom;

	public int digits;

	public string format;

	public string filename;

	public string fullname;

	public string name;

	public string extension;

	public int start_x;

	public int start_y;

	public int start_n;

	public int count_x;

	public int count_y;

	public bool display;

	public int select_index;

	public Rect menu_rect;

	public string output_format;

	public auto_search_class()
	{
		this.path_full = string.Empty;
		this.path = string.Empty;
		this.digits = 1;
		this.format = "%n";
		this.filename = "tile";
		this.extension = ".raw";
		this.start_n = 1;
		this.count_x = 1;
		this.count_y = 1;
		this.select_index = -1;
		this.output_format = "1";
	}

	public override void set_output_format()
	{
		if (this.digits < 1)
		{
			this.digits = 1;
		}
		string text = new string("0"[0], this.digits);
		this.output_format = this.format.Replace("%x", this.start_x.ToString(text));
		this.output_format = this.output_format.Replace("%y", this.start_y.ToString(text));
		this.output_format = this.output_format.Replace("%n", this.start_n.ToString(text));
	}

	public override bool strip_file()
	{
		string text = new string("0"[0], this.digits);
		string text2 = this.format.Replace("%x", this.start_x.ToString(text));
		text2 = text2.Replace("%y", this.start_y.ToString(text));
		text2 = text2.Replace("%n", this.start_n.ToString(text));
		bool arg_C5_0;
		if (this.path_full.Length == 0)
		{
			arg_C5_0 = false;
		}
		else
		{
			this.path = Path.GetDirectoryName(this.path_full);
			this.filename = Path.GetFileNameWithoutExtension(this.path_full);
			this.filename = this.filename.Replace(text2, string.Empty);
			this.extension = Path.GetExtension(this.path_full);
			arg_C5_0 = true;
		}
		return arg_C5_0;
	}

	public override void strip_name()
	{
		string text = new string("0"[0], this.digits);
		string text2 = this.format.Replace("%x", this.start_x.ToString(text));
		text2 = text2.Replace("%y", this.start_y.ToString(text));
		text2 = text2.Replace("%n", this.start_n.ToString(text));
		this.name = this.fullname;
		if (text2.Length > 0)
		{
			this.name = this.name.Replace(text2, string.Empty);
		}
	}

	public override string get_file(int count_x, int count_y, int count_n)
	{
		string text = new string("0"[0], this.digits);
		string text2 = this.format.Replace("%x", (count_x + this.start_x).ToString(text));
		text2 = text2.Replace("%y", (count_y + this.start_y).ToString(text));
		text2 = text2.Replace("%n", (count_n + this.start_n).ToString(text));
		return this.path + "/" + this.filename + text2 + this.extension;
	}

	public override string get_name(int count_x, int count_y, int count_n)
	{
		string text = new string("0"[0], this.digits);
		string text2 = this.format.Replace("%x", (count_x + this.start_x).ToString(text));
		text2 = text2.Replace("%y", (count_y + this.start_y).ToString(text));
		text2 = text2.Replace("%n", (count_n + this.start_n).ToString(text));
		return this.name + text2;
	}
}

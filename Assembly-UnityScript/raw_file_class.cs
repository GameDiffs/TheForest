using System;
using System.IO;
using UnityEngine;

[Serializable]
public class raw_file_class
{
	public bool assigned;

	public bool created;

	public string file;

	public string filename;

	public raw_mode_enum mode;

	public int length;

	public Vector2 resolution;

	public bool square;

	public bool loaded;

	public bool linked;

	public byte[] bytes;

	public FileStream fs;

	public float product1;

	public float product2;

	public raw_file_class()
	{
		this.created = true;
		this.file = string.Empty;
		this.filename = string.Empty;
		this.mode = raw_mode_enum.Windows;
		this.square = true;
		this.linked = true;
	}

	public override bool exists()
	{
		FileInfo fileInfo = new FileInfo(this.file);
		return fileInfo.Exists;
	}
}

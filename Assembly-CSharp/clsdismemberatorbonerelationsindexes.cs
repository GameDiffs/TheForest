using System;

[Serializable]
public class clsdismemberatorbonerelationsindexes
{
	public clsdismemberatorindexer propparentside;

	public clsdismemberatorindexer propchildrenside;

	public clsdismemberatorbonerelationsindexes()
	{
		this.propparentside = new clsdismemberatorindexer();
		this.propchildrenside = new clsdismemberatorindexer();
	}
}

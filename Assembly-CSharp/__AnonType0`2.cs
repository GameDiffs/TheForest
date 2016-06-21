using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class <>__AnonType0<<a>__T, <value>__T>
{
	private readonly <a>__T <a>;

	private readonly <value>__T <value>;

	public <a>__T a
	{
		get
		{
			return this.<a>;
		}
	}

	public <value>__T value
	{
		get
		{
			return this.<value>;
		}
	}

	[DebuggerHidden]
	public <>__AnonType0(<a>__T a, <value>__T value)
	{
		this.<a> = a;
		this.<value> = value;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		var <>__AnonType = obj as <>__AnonType0<<a>__T, <value>__T>;
		return <>__AnonType != null && EqualityComparer<<a>__T>.Default.Equals(this.<a>, <>__AnonType.<a>) && EqualityComparer<<value>__T>.Default.Equals(this.<value>, <>__AnonType.<value>);
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		int num = ((-2128831035 ^ EqualityComparer<<a>__T>.Default.GetHashCode(this.<a>)) * 16777619 ^ EqualityComparer<<value>__T>.Default.GetHashCode(this.<value>)) * 16777619;
		num += num << 13;
		num ^= num >> 7;
		num += num << 3;
		num ^= num >> 17;
		return num + (num << 5);
	}

	[DebuggerHidden]
	public override string ToString()
	{
		string[] expr_06 = new string[6];
		expr_06[0] = "{";
		expr_06[1] = " a = ";
		int arg_46_1 = 2;
		string arg_46_2;
		if (this.<a> != null)
		{
			<a>__T <a>__T = this.<a>;
			arg_46_2 = <a>__T.ToString();
		}
		else
		{
			arg_46_2 = string.Empty;
		}
		expr_06[arg_46_1] = arg_46_2;
		expr_06[3] = ", value = ";
		int arg_7F_1 = 4;
		string arg_7F_2;
		if (this.<value> != null)
		{
			<value>__T <value>__T = this.<value>;
			arg_7F_2 = <value>__T.ToString();
		}
		else
		{
			arg_7F_2 = string.Empty;
		}
		expr_06[arg_7F_1] = arg_7F_2;
		expr_06[5] = " }";
		return string.Concat(expr_06);
	}
}

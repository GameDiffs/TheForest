using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class <>__AnonType3<<Name>__T, <Components>__T>
{
	private readonly <Name>__T <Name>;

	private readonly <Components>__T <Components>;

	public <Name>__T Name
	{
		get
		{
			return this.<Name>;
		}
	}

	public <Components>__T Components
	{
		get
		{
			return this.<Components>;
		}
	}

	[DebuggerHidden]
	public <>__AnonType3(<Name>__T Name, <Components>__T Components)
	{
		this.<Name> = Name;
		this.<Components> = Components;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		var <>__AnonType = obj as <>__AnonType3<<Name>__T, <Components>__T>;
		return <>__AnonType != null && EqualityComparer<<Name>__T>.Default.Equals(this.<Name>, <>__AnonType.<Name>) && EqualityComparer<<Components>__T>.Default.Equals(this.<Components>, <>__AnonType.<Components>);
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		int num = ((-2128831035 ^ EqualityComparer<<Name>__T>.Default.GetHashCode(this.<Name>)) * 16777619 ^ EqualityComparer<<Components>__T>.Default.GetHashCode(this.<Components>)) * 16777619;
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
		expr_06[1] = " Name = ";
		int arg_46_1 = 2;
		string arg_46_2;
		if (this.<Name> != null)
		{
			<Name>__T <Name>__T = this.<Name>;
			arg_46_2 = <Name>__T.ToString();
		}
		else
		{
			arg_46_2 = string.Empty;
		}
		expr_06[arg_46_1] = arg_46_2;
		expr_06[3] = ", Components = ";
		int arg_7F_1 = 4;
		string arg_7F_2;
		if (this.<Components> != null)
		{
			<Components>__T <Components>__T = this.<Components>;
			arg_7F_2 = <Components>__T.ToString();
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

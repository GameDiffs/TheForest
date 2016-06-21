using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class <>__AnonType2<<Type>__T, <List>__T>
{
	private readonly <Type>__T <Type>;

	private readonly <List>__T <List>;

	public <Type>__T Type
	{
		get
		{
			return this.<Type>;
		}
	}

	public <List>__T List
	{
		get
		{
			return this.<List>;
		}
	}

	[DebuggerHidden]
	public <>__AnonType2(<Type>__T Type, <List>__T List)
	{
		this.<Type> = Type;
		this.<List> = List;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		var <>__AnonType = obj as <>__AnonType2<<Type>__T, <List>__T>;
		return <>__AnonType != null && EqualityComparer<<Type>__T>.Default.Equals(this.<Type>, <>__AnonType.<Type>) && EqualityComparer<<List>__T>.Default.Equals(this.<List>, <>__AnonType.<List>);
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		int num = ((-2128831035 ^ EqualityComparer<<Type>__T>.Default.GetHashCode(this.<Type>)) * 16777619 ^ EqualityComparer<<List>__T>.Default.GetHashCode(this.<List>)) * 16777619;
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
		expr_06[1] = " Type = ";
		int arg_46_1 = 2;
		string arg_46_2;
		if (this.<Type> != null)
		{
			<Type>__T <Type>__T = this.<Type>;
			arg_46_2 = <Type>__T.ToString();
		}
		else
		{
			arg_46_2 = string.Empty;
		}
		expr_06[arg_46_1] = arg_46_2;
		expr_06[3] = ", List = ";
		int arg_7F_1 = 4;
		string arg_7F_2;
		if (this.<List> != null)
		{
			<List>__T <List>__T = this.<List>;
			arg_7F_2 = <List>__T.ToString();
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

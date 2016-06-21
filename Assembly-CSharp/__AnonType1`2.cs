using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class <>__AnonType1<<priority>__T, <info>__T>
{
	private readonly <priority>__T <priority>;

	private readonly <info>__T <info>;

	public <priority>__T priority
	{
		get
		{
			return this.<priority>;
		}
	}

	public <info>__T info
	{
		get
		{
			return this.<info>;
		}
	}

	[DebuggerHidden]
	public <>__AnonType1(<priority>__T priority, <info>__T info)
	{
		this.<priority> = priority;
		this.<info> = info;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		var <>__AnonType = obj as <>__AnonType1<<priority>__T, <info>__T>;
		return <>__AnonType != null && EqualityComparer<<priority>__T>.Default.Equals(this.<priority>, <>__AnonType.<priority>) && EqualityComparer<<info>__T>.Default.Equals(this.<info>, <>__AnonType.<info>);
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		int num = ((-2128831035 ^ EqualityComparer<<priority>__T>.Default.GetHashCode(this.<priority>)) * 16777619 ^ EqualityComparer<<info>__T>.Default.GetHashCode(this.<info>)) * 16777619;
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
		expr_06[1] = " priority = ";
		int arg_46_1 = 2;
		string arg_46_2;
		if (this.<priority> != null)
		{
			<priority>__T <priority>__T = this.<priority>;
			arg_46_2 = <priority>__T.ToString();
		}
		else
		{
			arg_46_2 = string.Empty;
		}
		expr_06[arg_46_1] = arg_46_2;
		expr_06[3] = ", info = ";
		int arg_7F_1 = 4;
		string arg_7F_2;
		if (this.<info> != null)
		{
			<info>__T <info>__T = this.<info>;
			arg_7F_2 = <info>__T.ToString();
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

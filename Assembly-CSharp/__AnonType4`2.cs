using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class <>__AnonType4<<Identifier>__T, <Component>__T>
{
	private readonly <Identifier>__T <Identifier>;

	private readonly <Component>__T <Component>;

	public <Identifier>__T Identifier
	{
		get
		{
			return this.<Identifier>;
		}
	}

	public <Component>__T Component
	{
		get
		{
			return this.<Component>;
		}
	}

	[DebuggerHidden]
	public <>__AnonType4(<Identifier>__T Identifier, <Component>__T Component)
	{
		this.<Identifier> = Identifier;
		this.<Component> = Component;
	}

	[DebuggerHidden]
	public override bool Equals(object obj)
	{
		var <>__AnonType = obj as <>__AnonType4<<Identifier>__T, <Component>__T>;
		return <>__AnonType != null && EqualityComparer<<Identifier>__T>.Default.Equals(this.<Identifier>, <>__AnonType.<Identifier>) && EqualityComparer<<Component>__T>.Default.Equals(this.<Component>, <>__AnonType.<Component>);
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		int num = ((-2128831035 ^ EqualityComparer<<Identifier>__T>.Default.GetHashCode(this.<Identifier>)) * 16777619 ^ EqualityComparer<<Component>__T>.Default.GetHashCode(this.<Component>)) * 16777619;
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
		expr_06[1] = " Identifier = ";
		int arg_46_1 = 2;
		string arg_46_2;
		if (this.<Identifier> != null)
		{
			<Identifier>__T <Identifier>__T = this.<Identifier>;
			arg_46_2 = <Identifier>__T.ToString();
		}
		else
		{
			arg_46_2 = string.Empty;
		}
		expr_06[arg_46_1] = arg_46_2;
		expr_06[3] = ", Component = ";
		int arg_7F_1 = 4;
		string arg_7F_2;
		if (this.<Component> != null)
		{
			<Component>__T <Component>__T = this.<Component>;
			arg_7F_2 = <Component>__T.ToString();
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

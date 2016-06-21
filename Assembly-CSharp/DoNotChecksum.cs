using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DoNotChecksum : Attribute
{
}

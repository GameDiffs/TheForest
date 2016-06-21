using System;

namespace LitJson
{
	public delegate TValue ImporterFunc<TJson, TValue>(TJson input);
}

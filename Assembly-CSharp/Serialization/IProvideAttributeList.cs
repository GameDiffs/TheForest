using System;
using System.Collections.Generic;

namespace Serialization
{
	public interface IProvideAttributeList
	{
		bool AllowAllSimple(Type tp);

		IEnumerable<string> GetAttributeList(Type tp);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Cshongo
{
	[AttributeUsage(AttributeTargets.Property)]
	public class EnsureLowercaseAttribute : Attribute
	{
		/// <summary>
		/// Applying this attribute to a database property will ensure its always converted to a lowercase string
		/// </summary>
		public EnsureLowercaseAttribute( )
		{
		}
	}
}
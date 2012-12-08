using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Cshongo
{
	[AttributeUsage( AttributeTargets.Property )]
	public class EnsureIndexAttribute : Attribute
	{
		/// <summary>
		/// Applying this attribute to a database property will always ensure there's a db index built for this field
		/// </summary>
		public EnsureIndexAttribute( )
		{
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Cshongo
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CleanModelAttribute : Attribute
	{
		/// <summary>
		/// Applying this attribute to a repository class will ensure all IDataModels passed to the functions are cleaned before being processed
		/// NOTE: Automatically do this won't actually happen unless i can get some AOP up in here
		/// </summary>
		public CleanModelAttribute( )
		{
		}
	}
}
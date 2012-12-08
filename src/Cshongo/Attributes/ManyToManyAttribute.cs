using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Cshongo
{
	/// <summary>
	/// Defines a ManyToMany relationship between this class and another class
	/// </summary>
	[AttributeUsage( AttributeTargets.Class )]
	public class ManyToManyAttribute : Attribute
	{
		public Type PkType { get; set; }
		public Type FkType { get; set; }

		/// <summary>
		/// Name of the Primary Key Id field
		/// </summary>
		public string PkId { get; set; }
		/// <summary>
		/// Name of the Foreign Key Id field
		/// </summary>
		public string FkId { get; set; }
		
		public ManyToManyAttribute( Type s, Type d )
		{
			PkType = s; 
			FkType = d;

			PkId = s.Name + "Id";
			FkId = d.Name + "Id";
		}
	}
}
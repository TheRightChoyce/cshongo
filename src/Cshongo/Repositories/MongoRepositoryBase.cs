using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Cshongo;
using Cshongo.Interface;

namespace Cshongo
{
	/// <summary>
	/// Just sets up everything we need for a mongo repository, but doesn't define any CRUD methods
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MongoRepositoryBase<T>
		where T : IDataModel, new( )
	{
		#region Accessors
		/// <summary>
		/// Internal session
		/// </summary>
		protected Session _session { get; set; }
		public MongoCollection<T> collection { get; set; }

		/// <summary>
		/// Type/prefix to use for url hashing. If this isn't defined we won't use any url hashing
		/// </summary>
		public string UriHashType { get; set; }
		protected UriHashRepository UriHashing { get; set; }
		protected bool UriHashEnabled { get { return !string.IsNullOrWhiteSpace( UriHashType ); } }

		/// <summary>
		/// If true, each item will generage a url slug based off the item's name when saved
		/// </summary>
		public bool UriSlugEnabled { get; set; }

		/// <summary>
		/// Gets the current session we should use for database access
		/// </summary>
		/// <returns></returns>
		protected Session GetSession( )
		{
			return _session ?? new Session( );
		}
		#endregion

		#region Constructors

		public MongoRepositoryBase( )
		{
			collection = GetSession( ).Col<T>( );
			UriHashing = new UriHashRepository( );
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Helper for getting a lowered string value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected string _lower( string value )
		{
			if (string.IsNullOrWhiteSpace( value ))
				return "";
			return value.ToLowerInvariant( );
		}

		#endregion

		#region Uri Hash

		/// <summary>
		/// Gets the next url hash for this type of object
		/// </summary>
		/// <returns></returns>
		public string GetNextUriHash( )
		{
			if (string.IsNullOrWhiteSpace( UriHashType ))
				return "";
			if (UriHashing == null)
				UriHashing = new UriHashRepository( _session, UriHashType );
			return UriHashing.GetNextUriHash( );
		}

		#endregion

	}
}
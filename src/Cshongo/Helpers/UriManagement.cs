using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using Cshongo.Models;

namespace Cshongo
{
	public class UriHash : MongoDataModel
	{
		public int Index { get; set; }
		public string Type { get; set; }
		public string Hash { get; set; }
	}

	/// <summary>
	/// Helper repository for easily creating and maintaining url hashes
	/// </summary>
	public class UriHashRepository
	{
		/// <summary>
		/// The type/prefix to use for the hash.. ie http://sho.rt/t/abcxd where 't' would be the type
		/// </summary>
		protected string Type { get; set; }

		/// <summary>
		/// Internal connection to the database
		/// </summary>
		protected Session _db { get; set; }

		#region Constructors
		/// <summary>
		/// Creates a new repository with an empty type/prefix
		/// </summary>
		public UriHashRepository( ) : this( new Session(), "" ) { }

		public UriHashRepository( Session session ) : this( session, "" ) { }

		/// <summary>
		/// Creates a new repository using the specified type/prefix
		/// </summary>
		/// <param name="type"></param>
		public UriHashRepository( 
			Session session
			, string type
			)
		{
			Type = type;
			_db = session;
		}
		#endregion

		#region Uri hash
		/// <summary>
		/// Returns the next available hashin our custom base52 format
		/// </summary>
		/// <returns></returns>
		public string GetNextUriHash( )
		{
			var repo = _db.Col<UriHash>( );
			var baseChars = "0123456789bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ".ToCharArray( );
			var current = repo.FindOne( Query.EQ( "Type", Type ) );
			#region create if null
			if (current == null)
			{
				current = new UriHash( )
				{
					_id = DBHelpers.NewId(),
					Type = Type,
					Index = 0,
					Hash = "0"
				};
				repo.Save( current );
			}
			#endregion

			current.Index++;

			#region some math & char buffer magic
			int i = 32;
			char[] buffer = new char[i];
			int targetBase = baseChars.Length;
			var value = current.Index;

			do
			{
				buffer[--i] = baseChars[value % targetBase];
				value = value / targetBase;
			}
			while (value > 0);

			char[] result = new char[32 - i];
			Array.Copy( buffer, i, result, 0, 32 - i );
			#endregion
			current.Hash = new string( result );

			
			repo.Save( current );
			return current.Hash;
		}
		#endregion
	}
}
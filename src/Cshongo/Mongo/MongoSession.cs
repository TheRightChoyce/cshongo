using System;
using System.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Cshongo.Interface;

namespace Cshongo
{
	public class Session
	{
		private MongoServer _server;
		public MongoDatabase db { get; set; }
		public string DatabaseName { get; set; }

		#region Constructors
		/// <summary>
		/// Connects to a mongo server using the information provided in the config file
		/// </summary>
		public Session( ) : this (
				ConfigurationManager.ConnectionStrings["MongoDB_Master"].ToString( ),
				ConfigurationManager.AppSettings["MongoDB_MasterDB_Name"].ToString( )
			) { }
		
		/// <summary>
		/// Connects to the server with the provided connection strings
		/// </summary>
		/// <param name="connectionString"></param>
		public Session( string connectionString )
		{
            _server = new MongoClient(connectionString).GetServer();
			db = _server.GetDatabase( DatabaseName );
		}

		/// <summary>
		/// Connects to the server with the provided connection strings
		/// </summary>
		/// <param name="connectionString"></param>
		public Session( string connectionString, string database )
		{
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("Mongo connection string is empty");
            if (string.IsNullOrWhiteSpace(database))
                throw new Exception("Mongo database string is empty");

            _server = new MongoClient(connectionString).GetServer();
			db = _server.GetDatabase( database );
		}
		/// <summary>
		/// Connects to the server and then gets the database based off the provided settings
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="settings"></param>
		public Session( string connectionString, MongoDatabaseSettings settings )
		{
            _server = new MongoClient(connectionString).GetServer();
			db = _server.GetDatabase( settings );
		}
		#endregion

		#region Helpers

		/// <summary>
		/// Gets a lowered version of this string value
		/// </summary>
		/// <param name="value">any string value to lower</param>
		/// <returns></returns>
		public string GetLowered( string value )
		{
			if (string.IsNullOrWhiteSpace( value ))
				return "";
			return value.ToLowerInvariant( );
		}
		
		/// <summary>
		/// Gets a single collection from this database
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="col"></param>
		/// <returns></returns>
		public MongoCollection<T> Col<T>( ) where T : IDataModel, new( )
		{
			return db.GetCollection<T>( new T( ).GetType( ).Name );
		}
		#endregion
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Reflection;
using System.Reflection.Emit;
using Cshongo.Interface;

namespace Cshongo.Models
{
    /// <summary>
    /// Contains a few static "factory" like meothds for mapping and ease of use
    /// </summary>
	public static class Factory
	{
		/// <summary>
		/// Maps a bson source document back to this object type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T MapFromDb<T>( BsonDocument source )
					where T : Models.MongoDataModel, new( )
		{
			return MapFromDbUsingBson<T>( source );
		}

		#region Map from bson document

		/// <summary>
		/// Attempted this first because MapTo was quicker with Reflection, but tests showed using the Bson converter was MUCH faster in this case.
		/// Also could not verift completion, so this is left for completeness as well
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T MapFromDbUsingReflection<T>( BsonDocument source )
					where T : Models.MongoDataModel, new( )
		{
			var item = new T( );
			var props = item.GetType( ).GetProperties( );
			BsonValue val;
			foreach (var p in props)
			{
				if (source.TryGetValue( p.Name, out val ))
				{
					switch (p.PropertyType.Name.ToLower( ))
					{
						case "datetime":
							p.SetValue( item, val.AsDateTime, null ); break;
						case "list`1":
							var pType = p.PropertyType;
							if (pType.IsGenericType && pType.GetGenericTypeDefinition( ) == typeof( List<> ))
							{
								var type = pType.GetGenericArguments( )[0];
								var listType = typeof( List<> ).MakeGenericType( type );
								IList itemList = (IList)Activator.CreateInstance( listType );
								foreach (var arrayItem in val.AsBsonArray)
								{
									if (arrayItem.IsBoolean)
										itemList.Add( arrayItem.AsBoolean );
									else if (/*arrayItem.IsBsonDateTime ||*/ arrayItem.IsDateTime)
										itemList.Add( arrayItem.AsDateTime );
									else if (arrayItem.IsDouble)
										itemList.Add( arrayItem.AsDouble );
									else if (arrayItem.IsGuid)
										itemList.Add( arrayItem.AsGuid );
									else if (arrayItem.IsInt32)
										itemList.Add( arrayItem.AsInt32 );
									else if (arrayItem.IsInt64)
										itemList.Add( arrayItem.AsInt64 );
									else if (arrayItem.IsObjectId)
										itemList.Add( arrayItem.AsObjectId );
									else if (arrayItem.IsString)
										itemList.Add( arrayItem.AsString );
								}

								p.SetValue( item, itemList, null );
							}
							break;
						default:
							p.SetValue( item, val.RawValue, null ); break;
					}
				}
			}
			return item;
		}

		/// <summary>
		/// Maps from a document into a typed object.. turns out to be super quick!
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T MapFromDbUsingBson<T>( BsonDocument source )
					where T : Models.MongoDataModel, new( )
		{
			return BsonSerializer.Deserialize<T>( source );
		}

		#endregion		
	}

	/// <summary>
	/// This is a base model you should for all your data models when using mongodb
	/// </summary>
	[Serializable, BsonIgnoreExtraElements]
	public class MongoDataModel : IDataModel
	{
        /// <summary>
        /// The mongoId of this item
        /// </summary>
        [BsonId]
        public ObjectId _id { get; set; }
        
        /// <summary>
        /// Provides a string friendly Id for this object
        /// </summary>
        [BsonIgnore]
        public string Id
        {
            get { return _id == ObjectId.Empty ? "" : _id.ToString(); }
            set { _id = ObjectId.Parse(value); }
        }

        /// <summary>
        /// Meta value for if this data model has already been "cleaned" or not
        /// </summary>
        [BsonIgnore]
        public string _cleaned { get; set; }
		
        // need to put this here so auto url-slugs will work
		/// <summary>
		/// If you want to apply attributes to this field, simply override it in your data model (keyword 'new' won't work)
		/// </summary>
		[BsonIgnoreIfNull]
		public virtual string Name { get; set; }
		
        #region Url helpers for publicly accessable objects
		
        /// <summary>
        /// This provides a place to store a bitly style url hash.. e.g. http://mysi.te/chosfHGy
        /// </summary>
        [BsonIgnoreIfNull]
		public string UriHash { get; set; }
		/// <summary>
		/// Provides a url friendly slug for this model based on the name.. e.g. if the name is "this is some item" then the slug would be "this-is-some-item"
		/// </summary>
        [BsonIgnoreIfNull]
		public string UriSlug { get; set; }

        #endregion

        // Meta
        /// <summary>
        /// Date we last updated this object --always store in UTC!
        /// We don't use the mongo utc here in case we need to pass a date value through an API to another source (we then have server time and not the user's local time)
        /// </summary>
        [BsonIgnoreIfNull] 
        public DateTime LastUpdateOnUtc { get; set; }

		/// <summary>
		/// A flag for when this item was deleted -- TODO: Currently not use this and always doing a hard delete
		/// </summary>
        [BsonIgnoreIfNull] 
        public DateTime? DateDeletedUtc { get; set; }


		#region Constructor
		public MongoDataModel( )
		{
			LastUpdateOnUtc = DateTime.UtcNow;
		}
		#endregion

		/// <summary>
		/// Serializes this object into a valid BsonDocument
		/// </summary>
		/// <returns></returns>
		public BsonDocument MapToDB( )
		{
			return MapToDBUsingToBson( );
		}

		#region Map to data model

		/// <summary>
		/// Maps to a document using reflection. 
		/// I found this to be slightly quicker than using 'BsonSerializer.Deserialize', but wasn't able to verify completeness and defaulted to BSON instead
		/// </summary>
		/// <returns></returns>
		public BsonDocument MapToDBUsingReflection( )
		{
			var props = this.GetType( ).GetProperties( );
			var doc = new BsonDocument( );

			foreach (var p in props)
			{
				if (p.Name == "Id") // skip the Id element since its just a wrapper
					continue;

				BsonElement ele;
				if (p.PropertyType.Name == "DateTime")
					ele = new BsonElement( p.Name, BsonDateTime.Create( p.GetValue( this, null ) ) );
				else
					ele = new BsonElement( p.Name, BsonValue.Create( p.GetValue( this, null ) ) );
				doc.Add( ele );
			}

			return doc;
		}

		public BsonDocument MapToDBUsingToBson( )
		{
			return BsonSerializer.Deserialize<BsonDocument>
			(
				this.ToBson( )
			);
		}
		#endregion
	}

    /*
	/// <summary>
	/// Base model for data models that keep an internal count to them
	/// </summary>
	[Serializable, BsonIgnoreExtraElements]
	public class MongoCountedDataModel : MongoDataModel, ICountedDataModel
	{
		[DefaultValue(1)] // must have at least a count of one, otherwise this object shouldn't exist
		public int Count { get; set; }
		
		public MongoCountedDataModel( )
		{
			Count = 1;
		}
	}

	/// <summary>
	/// Base model for data models that keep an internal count to them
	/// </summary>
	[Serializable, BsonIgnoreExtraElements]
	public class MongoCountedUserItemDataModel : MongoCountedDataModel, ICountedUserItemDataModel
	{
		/// <summary>
		/// If you want to apply attributes to this field, simply 'override' it in your data model (keyword 'new' won't work)
		/// </summary>
		[Index, EnsureLowercase]
		public virtual string UserName { get; set; }

		public MongoCountedUserItemDataModel( )
		{
		}
	}
    */
}
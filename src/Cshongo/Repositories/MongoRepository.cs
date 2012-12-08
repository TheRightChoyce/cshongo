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
	public class MongoRepository<T> 
		: MongoRepositoryBase<T>, IRepository<T> 
		where T : IDataModel, new( )
	{
		#region Generic Methods

		/// <summary>
		/// Gets all active items in the database. This should be used as a baseline for all queries
		/// </summary>
		/// <returns></returns>
		public IQueryable<T> Get( )
		{
            //var query = Query.EQ( "DateDeletedUtc", null );
            return collection.FindAll()
                    .SetSortOrder(SortBy.Ascending("CreatedOnUtc"))
                    .AsQueryable();
		}
		
		/// <summary>
		/// Get a single item by id
		/// </summary>
		/// <typeparam name="T">The type/collection to use</typeparam>
		/// <param name="id">string id for this element</param>
		/// <returns></returns>
		public T Get( string id )
		{
			return collection.FindOneById( ObjectId.Parse(id) );
		}

		/// <summary>
		/// Get a single item by a pk/fk pair
		/// </summary>
		/// <param name="id">primary key to search by</param>
		/// <param name="fkId">foreign key to seach by</param>
		/// <returns></returns>
		public T Get( string id, string fkId )
		{
			var info = typeof(T);
			var attributes = info.GetCustomAttributes( typeof(ManyToManyAttribute), false );

			if (attributes.Count( ) == 0)
				throw new Exception( "Get(id, fkid) can not be called on '" + info.Name + "' because it is not defined with a ManyToMany attribute" );
			
			// grab the attribute and return based on the pk/fk pair
			var attr = attributes.First( ) as ManyToManyAttribute;
			return collection.FindOne( Query.And(
				Query.EQ( attr.PkId, id )
				, Query.EQ( attr.FkId, fkId )
			) );
		}

		/// <summary>
		/// Gets a single item by a non-key field
		/// </summary>
		/// <param name="field"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public T GetByField( string field, string value )
		{
			return collection.FindOne( Query.EQ( field, value ) );
		}

		/// <summary>
		/// Creates a new item from the provider document
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		public T Create( T item )
		{
			item = DBHelpers.PrepareModel( item );
			return CreateCleaned( item );
		}

		/// <summary>
		/// Creates an item without calling any of the helper method cleaners
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected T CreateCleaned( T item )
		{
            // Check to see if some meta values are enabled
            if (UriHashEnabled && string.IsNullOrWhiteSpace(item.UriHash))
                item.UriHash = UriHashing.GetNextUriHash();
            if (UriSlugEnabled && !string.IsNullOrWhiteSpace(item.UriSlug))
                item.UriSlug = item.UriSlug.Slugify();

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                item._id = DBHelpers.NewId();
                item.LastUpdateOnUtc = item._id.CreationTime;
            }
            else
                item.LastUpdateOnUtc = DateTime.UtcNow;
			
            var result = collection.Insert( item );
			if (result != null && !result.Ok)
				throw new Exception( result.ErrorMessage );
			return item;
		}

		/// <summary>
		/// Updates the provided document in the collection; returns false if update fails or item isn't found
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		public bool Update( T item )
		{
			item = DBHelpers.PrepareModel( item );
			if (string.IsNullOrWhiteSpace( item.Id )) // exit if no id
				return false;
			if (UriSlugEnabled && !string.IsNullOrWhiteSpace( item.UriSlug ))
				item.UriSlug = item.UriSlug.Slugify( );
			item.LastUpdateOnUtc = DateTime.UtcNow;
			collection.Save( item );
			return true;
		}

		/// <summary>
		/// Marks on item as deleted in the database
		/// </summary>
		/// <param name="id"></param>
		public bool Delete( string id )
		{
			if( string.IsNullOrWhiteSpace( id ) )
				return false;
			var item = collection.FindOneById( ObjectId.Parse( id ) );
			if (item == null)
				return false;
			// TODO: Enable attribute for hard/soft delete
            //item.DateDeletedUtc = DateTime.UtcNow;
			//collection.Save( item );

            collection.Remove( Query.EQ( "_id", ObjectId.Parse( id ) ) );
			return true;
		}
		#endregion
	}
}
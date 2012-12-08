using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cshongo.Interface
{
	/// <summary>
	/// CRUD interface with a few helper functions
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRepository<T> where T : IDataModel
	{
		#region Generic Methods
		
		/// <summary>
		/// Gets all items
		/// </summary>
		/// <returns></returns>
		IQueryable<T> Get( );
		
		/// <summary>
		/// Gets a single item
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		T Get( string id );

		/// <summary>
		/// Gets a single item from a pk/fk key pair. This will throw an exception 
		/// if the class isn't defined with a [ManyToMany] attribute
		/// </summary>
		/// <param name="id"></param>
		/// <param name="fkId"></param>
		/// <returns></returns>
		T Get( string id, string fkId );

		/// <summary>
		/// Gets a single item by searching on a specific non-id field. Please ensure 
		/// this field is marked with [Index] or you'll be sorry
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		T GetByField( string fieldName, string value );
		
		/// <summary>
		/// Creates and returns a new item
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Create( T item );
		
		/// <summary>
		/// Updates an existing item
		/// </summary>
		/// <typeparam name="T"></typeparam>
		bool Update( T item );

		/// <summary>
		/// Deletes this item from the repository; returns false if the item wasn't found
		/// </summary>
		/// <param name="id"></param>
		bool Delete( string id );
		
		#endregion
	}
    
    /*
	/// <summary>
	/// Interface for data models that have a 'Count' field
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ICountedRepository<T> : IRepository<T>
		where T : ICountedDataModel
	{
		/// <summary>
		/// Creates this item id needed, otherwise it will update the count on the existing item
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		new T Create( T item );

		/// <summary>
		/// This is the string name of the field which will be used when aggregating the count
		/// </summary>
		string PrimaryCountField { get; set; }

		/// <summary>
		/// Gets an object by its primary counting field value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		T GetByPrimaryField( string value );
		
		/// <summary>
		/// Either decrements the count on this item, or removes it entirely if the current count is 1
		/// </summary>
		/// <param name="item"></param>
		bool Remove( string fieldValue );
	}
    */
}

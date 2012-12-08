using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver;
using Cshongo.Interface;

namespace Cshongo
{
	public static class DBHelpers
	{
		#region Helpers to see if this model is/isn't loaded from the database
		/// <summary>
		/// Returns true if the Id for this dataModel IS set
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static bool IsLoaded( this IDataModel model )
		{
			if (model == null)
				return false;
            return !string.IsNullOrWhiteSpace( model.Id );
		}

		/// <summary>
		/// Returns true if the Id for this dataModel is NOT set
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static bool IsEmpty( this IDataModel model )
		{
			if (model == null)
				return true;
			return string.IsNullOrWhiteSpace( model.Id );
		}
		#endregion

		#region Id related
		/// <summary>
		/// Generates a new id for this object if there currently isn't one set
		/// </summary>
		/// <returns></returns>
		public static void EnsureId( this IDataModel model )
		{
			if (string.IsNullOrWhiteSpace( model.Id ))
                model._id = ObjectId.GenerateNewId();
		}

		/// <summary>
		/// Generates a new id when there isn't currently an object to reference
		/// </summary>
		/// <returns></returns>
		public static ObjectId NewId( )
		{
            return ObjectId.GenerateNewId();
		}
		#endregion

		#region Model specific helpers
		/// <summary>
		/// Prepares this model for the databsae by enforces any attributes, and ensures Id, etc..
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="model"></param>
		/// <returns></returns>
		public static T PrepareModel<T>( T model ) where T : IDataModel
		{
			foreach (var property in model.GetType().GetProperties( ))
			{
                // Ensures an ObjectId is set
                if (property.Name == "_id" && property.GetValue( model, null ) == null )
					property.SetValue( model, NewId( ), null );
				
				// ensure lower case
				var attributes = property.GetCustomAttributes( typeof( EnsureLowercaseAttribute ), false );
				if (attributes.Count( ) == 1)
				{
					var curVal = property.GetValue( model, null );
					if (curVal != null)
					{
						property.SetValue(
							model
							, property.GetValue( model, null ).ToString( ).ToLowerInvariant( )
							, null
						);
					}
				}
				
				// enforce default value
				attributes = property.GetCustomAttributes( typeof( DefaultValueAttribute ), false );
				if (attributes.Count( ) == 1)
				{
					var value = property.GetValue( model, null );
					if (value == null || string.IsNullOrWhiteSpace( value.ToString( ) ))
					{
						property.SetValue(
							model
							, (attributes[0] as DefaultValueAttribute).Value
							, null
						);
					}
				}

                // check for enabled uri hashing
			}
			return model;
		}


		/// <summary>
		/// Converts this property into a string for lookup in mongodb
		/// </summary>
		/// <param name="?"></param>
		/// <returns></returns>
		public static string FieldName<T, R>( this T obj, Expression<Func<T, R>> expr )
		{
			var node = expr.Body as MemberExpression;
			if (object.ReferenceEquals( null, node ))
				throw new InvalidOperationException( "Error, DataModel you're trying to access is null" );
			return node.Member.Name;
		}

		/// <summary>
		/// Gets the value of a property via a string reference to that property's name
		/// </summary>
		/// <param name="model"></param>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static string GetFieldValue( this IDataModel model, string fieldName )
		{
			if (model == null)
				throw new Exception( "GetFieldValue: passed in model is null." );
			var properties = model.GetType( ).GetProperties( );
			if (properties == null)
				return string.Empty;

			var prop = properties.Where( x => x.Name == fieldName ).FirstOrDefault( );
			if( prop == null )
				return string.Empty;

			var val = prop.GetValue( model, null );
			if (val == null )
				return string.Empty;
			
			return val.ToString();
		}
		
		#endregion
	}

	/// <summary>
	/// If we don't have an actual object we can still refer to its type with this generic static class
	/// </summary>
	public static class MembersOf<T>
	{
		public static string FieldName<R>( Expression<Func<T, R>> expr )
		{
			var node = expr.Body as MemberExpression;
			if (object.ReferenceEquals( null, node ))
				throw new InvalidOperationException( "Expression couldn't find a matching member for this class" );
			return node.Member.Name;
		}
	}
}
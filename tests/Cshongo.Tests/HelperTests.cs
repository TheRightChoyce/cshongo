using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit;
using Cshongo;
using Cshongo.Models;
using MongoDB.Bson;

namespace Congo.Tests.DB
{
	[TestFixture]
	public class HelperTests
	{
		[SetUp]
		public static void Init( )
		{
		}

		/// <summary>
		/// Compares bson conversion to an expression for reverse looking up a field value
		/// </summary>
		[Test]
		public void Compare_DataModel_FieldLookup_ByString( )
		{
			var item = new MongoDataModel( )
			{
				Name = "this is the name"
			};

			var now = DateTime.UtcNow;
			for (var i = 0; i < 10000; i++)
			{
				var bson = item.ToBsonDocument( );
				var result = bson["Name"].AsString;
			}
			var bsonDuration = DateTime.UtcNow - now;

			now = DateTime.UtcNow;
			for (var i = 0; i < 10000; i++)
			{
				var result = item.GetFieldValue( "Name" );
			}
			var linqDuration = DateTime.UtcNow - now;

			var linqSeconds = linqDuration.TotalSeconds;
			var bsonSeconds = bsonDuration.TotalSeconds;
			
			//Assert.LessOrEqual( bsonSeconds, linqSeconds, "bson Greater {0} vs {1}", bsonSeconds, linqSeconds );
			Assert.LessOrEqual( linqSeconds, bsonSeconds, "linq Greater {0} vs {1}", linqSeconds, bsonSeconds );
		}

		[Test]
		public void Test_DBHelpers_CleanModel( )
		{
			// TODO
		}

	}
}

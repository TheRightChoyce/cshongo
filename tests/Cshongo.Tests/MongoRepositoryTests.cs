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
	public class MongoRepositoryTests
	{
		MongoRepository<MongoDataModel> repo = new MongoRepository<MongoDataModel>( );

		[SetUp]
		public static void Init( )
		{
			var repo = new MongoRepository<MongoDataModel>( );
			repo.collection.RemoveAll( );
		}

		#region Helpers

		public const string Name = "name";

		public MongoDataModel GetNew( )
		{
			return new MongoDataModel()
			{
				Name = Name
			};
		}

		#endregion

		#region Basic CRUD

        #region Create
        [Test]
		public void Test_Create( )
		{
			var result = repo.Create( GetNew( ) );

			Assert.IsNotNull( result );
			Assert.IsNotNullOrEmpty( result.Id );
			Assert.AreEqual( Name, result.Name );
			
			Assert.IsNotNull( result.LastUpdateOnUtc );
			
            var diff = result.LastUpdateOnUtc - result._id.CreationTime;
            //Assert.GreaterOrEqual(result.LastUpdateOnUtc, result._id.CreationTime);
            Assert.AreEqual(new TimeSpan(0), diff,
                string.Format("create time is: {0}, update time is: {1}"
                    , result._id.CreationTime.ToString()
                    , result.LastUpdateOnUtc.ToString()
                )
            );

            diff = DateTime.UtcNow - result._id.CreationTime;
            Assert.GreaterOrEqual( 2, diff.TotalSeconds ); // allow for a little variance here

			var list = repo.collection.FindAll( );
			Assert.IsNotNull( list );
			Assert.AreEqual( 1, list.Count() );
		}
        
        #endregion

        [Test]
		public void Test_Get_One( )
		{
            var result = repo.Create(GetNew());

            var get = repo.Get(result.Id);
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(result.Name, get.Name);
		}

		[Test]
		public void Test_Get_All( )
		{
            var result1 = repo.Create(GetNew());
            var result2 = repo.Create(GetNew());
            var result3 = repo.Create(GetNew());

            var getAll = repo.Get();
            Assert.IsNotNull(getAll);
            Assert.AreEqual(3, getAll.Count());
            Assert.AreEqual(result1.Id, getAll.First().Id);
            Assert.AreEqual(result3.Id, getAll.Last().Id);

		}

		[Test]
		public void Test_Update( )
		{
            var original = repo.Create(GetNew());

            var originalName = original.Name;
            var originalUpdatedOn = original.LastUpdateOnUtc;
            var newName = "new name";

            original.Name = newName;
            repo.Update(original);

            // Ensure update is correct
            var result = repo.Get(original.Id);
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(result.Id, original.Id);
            Assert.AreNotEqual(result.Name, originalName);
            Assert.AreEqual(newName, result.Name);
            Assert.Greater(result.LastUpdateOnUtc, originalUpdatedOn);
            Assert.Greater(result.LastUpdateOnUtc, result._id.CreationTime);

            // Ensure there's still only one
            var all = repo.Get();
            Assert.AreEqual(1, all.Count());

            // TODO Ensure if multiple, then no other items have changed?
		}

		[Test]
		public void Test_Delete( )
		{
            var original = repo.Create(GetNew());
            var original2 = repo.Create(GetNew());
            
            var result = repo.Delete(original.Id);

            Assert.AreEqual(true, result);

            var all = repo.Get();
            Assert.AreEqual(1, all.Count());
            Assert.AreEqual(original2.Id, all.First().Id);
		}
		#endregion
	}
}

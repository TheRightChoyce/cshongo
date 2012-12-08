using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
namespace Cshongo.Interface
{
	public interface IDataModel
	{
        ObjectId _id { get; set; }
        string Id { get; set; }
		string Name { get; set; }
		string UriHash { get; set; }
		string UriSlug { get; set; }

		DateTime LastUpdateOnUtc { get; set; }
		DateTime? DateDeletedUtc { get; set; }
	}
}
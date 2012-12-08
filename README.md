cshongo
=======

C# Wrapper for MongoDB

Provides a base repo for working with mongo + a suite of helper attributes and ease of use functions for making kickstarting a project even quicker. This initial release was original built around a year ago for one of my startups and has languished on my computer since. I've extracted what I felt was all the generic methods and will continue to refine and refactor until I feel like everything is in the right place.

Examples:

```csharp

namespace MyProject.Models.DataModels
{
  public class MyModel : Cshongo.Models.MongoDataModel
  {
    [EnsureIndex, EnsureLowercase]
    public string User { get; set; } // the user this model belongs to
    
    [EnsureIndex]
    public override string Name { get; set; } // override the base model name field to provide the Index attribute
    
    public DateTime SomeDateUtc { get; set; } // some other field without any attributes
  }
}

namespace MyProject.Core.Repository
{
  public class MyModelRepository : Cshongo.MongoRepository<MyModel>
  {
    public MyModelRepository()
    {
      UriSlugEnabled = true; // automatically creates a slug form "this-is-the-item-name"
    }
  }
}


namespace MyProject
{
  public class MyProjectConsole
  {
    static void Main( string[] args )
    {
      var repo = new MyModelRepository();
      var items = repo.Get(); // gets all items
      
      // do whatever here!
      
    }
  }
}

```
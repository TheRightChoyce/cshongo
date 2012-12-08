using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cshongo
{
    public static class Bootstrapper
    {
        /// <summary>
        /// To bootstrap Cshongo, provide us with a class in your datamodels namespace and we'll ensure indexes and any other startup task
        /// </summary>
        /// <param name="reference"></param>
        public static void Init(Type reference)
        {
            // todo --bootstrap  ensure indexes?
            EnsureIndexes(reference);
        }

        /// <summary>
        /// Ensures all indexes are in place all datamodels in this project
        /// </summary>
        /// <param name="referenceType">The type of any class that exists in your datamodels namespace</param>
        public static void EnsureIndexes(Type referenceClass)
        {
            var mongo = new Session();
            var dataModels = from t in referenceClass.Assembly.GetTypes()
                             where t.IsClass // && t.Namespace.EndsWith("Models.DataModels")
                             select t;

            foreach (var dModel in dataModels)
            {
                foreach (var property in dModel.GetProperties())
                {
                    var attributes = property.GetCustomAttributes(typeof(EnsureIndexAttribute), false);
                    if (attributes.Count() == 0)
                        continue;
                    mongo.db.GetCollection(dModel.Name).EnsureIndex(property.Name);
                }
            }
        }
    }
}

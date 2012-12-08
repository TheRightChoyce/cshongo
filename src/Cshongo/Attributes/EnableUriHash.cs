using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cshongo
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EnableUriHash : Attribute
    {
        /// <summary>
        /// Provides an attribute for enabling the use of Uri Hashes on a model
        /// </summary>
        public EnableUriHash()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cshongo.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EnableHardDelete : Attribute
    {
        /// <summary>
        /// If applied, then documents will be delete from the collection rather than just being marked as deleted
        /// </summary>
        public EnableHardDelete()
        {
        }
    }
}

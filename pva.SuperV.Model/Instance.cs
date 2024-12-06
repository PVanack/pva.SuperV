using pva.SuperV.Model.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pva.SuperV.Model
{
    public class Instance : IInstance
    {
        public string Name { get; set; }

        public Class Class { get; set; }

        //public IField<T> GetField<T>(string name)
        //{
        //    if (!Class.FieldDefinitions.ContainsKey(name))
        //    {
        //        throw new UnknownFieldException(name, Class.Name);
        //    }
        //    return 
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pva.SuperV.Model
{
    public interface IInstance
    {
        public String Name { get; set; }
        //public IField<T> GetField<T>(String name);
    }
}

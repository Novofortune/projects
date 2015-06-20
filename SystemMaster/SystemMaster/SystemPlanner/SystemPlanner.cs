using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMaster.SystemPlanner
{
    class SystemPlanner
    {
        public Dictionary<string, Class> ClassDictionary;
    }
    class Class {
        public string Name { get; set; }
        public Dictionary<string, ClassMember> Members { get; set; }
    }
    class ClassMember{
        public string Name { get; set; }
        public string Type { get; set; }
        public object value { get; set; }
    }
}

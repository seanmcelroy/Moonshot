using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonshot.Common
{
    using System.Xml.Serialization;

    public class PropDirSerializationItem
    {
        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public string value;
    }
}

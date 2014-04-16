namespace Moonshot.Common
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    public class Thing : DynamicObject, ISerializable, IDynamicMetaObjectProvider
    {
        private static readonly char[] Base36Characters = 
                                                        {
                                                            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B',
                                                            'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
                                                            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
                                                        };

        private static readonly string[] _readonlyProperties = { "dirty", "id" };
        private static readonly string[] _serializeSkipProperties = { "dirty", "impl" };
        private static readonly string[] _writeonlyProperties = { "pass" };
        
        public virtual string Id
        {
            get
            {
                return (string)_properties["id"];
            }
        }

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>(10);
        private readonly Dictionary<string, string> _propdirs = new Dictionary<string, string>(10);

        public Thing(string id)
        {
            _properties["id"] = id;
            _properties["owner"] = id; // I own myself.
        }

        public Thing()
        {
            var random = new Random(Environment.TickCount);
            var id10 = random.NextLong(1000,2176782335);

            // 32 is the worst cast buffer size for base 2 and int.MaxValue
            int i = 32;
            var buffer = new char[i];
            var targetBase = Base36Characters.Length;

            do
            {
                buffer[--i] = Base36Characters[id10 % targetBase];
                id10 = id10 / targetBase;
            }
            while (id10 > 0);

            var result = new char[32 - i];
            Array.Copy(buffer, i, result, 0, 32 - i);
            var id = new string(result);
            id = "#" + id.PadLeft(6, '0');

            _properties["id"] = id;
            _properties["owner"] = id; // I own myself.
            _properties["dirty"] = true;
        }

        protected Thing(SerializationInfo info, StreamingContext context)
        {
            var propdirsSerializer = new XmlSerializer(typeof(PropDirSerializationItem[]), new XmlRootAttribute() { ElementName = "propdir" });

            foreach (var property in info)
            {
                if (property.Name == "_propdirs")
                    _propdirs = ((PropDirSerializationItem[])propdirsSerializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(((JValue)property.Value).ToString())))).ToDictionary(i => i.name, i => i.value);
                else
                    _properties.Add(property.Name, property.Value);
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        public override sealed bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_writeonlyProperties.Contains(binder.Name.ToLowerInvariant()))
            {
                result = null;
                return false;
            }
            string name = binder.Name.ToLowerInvariant();

            if (_properties.TryGetValue(name, out result)) return true;
            result = false;
            return true;
        }

        public override sealed bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_readonlyProperties.Contains(binder.Name.ToLowerInvariant())) return false;
            _properties[binder.Name.ToLowerInvariant()] = value;
            _properties["dirty"] = true;
            return true;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var propdirsSerializer = new XmlSerializer(typeof(PropDirSerializationItem[]), new XmlRootAttribute() { ElementName = "propdir" });

            foreach (var key in _properties.Keys)
                if (!_serializeSkipProperties.Contains(key))
                    info.AddValue(key, _properties[key]);

            var ms = new MemoryStream();
            propdirsSerializer.Serialize(ms, _propdirs.Select(kv => new PropDirSerializationItem() { name = kv.Key, value = kv.Value }).ToArray());
            info.AddValue("_propdirs", Encoding.UTF8.GetString(ms.ToArray()));

            _properties["dirty"] = false;
        }
    }
}

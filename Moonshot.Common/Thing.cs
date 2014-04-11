namespace Moonshot.Common
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public class Thing : DynamicObject, ISerializable
    {
        private static readonly char[] Base36Characters = 
                                                        {
                                                            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B',
                                                            'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
                                                            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
                                                        };

        private static readonly string[] _readonlyProperties = { "id" };
        private static readonly string[] _serializeSkipProperties = { "impl" };
        private static readonly string[] _writeonlyProperties = { "pass" };

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>(10);

        public Thing()
        {
            var random = new Random(Environment.TickCount);
            long id10 = 0;
            for (var n = 0; n < 1257; n++)
                id10 += random.Next(0, 2147483647);

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

            _properties["id"] = result;
            _properties["owner"] = result; // I own myself.
        }

        protected Thing(SerializationInfo info, StreamingContext context)
        {
            foreach (var property in info)
                _properties.Add(property.Name, property.Value);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
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

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_readonlyProperties.Contains(binder.Name.ToLowerInvariant())) return false;
            _properties[binder.Name.ToLowerInvariant()] = value;
            return true;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var key in _properties.Keys)
                if (!_serializeSkipProperties.Contains(key))
                    info.AddValue(key, _properties[key]);
        }
    }
}

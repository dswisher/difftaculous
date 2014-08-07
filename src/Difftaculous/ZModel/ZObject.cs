
using System.Collections.Generic;


namespace Difftaculous.ZModel
{
    internal class ZObject : ZToken
    {
        private readonly Dictionary<string, ZProperty> _properties = new Dictionary<string, ZProperty>();


        public override TokenType Type { get { return TokenType.Object; } }


        public ZObject()
        {
        }


        public ZObject(params ZProperty[] properties)
        {
            foreach (var p in properties)
            {
                AddProperty(p);
            }
        }


        public IEnumerable<ZProperty> Properties
        {
            get { return _properties.Values; }
        }


        public ZProperty GetProperty(string name)
        {
            if (name == null)
            {
                return null;
            }

            ZProperty property;
            _properties.TryGetValue(name, out property);
            return property;
        }


        public override ZToken this[string propertyName]
        {
            get
            {
                ZProperty property = GetProperty(propertyName);
                return (property != null) ? property.Value : null;
            }
        }


        public void AddProperty(ZProperty property)
        {
            _properties.Add(property.Name, property);
            property.Parent = this;
        }
    }
}

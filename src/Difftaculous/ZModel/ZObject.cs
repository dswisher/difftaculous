
using System.Collections.Generic;
using Difftaculous.Adapters;

namespace Difftaculous.ZModel
{
    internal class ZObject : ZToken, IObject
    {
        private readonly Dictionary<string, IProperty> _properties = new Dictionary<string, IProperty>();


        public override TokenType Type { get { return TokenType.Object; } }


        public IEnumerable<IProperty> Properties
        {
            get { return _properties.Values; }
        }


        public IProperty GetProperty(string name)
        {
            if (name == null)
            {
                return null;
            }

            IProperty property;
            _properties.TryGetValue(name, out property);
            return property;
        }


        public override IToken this[string propertyName]
        {
            get
            {
                IProperty property = GetProperty(propertyName);
                return (property != null) ? property.Value : null;
            }
        }


        public void AddProperty(IProperty property)
        {
            _properties.Add(property.Name, property);
            ((ZToken)property).Parent = this;
        }
    }
}

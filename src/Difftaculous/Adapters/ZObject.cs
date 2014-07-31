
using System;
using System.Collections.Generic;
using Difftaculous.Paths;

namespace Difftaculous.Adapters
{
    internal class ZObject : IObject
    {
        private readonly List<IProperty> _properties = new List<IProperty>();


        public DiffPath Path
        {
            // TODO - implement path - need to navigate up to parents
            get { throw new NotImplementedException(); }
        }


        public IEnumerable<IProperty> Properties
        {
            get { return _properties; }
        }


        public void AddProperty(IProperty property)
        {
            _properties.Add(property);
        }
    }
}

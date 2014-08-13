﻿
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable DoNotCallOverridableMethodsInConstructor


namespace Difftaculous.ZModel
{
    internal class ZObject : ZContainer, IDictionary<string, ZToken> // , INotifyPropertyChanged, ICustomTypeDescriptor, INotifyPropertyChanging
    {
        private readonly ZPropertyKeyedCollection _properties = new ZPropertyKeyedCollection();


        /// <summary>
        /// Gets the container's children tokens.
        /// </summary>
        /// <value>The container's children tokens.</value>
        protected override IList<ZToken> ChildrenTokens
        {
            get { return _properties; }
        }


#if false
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

#if !(NET20 || NETFX_CORE || PORTABLE || PORTABLE40)
        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;
#endif

#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ZObject"/> class.
        /// </summary>
        public ZObject()
        {
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="ZObject"/> class from another <see cref="ZObject"/> object.
        /// </summary>
        /// <param name="other">A <see cref="ZObject"/> object to copy from.</param>
        public ZObject(ZObject other)
            : base(other)
        {
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="ZObject"/> class with the specified content.
        /// </summary>
        /// <param name="content">The contents of the object.</param>
        public ZObject(params object[] content)
            : this((object)content)
        {
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="ZObject"/> class with the specified content.
        /// </summary>
        /// <param name="content">The contents of the object.</param>
        public ZObject(object content)
        {
            Add(content);
        }



        internal override bool DeepEquals(ZToken node)
        {
            ZObject t = node as ZObject;
            if (t == null)
                return false;

            return _properties.Compare(t._properties);
        }



        internal override void InsertItem(int index, ZToken item, bool skipParentCheck)
        {
            // don't add comments to ZObject, no name to reference comment by
            if (item != null && item.Type == TokenType.Comment)
                return;

            base.InsertItem(index, item, skipParentCheck);
        }



        internal override void ValidateToken(ZToken o, ZToken existing)
        {
            ValidationUtils.ArgumentNotNull(o, "o");

            if (o.Type != TokenType.Property)
            {
                throw new ArgumentException(string.Format("Can not add {0} to {1}.", o.GetType().Name, GetType().Name));
            }

            ZProperty newProperty = (ZProperty)o;

            if (existing != null)
            {
                ZProperty existingProperty = (ZProperty)existing;

                if (newProperty.Name == existingProperty.Name)
                {
                    return;
                }
            }

            if (_properties.TryGetValue(newProperty.Name, out existing))
            {
                throw new ArgumentException(string.Format("Can not add property {0} to {1}. Property with the same name already exists on object.", newProperty.Name, GetType().Name));
            }
        }


#if false
        internal void InternalPropertyChanged(JProperty childProperty)
        {
            OnPropertyChanged(childProperty.Name);
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
            if (_listChanged != null)
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, IndexOfItem(childProperty)));
#endif
#if !(NET20 || NET35 || PORTABLE40)
            if (_collectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, childProperty, childProperty, IndexOfItem(childProperty)));
#endif
        }

        internal void InternalPropertyChanging(JProperty childProperty)
        {
#if !(NET20 || NETFX_CORE || PORTABLE40 || PORTABLE)
            OnPropertyChanging(childProperty.Name);
#endif
        }

        internal override ZToken CloneToken()
        {
            return new ZObject(this);
        }
#endif



        /// <summary>
        /// Gets the node type for this <see cref="ZToken"/>.
        /// </summary>
        /// <value>The type.</value>
        public override TokenType Type
        {
            get { return TokenType.Object; }
        }



        /// <summary>
        /// Gets an <see cref="IEnumerable{ZProperty}"/> of this object's properties.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{ZProperty}"/> of this object's properties.</returns>
        public IEnumerable<ZProperty> Properties()
        {
            return _properties.Cast<ZProperty>();
        }


        /// <summary>
        /// Gets a <see cref="ZProperty"/> the specified name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>A <see cref="ZProperty"/> with the specified name or null.</returns>
        public ZProperty Property(string name, bool caseSensitive = true)
        {
            if (name == null)
            {
                return null;
            }

            ZToken property;
            _properties.TryGetValue(name, out property, caseSensitive);
            return (ZProperty)property;
        }



        /// <summary>
        /// Gets an <see cref="ZEnumerable{ZToken}"/> of this object's property values.
        /// </summary>
        /// <returns>An <see cref="ZEnumerable{ZToken}"/> of this object's property values.</returns>
        public ZEnumerable<ZToken> PropertyValues()
        {
            return new ZEnumerable<ZToken>(Properties().Select(p => p.Value));
        }



        /// <summary>
        /// Gets the <see cref="ZToken"/> with the specified key.
        /// </summary>
        /// <value>The <see cref="ZToken"/> with the specified key.</value>
        public override ZToken this[object key]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(key, "o");

                string propertyName = key as string;
                if (propertyName == null)
                {
                    throw new NotImplementedException();
                    //throw new ArgumentException("Accessed ZObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }

                return this[propertyName];
            }
            set
            {
                ValidationUtils.ArgumentNotNull(key, "o");

                string propertyName = key as string;
                if (propertyName == null)
                {
                    throw new NotImplementedException();
                    // throw new ArgumentException("Set ZObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
                }

                this[propertyName] = value;
            }
        }



        /// <summary>
        /// Gets or sets the <see cref="ZToken"/> with the specified property name.
        /// </summary>
        /// <value></value>
        public ZToken this[string propertyName]
        {
            get
            {
                ValidationUtils.ArgumentNotNull(propertyName, "propertyName");
                ZProperty property = Property(propertyName);
                return (property != null) ? property.Value : null;
            }
            set
            {
#if false
                JProperty property = Property(propertyName);
                if (property != null)
                {
                    property.Value = value;
                }
                else
                {
#if !(NET20 || NETFX_CORE || PORTABLE40 || PORTABLE)
                    OnPropertyChanging(propertyName);
#endif
                    Add(new JProperty(propertyName, value));
                    OnPropertyChanged(propertyName);
                }
#endif
                throw new NotImplementedException();
            }
        }



#if false
        /// <summary>
        /// Loads an <see cref="ZObject"/> from a <see cref="JsonReader"/>. 
        /// </summary>
        /// <param name="reader">A <see cref="JsonReader"/> that will be read for the content of the <see cref="ZObject"/>.</param>
        /// <returns>A <see cref="ZObject"/> that contains the JSON that was read from the specified <see cref="JsonReader"/>.</returns>
        public new static ZObject Load(JsonReader reader)
        {
            ValidationUtils.ArgumentNotNull(reader, "reader");

            if (reader.TokenType == JsonToken.None)
            {
                if (!reader.Read())
                    throw JsonReaderException.Create(reader, "Error reading ZObject from JsonReader.");
            }

            while (reader.TokenType == JsonToken.Comment)
            {
                reader.Read();
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw JsonReaderException.Create(reader, "Error reading ZObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            ZObject o = new ZObject();
            o.SetLineInfo(reader as IJsonLineInfo);

            o.ReadTokenFrom(reader);

            return o;
        }

        /// <summary>
        /// Load a <see cref="ZObject"/> from a string that contains JSON.
        /// </summary>
        /// <param name="json">A <see cref="String"/> that contains JSON.</param>
        /// <returns>A <see cref="ZObject"/> populated from the string that contains JSON.</returns>
        /// <example>
        ///   <code lang="cs" source="..\Src\Newtonsoft.Json.Tests\Documentation\LinqToJsonTests.cs" region="LinqToJsonCreateParse" title="Parsing a JSON Object from Text" />
        /// </example>
        public new static ZObject Parse(string json)
        {
            using (JsonReader reader = new JsonTextReader(new StringReader(json)))
            {
                ZObject o = Load(reader);

                if (reader.Read() && reader.TokenType != JsonToken.Comment)
                    throw JsonReaderException.Create(reader, "Additional text found in JSON string after parsing content.");

                return o;
            }
        }

        /// <summary>
        /// Creates a <see cref="ZObject"/> from an object.
        /// </summary>
        /// <param name="o">The object that will be used to create <see cref="ZObject"/>.</param>
        /// <returns>A <see cref="ZObject"/> with the values of the specified object</returns>
        public new static ZObject FromObject(object o)
        {
            return FromObject(o, JsonSerializer.CreateDefault());
        }

        /// <summary>
        /// Creates a <see cref="ZObject"/> from an object.
        /// </summary>
        /// <param name="o">The object that will be used to create <see cref="ZObject"/>.</param>
        /// <param name="jsonSerializer">The <see cref="JsonSerializer"/> that will be used to read the object.</param>
        /// <returns>A <see cref="ZObject"/> with the values of the specified object</returns>
        public new static ZObject FromObject(object o, JsonSerializer jsonSerializer)
        {
            ZToken token = FromObjectInternal(o, jsonSerializer);

            if (token != null && token.Type != ZTokenType.Object)
                throw new ArgumentException("Object serialized to {0}. ZObject instance expected.".FormatWith(CultureInfo.InvariantCulture, token.Type));

            return (ZObject)token;
        }

        /// <summary>
        /// Writes this token to a <see cref="JsonWriter"/>.
        /// </summary>
        /// <param name="writer">A <see cref="JsonWriter"/> into which this method will write.</param>
        /// <param name="converters">A collection of <see cref="JsonConverter"/> which will be used when writing the token.</param>
        public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
        {
            writer.WriteStartObject();

            for (int i = 0; i < _properties.Count; i++)
            {
                _properties[i].WriteTo(writer, converters);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Gets the <see cref="Newtonsoft.Json.Linq.ZToken"/> with the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The <see cref="Newtonsoft.Json.Linq.ZToken"/> with the specified property name.</returns>
        public ZToken GetValue(string propertyName)
        {
            return GetValue(propertyName, StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the <see cref="Newtonsoft.Json.Linq.ZToken"/> with the specified property name.
        /// The exact property name will be searched for first and if no matching property is found then
        /// the <see cref="StringComparison"/> will be used to match a property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="comparison">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <returns>The <see cref="Newtonsoft.Json.Linq.ZToken"/> with the specified property name.</returns>
        public ZToken GetValue(string propertyName, StringComparison comparison)
        {
            if (propertyName == null)
                return null;

            // attempt to get value via dictionary first for performance
            JProperty property = Property(propertyName);
            if (property != null)
                return property.Value;

            // test above already uses this comparison so no need to repeat
            if (comparison != StringComparison.Ordinal)
            {
                foreach (JProperty p in _properties)
                {
                    if (string.Equals(p.Name, propertyName, comparison))
                        return p.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to get the <see cref="Newtonsoft.Json.Linq.ZToken"/> with the specified property name.
        /// The exact property name will be searched for first and if no matching property is found then
        /// the <see cref="StringComparison"/> will be used to match a property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparison">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <returns>true if a value was successfully retrieved; otherwise, false.</returns>
        public bool TryGetValue(string propertyName, StringComparison comparison, out ZToken value)
        {
            value = GetValue(propertyName, comparison);
            return (value != null);
        }
#endif


        #region IDictionary<string,ZToken> Members

        //public void Add(ZProperty property)
        //{
        //    _properties.Add(property.Name, property);
        //    property.Parent = this;
        //}

        /// <summary>
        /// Adds the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void Add(string propertyName, ZToken value)
        {
            Add(new ZProperty(propertyName, value));
        }


        bool IDictionary<string, ZToken>.ContainsKey(string key)
        {
            return _properties.Contains(key);
        }


        ICollection<string> IDictionary<string, ZToken>.Keys
        {
            // todo: make order the collection returned match ZObject order
            get { return _properties.Keys; }
        }


        /// <summary>
        /// Removes the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>true if item was successfully removed; otherwise, false.</returns>
        public bool Remove(string propertyName)
        {
            ZProperty property = Property(propertyName);
            if (property == null)
                return false;

            throw new NotImplementedException();
            //property.Remove();
            //return true;
        }



        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if a value was successfully retrieved; otherwise, false.</returns>
        public bool TryGetValue(string propertyName, out ZToken value)
        {
            ZProperty property = Property(propertyName);
            if (property == null)
            {
                value = null;
                return false;
            }

            value = property.Value;
            return true;
        }



        ICollection<ZToken> IDictionary<string, ZToken>.Values
        {
            get
            {
                // todo: need to wrap _properties.Values with a collection to get the JProperty value
                throw new NotImplementedException();
            }
        }
        #endregion
        

        #region ICollection<KeyValuePair<string,ZToken>> Members

        void ICollection<KeyValuePair<string, ZToken>>.Add(KeyValuePair<string, ZToken> item)
        {
            Add(new ZProperty(item.Key, item.Value));
        }


        void ICollection<KeyValuePair<string, ZToken>>.Clear()
        {
            throw new NotImplementedException();
            // RemoveAll();
        }


        bool ICollection<KeyValuePair<string, ZToken>>.Contains(KeyValuePair<string, ZToken> item)
        {
            ZProperty property = Property(item.Key);
            if (property == null)
                return false;

            return (property.Value == item.Value);
        }



        void ICollection<KeyValuePair<string, ZToken>>.CopyTo(KeyValuePair<string, ZToken>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
            //if (array == null)
            //    throw new ArgumentNullException("array");
            //if (arrayIndex < 0)
            //    throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
            //if (arrayIndex >= array.Length && arrayIndex != 0)
            //    throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
            //if (Count > array.Length - arrayIndex)
            //    throw new ArgumentException("The number of elements in the source ZObject is greater than the available space from arrayIndex to the end of the destination array.");

            //int index = 0;
            //foreach (ZProperty property in _properties)
            //{
            //    array[arrayIndex + index] = new KeyValuePair<string, ZToken>(property.Name, property.Value);
            //    index++;
            //}
        }



        bool ICollection<KeyValuePair<string, ZToken>>.IsReadOnly
        {
            get { return false; }
        }


        bool ICollection<KeyValuePair<string, ZToken>>.Remove(KeyValuePair<string, ZToken> item)
        {
            if (!((ICollection<KeyValuePair<string, ZToken>>)this).Contains(item))
                return false;

            ((IDictionary<string, ZToken>)this).Remove(item.Key);
            return true;
        }
        #endregion


#if false
        internal override int GetDeepHashCode()
        {
            return ContentsHashCode();
        }
#endif



        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, ZToken>> GetEnumerator()
        {
            foreach (var zToken in _properties)
            {
                var property = (ZProperty) zToken;

                yield return new KeyValuePair<string, ZToken>(property.Name, property.Value);
            }
        }



#if false
        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event with the provided arguments.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

#if !(NETFX_CORE || PORTABLE40 || PORTABLE || NET20)
        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event with the provided arguments.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }
#endif

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
        // include custom type descriptor on ZObject rather than use a provider because the properties are specific to a type

        #region ICustomTypeDescriptor
        /// <summary>
        /// Returns the properties for this instance of a component.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the properties for this component instance.
        /// </returns>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }

        /// <summary>
        /// Returns the properties for this instance of a component using the attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the filtered properties for this component instance.
        /// </returns>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection descriptors = new PropertyDescriptorCollection(null);

            foreach (KeyValuePair<string, ZToken> propertyValue in this)
            {
                descriptors.Add(new JPropertyDescriptor(propertyValue.Key));
            }

            return descriptors;
        }

        /// <summary>
        /// Returns a collection of custom attributes for this instance of a component.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.AttributeCollection"/> containing the attributes for this object.
        /// </returns>
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return AttributeCollection.Empty;
        }

        /// <summary>
        /// Returns the class name of this instance of a component.
        /// </summary>
        /// <returns>
        /// The class name of the object, or null if the class does not have a name.
        /// </returns>
        string ICustomTypeDescriptor.GetClassName()
        {
            return null;
        }

        /// <summary>
        /// Returns the name of this instance of a component.
        /// </summary>
        /// <returns>
        /// The name of the object, or null if the object does not have a name.
        /// </returns>
        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }

        /// <summary>
        /// Returns a type converter for this instance of a component.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter"/> that is the converter for this object, or null if there is no <see cref="T:System.ComponentModel.TypeConverter"/> for this object.
        /// </returns>
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return new TypeConverter();
        }

        /// <summary>
        /// Returns the default event for this instance of a component.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.EventDescriptor"/> that represents the default event for this object, or null if this object does not have events.
        /// </returns>
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }

        /// <summary>
        /// Returns the default property for this instance of a component.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptor"/> that represents the default property for this object, or null if this object does not have properties.
        /// </returns>
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }

        /// <summary>
        /// Returns an editor of the specified type for this instance of a component.
        /// </summary>
        /// <param name="editorBaseType">A <see cref="T:System.Type"/> that represents the editor for this object.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> of the specified type that is the editor for this object, or null if the editor cannot be found.
        /// </returns>
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }

        /// <summary>
        /// Returns the events for this instance of a component using the specified attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.EventDescriptorCollection"/> that represents the filtered events for this component instance.
        /// </returns>
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }

        /// <summary>
        /// Returns the events for this instance of a component.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.EventDescriptorCollection"/> that represents the events for this component instance.
        /// </returns>
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }

        /// <summary>
        /// Returns an object that contains the property described by the specified property descriptor.
        /// </summary>
        /// <param name="pd">A <see cref="T:System.ComponentModel.PropertyDescriptor"/> that represents the property whose owner is to be found.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the owner of the specified property.
        /// </returns>
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return null;
        }
        #endregion

#endif

#if !(NET35 || NET20 || PORTABLE40)
        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject"/> responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject"/> to bind this object.
        /// </returns>
        protected override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicProxyMetaObject<ZObject>(parameter, this, new ZObjectDynamicProxy(), true);
        }

        private class ZObjectDynamicProxy : DynamicProxy<ZObject>
        {
            public override bool TryGetMember(ZObject instance, GetMemberBinder binder, out object result)
            {
                // result can be null
                result = instance[binder.Name];
                return true;
            }

            public override bool TrySetMember(ZObject instance, SetMemberBinder binder, object value)
            {
                ZToken v = value as ZToken;

                // this can throw an error if value isn't a valid for a JValue
                if (v == null)
                    v = new JValue(value);

                instance[binder.Name] = v;
                return true;
            }

            public override IEnumerable<string> GetDynamicMemberNames(ZObject instance)
            {
                return instance.Properties().Select(p => p.Name);
            }
        }
#endif
#endif

    }
}

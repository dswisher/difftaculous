﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Difftaculous.ZModel
{
    internal abstract class ZContainer : ZToken, IList<ZToken> // , ITypedList, IBindingList, INotifyCollectionChanged, IList, INotifyCollectionChanged
    {

#if false
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
        internal ListChangedEventHandler _listChanged;
        internal AddingNewEventHandler _addingNew;

        /// <summary>
        /// Occurs when the list changes or an item in the list changes.
        /// </summary>
        public event ListChangedEventHandler ListChanged
        {
            add { _listChanged += value; }
            remove { _listChanged -= value; }
        }

        /// <summary>
        /// Occurs before an item is added to the collection.
        /// </summary>
        public event AddingNewEventHandler AddingNew
        {
            add { _addingNew += value; }
            remove { _addingNew -= value; }
        }
#endif
#if !(NET20 || NET35 || PORTABLE40)
        internal NotifyCollectionChangedEventHandler _collectionChanged;

        /// <summary>
        /// Occurs when the items list of the collection has changed, or the collection is reset.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _collectionChanged += value; }
            remove { _collectionChanged -= value; }
        }
#endif
#endif


        /// <summary>
        /// Gets the container's children tokens.
        /// </summary>
        /// <value>The container's children tokens.</value>
        protected abstract IList<ZToken> ChildrenTokens { get; }


#if false
        private object _syncRoot;
#if !(PORTABLE40)
        private bool _busy;
#endif
#endif


        internal ZContainer()
        {
        }


        internal ZContainer(ZContainer other)
            : this()
        {
            ValidationUtils.ArgumentNotNull(other, "c");

            foreach (ZToken child in other)
            {
                Add(child);
            }
        }


#if false


        internal void CheckReentrancy()
        {
#if !(PORTABLE40)
            if (_busy)
                throw new InvalidOperationException("Cannot change {0} during a collection change event.".FormatWith(CultureInfo.InvariantCulture, GetType()));
#endif
        }

        internal virtual IList<ZToken> CreateChildrenCollection()
        {
            return new List<ZToken>();
        }

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
        /// <summary>
        /// Raises the <see cref="AddingNew"/> event.
        /// </summary>
        /// <param name="e">The <see cref="AddingNewEventArgs"/> instance containing the event data.</param>
        protected virtual void OnAddingNew(AddingNewEventArgs e)
        {
            AddingNewEventHandler handler = _addingNew;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="ListChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ListChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            ListChangedEventHandler handler = _listChanged;

            if (handler != null)
            {
                _busy = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    _busy = false;
                }
            }
        }
#endif
#if !(NET20 || NET35 || PORTABLE40)
        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = _collectionChanged;

            if (handler != null)
            {
                _busy = true;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    _busy = false;
                }
            }
        }
#endif
#endif


        /// <summary>
        /// Gets a value indicating whether this token has child tokens.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this token has child values; otherwise, <c>false</c>.
        /// </value>
        public override bool HasValues
        {
            get { return ChildrenTokens.Count > 0; }
        }



        internal bool ContentsEqual(ZContainer container)
        {
            if (container == this)
                return true;

            IList<ZToken> t1 = ChildrenTokens;
            IList<ZToken> t2 = container.ChildrenTokens;

            if (t1.Count != t2.Count)
                return false;

            for (int i = 0; i < t1.Count; i++)
            {
                if (!t1[i].DeepEquals(t2[i]))
                    return false;
            }

            return true;
        }



        /// <summary>
        /// Get the first child token of this token.
        /// </summary>
        /// <value>
        /// A <see cref="ZToken"/> containing the first child token of the <see cref="ZToken"/>.
        /// </value>
        public override ZToken First
        {
            get { return ChildrenTokens.FirstOrDefault(); }
        }

        /// <summary>
        /// Get the last child token of this token.
        /// </summary>
        /// <value>
        /// A <see cref="ZToken"/> containing the last child token of the <see cref="ZToken"/>.
        /// </value>
        public override ZToken Last
        {
            get { return ChildrenTokens.LastOrDefault(); }
        }



        /// <summary>
        /// Returns a collection of the child tokens of this token, in document order.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="ZToken"/> containing the child tokens of this <see cref="ZToken"/>, in document order.
        /// </returns>
        public override ZEnumerable<ZToken> Children()
        {
            return new ZEnumerable<ZToken>(ChildrenTokens);
        }



        /// <summary>
        /// Returns a collection of the child values of this token, in document order.
        /// </summary>
        /// <typeparam name="T">The type to convert the values to.</typeparam>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> containing the child values of this <see cref="ZToken"/>, in document order.
        /// </returns>
        public override IEnumerable<T> Values<T>()
        {
            return ChildrenTokens.Convert<ZToken, T>();
        }



        /// <summary>
        /// Returns a collection of the descendant tokens for this token in document order.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{ZToken}"/> containing the descendant tokens of the <see cref="ZToken"/>.</returns>
        public IEnumerable<ZToken> Descendants()
        {
            foreach (ZToken o in ChildrenTokens)
            {
                yield return o;
                ZContainer c = o as ZContainer;
                if (c != null)
                {
                    foreach (ZToken d in c.Descendants())
                    {
                        yield return d;
                    }
                }
            }
        }



        internal bool IsMultiContent(object content)
        {
            return (content is IEnumerable && !(content is string) && !(content is ZToken) && !(content is byte[]));
        }



        internal ZToken EnsureParentToken(ZToken item, bool skipParentCheck)
        {
            if (item == null)
                return ZValue.CreateNull();

            if (skipParentCheck)
                return item;

            // to avoid a token having multiple parents or creating a recursive loop, create a copy if...
            // the item already has a parent
            // the item is being added to itself
            // the item is being added to the root parent of itself
            if (item.Parent != null || item == this || (item.HasValues && Root == item))
            {
                throw new NotImplementedException();
                // item = item.CloneToken();
            }

            return item;
        }



        private class ZTokenReferenceEqualityComparer : IEqualityComparer<ZToken>
        {
            public static readonly ZTokenReferenceEqualityComparer Instance = new ZTokenReferenceEqualityComparer();

            public bool Equals(ZToken x, ZToken y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(ZToken obj)
            {
                if (obj == null)
                    return 0;

                return obj.GetHashCode();
            }
        }



        internal int IndexOfItem(ZToken item)
        {
            return ChildrenTokens.IndexOf(item, ZTokenReferenceEqualityComparer.Instance);
        }



        internal virtual void InsertItem(int index, ZToken item, bool skipParentCheck)
        {
            if (index > ChildrenTokens.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index must be within the bounds of the List.");
            }

            //CheckReentrancy();

            item = EnsureParentToken(item, skipParentCheck);

            ZToken previous = (index == 0) ? null : ChildrenTokens[index - 1];
            // haven't inserted new token yet so next token is still at the inserting index
            ZToken next = (index == ChildrenTokens.Count) ? null : ChildrenTokens[index];

            ValidateToken(item, null);

            item.Parent = this;

            item.Previous = previous;
            if (previous != null)
                previous.Next = item;

            item.Next = next;
            if (next != null)
                next.Previous = item;

            ChildrenTokens.Insert(index, item);

            // TODO - OnChanged notifications
#if false
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
            if (_listChanged != null)
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
#endif
#if !(NET20 || NET35 || PORTABLE40)
            if (_collectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
#endif
#endif
        }



        internal virtual void RemoveItemAt(int index)
        {
            throw new NotImplementedException();
#if false
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
            if (index >= ChildrenTokens.Count)
                throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");

            CheckReentrancy();

            ZToken item = ChildrenTokens[index];
            ZToken previous = (index == 0) ? null : ChildrenTokens[index - 1];
            ZToken next = (index == ChildrenTokens.Count - 1) ? null : ChildrenTokens[index + 1];

            if (previous != null)
                previous.Next = next;
            if (next != null)
                next.Previous = previous;

            item.Parent = null;
            item.Previous = null;
            item.Next = null;

            ChildrenTokens.RemoveAt(index);

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
            if (_listChanged != null)
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
#endif
#if !(NET20 || NET35 || PORTABLE40)
            if (_collectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
#endif
#endif
        }



        internal virtual bool RemoveItem(ZToken item)
        {
            int index = IndexOfItem(item);
            if (index >= 0)
            {
                RemoveItemAt(index);
                return true;
            }

            return false;
        }



        internal virtual ZToken GetItem(int index)
        {
            return ChildrenTokens[index];
        }


        internal virtual void SetItem(int index, ZToken item)
        {
            throw new NotImplementedException();
#if false
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Index is less than 0.");
            if (index >= ChildrenTokens.Count)
                throw new ArgumentOutOfRangeException("index", "Index is equal to or greater than Count.");

            ZToken existing = ChildrenTokens[index];

            if (IsTokenUnchanged(existing, item))
                return;

            CheckReentrancy();

            item = EnsureParentToken(item, false);

            ValidateToken(item, existing);

            ZToken previous = (index == 0) ? null : ChildrenTokens[index - 1];
            ZToken next = (index == ChildrenTokens.Count - 1) ? null : ChildrenTokens[index + 1];

            item.Parent = this;

            item.Previous = previous;
            if (previous != null)
                previous.Next = item;

            item.Next = next;
            if (next != null)
                next.Previous = item;

            ChildrenTokens[index] = item;

            existing.Parent = null;
            existing.Previous = null;
            existing.Next = null;

#if !(NETFX_CORE || PORTABLE || PORTABLE40)
            if (_listChanged != null)
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
#endif
#if !(NET20 || NET35 || PORTABLE40)
            if (_collectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, existing, index));
#endif
#endif
        }



        internal virtual void ClearItems()
        {
            //CheckReentrancy();

            foreach (ZToken item in ChildrenTokens)
            {
                item.Parent = null;
                item.Previous = null;
                item.Next = null;
            }

            ChildrenTokens.Clear();

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
#if false
            if (_listChanged != null)
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
#endif
#endif
#if !(NET20 || NET35 || PORTABLE40)
#if false
            if (_collectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
#endif
#endif
        }


#if false
        internal virtual void ReplaceItem(ZToken existing, ZToken replacement)
        {
            if (existing == null || existing.Parent != this)
                return;

            int index = IndexOfItem(existing);
            SetItem(index, replacement);
        }
#endif


        internal virtual bool ContainsItem(ZToken item)
        {
            return (IndexOfItem(item) != -1);
        }


        internal virtual void CopyItemsTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
            if (arrayIndex >= array.Length && arrayIndex != 0)
                throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
            if (Count > array.Length - arrayIndex)
                throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");

            int index = 0;
            foreach (ZToken token in ChildrenTokens)
            {
                array.SetValue(token, arrayIndex + index);
                index++;
            }
        }


#if false
        internal static bool IsTokenUnchanged(ZToken currentValue, ZToken newValue)
        {
            JValue v1 = currentValue as JValue;
            if (v1 != null)
            {
                // null will get turned into a JValue of type null
                if (v1.Type == ZTokenType.Null && newValue == null)
                    return true;

                return v1.Equals(newValue);
            }

            return false;
        }
#endif



        internal virtual void ValidateToken(ZToken o, ZToken existing)
        {
            ValidationUtils.ArgumentNotNull(o, "o");

            if (o.Type == TokenType.Property)
            {
                throw new ArgumentException(string.Format("Can not add {0} to {1}.", o.GetType(), GetType()));
            }
        }


        /// <summary>
        /// Adds the specified content as children of this <see cref="ZToken"/>.
        /// </summary>
        /// <param name="content">The content to be added.</param>
        public virtual void Add(object content)
        {
            AddInternal(ChildrenTokens.Count, content, false);
        }


        internal void AddAndSkipParentCheck(ZToken token)
        {
            AddInternal(ChildrenTokens.Count, token, true);
        }


        /// <summary>
        /// Adds the specified content as the first child of this <see cref="ZToken"/>.
        /// </summary>
        /// <param name="content">The content to be added.</param>
        public void AddFirst(object content)
        {
            AddInternal(0, content, false);
        }



        internal void AddInternal(int index, object content, bool skipParentCheck)
        {
            if (IsMultiContent(content))
            {
                IEnumerable enumerable = (IEnumerable)content;

                int multiIndex = index;
                foreach (object c in enumerable)
                {
                    AddInternal(multiIndex, c, skipParentCheck);
                    multiIndex++;
                }
            }
            else
            {
                ZToken item = CreateFromContent(content);

                InsertItem(index, item, skipParentCheck);
            }
        }



        internal ZToken CreateFromContent(object content)
        {
            if (content is ZToken)
                return (ZToken)content;

            return new ZValue(content);
        }



#if false
        /// <summary>
        /// Creates an <see cref="JsonWriter"/> that can be used to add tokens to the <see cref="ZToken"/>.
        /// </summary>
        /// <returns>An <see cref="JsonWriter"/> that is ready to have content written to it.</returns>
        public JsonWriter CreateWriter()
        {
            return new ZTokenWriter(this);
        }

        /// <summary>
        /// Replaces the children nodes of this token with the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void ReplaceAll(object content)
        {
            ClearItems();
            Add(content);
        }

        /// <summary>
        /// Removes the child nodes from this token.
        /// </summary>
        public void RemoveAll()
        {
            ClearItems();
        }

        internal void ReadTokenFrom(JsonReader reader)
        {
            int startDepth = reader.Depth;

            if (!reader.Read())
                throw JsonReaderException.Create(reader, "Error reading {0} from JsonReader.".FormatWith(CultureInfo.InvariantCulture, GetType().Name));

            ReadContentFrom(reader);

            int endDepth = reader.Depth;

            if (endDepth > startDepth)
                throw JsonReaderException.Create(reader, "Unexpected end of content while loading {0}.".FormatWith(CultureInfo.InvariantCulture, GetType().Name));
        }

        internal void ReadContentFrom(JsonReader r)
        {
            ValidationUtils.ArgumentNotNull(r, "r");
            IJsonLineInfo lineInfo = r as IJsonLineInfo;

            JContainer parent = this;

            do
            {
                if (parent is JProperty && ((JProperty)parent).Value != null)
                {
                    if (parent == this)
                        return;

                    parent = parent.Parent;
                }

                switch (r.TokenType)
                {
                    case JsonToken.None:
                        // new reader. move to actual content
                        break;
                    case JsonToken.StartArray:
                        JArray a = new JArray();
                        a.SetLineInfo(lineInfo);
                        parent.Add(a);
                        parent = a;
                        break;

                    case JsonToken.EndArray:
                        if (parent == this)
                            return;

                        parent = parent.Parent;
                        break;
                    case JsonToken.StartObject:
                        JObject o = new JObject();
                        o.SetLineInfo(lineInfo);
                        parent.Add(o);
                        parent = o;
                        break;
                    case JsonToken.EndObject:
                        if (parent == this)
                            return;

                        parent = parent.Parent;
                        break;
                    case JsonToken.StartConstructor:
                        JConstructor constructor = new JConstructor(r.Value.ToString());
                        constructor.SetLineInfo(lineInfo);
                        parent.Add(constructor);
                        parent = constructor;
                        break;
                    case JsonToken.EndConstructor:
                        if (parent == this)
                            return;

                        parent = parent.Parent;
                        break;
                    case JsonToken.String:
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.Date:
                    case JsonToken.Boolean:
                    case JsonToken.Bytes:
                        JValue v = new JValue(r.Value);
                        v.SetLineInfo(lineInfo);
                        parent.Add(v);
                        break;
                    case JsonToken.Comment:
                        v = JValue.CreateComment(r.Value.ToString());
                        v.SetLineInfo(lineInfo);
                        parent.Add(v);
                        break;
                    case JsonToken.Null:
                        v = JValue.CreateNull();
                        v.SetLineInfo(lineInfo);
                        parent.Add(v);
                        break;
                    case JsonToken.Undefined:
                        v = JValue.CreateUndefined();
                        v.SetLineInfo(lineInfo);
                        parent.Add(v);
                        break;
                    case JsonToken.PropertyName:
                        string propertyName = r.Value.ToString();
                        JProperty property = new JProperty(propertyName);
                        property.SetLineInfo(lineInfo);
                        JObject parentObject = (JObject)parent;
                        // handle multiple properties with the same name in JSON
                        JProperty existingPropertyWithName = parentObject.Property(propertyName);
                        if (existingPropertyWithName == null)
                            parent.Add(property);
                        else
                            existingPropertyWithName.Replace(property);
                        parent = property;
                        break;
                    default:
                        throw new InvalidOperationException("The JsonReader should not be on a token of type {0}.".FormatWith(CultureInfo.InvariantCulture, r.TokenType));
                }
            } while (r.Read());
        }

        internal int ContentsHashCode()
        {
            int hashCode = 0;
            foreach (ZToken item in ChildrenTokens)
            {
                hashCode ^= item.GetDeepHashCode();
            }
            return hashCode;
        }

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return string.Empty;
        }

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            ICustomTypeDescriptor d = First as ICustomTypeDescriptor;
            if (d != null)
                return d.GetProperties();

            return null;
        }
#endif
#endif


        #region IList<ZToken> Members

        int IList<ZToken>.IndexOf(ZToken item)
        {
            return IndexOfItem(item);
        }

        void IList<ZToken>.Insert(int index, ZToken item)
        {
            InsertItem(index, item, false);
        }

        void IList<ZToken>.RemoveAt(int index)
        {
            throw new NotImplementedException();
            // RemoveItemAt(index);
        }

        ZToken IList<ZToken>.this[int index]
        {
            get { return GetItem(index); }
            set { SetItem(index, value); }
        }

        #endregion


        #region ICollection<ZToken> Members

        void ICollection<ZToken>.Add(ZToken item)
        {
            Add(item);
        }

        void ICollection<ZToken>.Clear()
        {
            ClearItems();
        }

        bool ICollection<ZToken>.Contains(ZToken item)
        {
            return ContainsItem(item);
        }

        void ICollection<ZToken>.CopyTo(ZToken[] array, int arrayIndex)
        {
            throw new NotImplementedException();
            // CopyItemsTo(array, arrayIndex);
        }

        bool ICollection<ZToken>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<ZToken>.Remove(ZToken item)
        {
            throw new NotImplementedException();
            // return RemoveItem(item);
        }

        #endregion



#if false
        private ZToken EnsureValue(object value)
        {
            if (value == null)
                return null;

            if (value is ZToken)
                return (ZToken)value;

            throw new ArgumentException("Argument is not a ZToken.");
        }

        #region IList Members
        int IList.Add(object value)
        {
            Add(EnsureValue(value));
            return Count - 1;
        }

        void IList.Clear()
        {
            ClearItems();
        }

        bool IList.Contains(object value)
        {
            return ContainsItem(EnsureValue(value));
        }

        int IList.IndexOf(object value)
        {
            return IndexOfItem(EnsureValue(value));
        }

        void IList.Insert(int index, object value)
        {
            InsertItem(index, EnsureValue(value), false);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            RemoveItem(EnsureValue(value));
        }

        void IList.RemoveAt(int index)
        {
            RemoveItemAt(index);
        }

        object IList.this[int index]
        {
            get { return GetItem(index); }
            set { SetItem(index, EnsureValue(value)); }
        }
        #endregion
#endif


        #region ICollection Members
#if false
        void ICollection.CopyTo(Array array, int index)
        {
            CopyItemsTo(array, index);
        }
#endif


        /// <summary>
        /// Gets the count of child JSON tokens.
        /// </summary>
        /// <value>The count of child JSON tokens</value>
        public int Count
        {
            get { return ChildrenTokens.Count; }
        }


#if false
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);

                return _syncRoot;
            }
        }
#endif
        #endregion


#if false
        #region IBindingList Members
#if !(NETFX_CORE || PORTABLE || PORTABLE40)
        void IBindingList.AddIndex(PropertyDescriptor property)
        {
        }

        object IBindingList.AddNew()
        {
            AddingNewEventArgs args = new AddingNewEventArgs();
            OnAddingNew(args);

            if (args.NewObject == null)
                throw new JsonException("Could not determine new value to add to '{0}'.".FormatWith(CultureInfo.InvariantCulture, GetType()));

            if (!(args.NewObject is ZToken))
                throw new JsonException("New item to be added to collection must be compatible with {0}.".FormatWith(CultureInfo.InvariantCulture, typeof(ZToken)));

            ZToken newItem = (ZToken)args.NewObject;
            Add(newItem);

            return newItem;
        }

        bool IBindingList.AllowEdit
        {
            get { return true; }
        }

        bool IBindingList.AllowNew
        {
            get { return true; }
        }

        bool IBindingList.AllowRemove
        {
            get { return true; }
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotSupportedException();
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }

        bool IBindingList.IsSorted
        {
            get { return false; }
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
        }

        void IBindingList.RemoveSort()
        {
            throw new NotSupportedException();
        }

        ListSortDirection IBindingList.SortDirection
        {
            get { return ListSortDirection.Ascending; }
        }

        PropertyDescriptor IBindingList.SortProperty
        {
            get { return null; }
        }

        bool IBindingList.SupportsChangeNotification
        {
            get { return true; }
        }

        bool IBindingList.SupportsSearching
        {
            get { return false; }
        }

        bool IBindingList.SupportsSorting
        {
            get { return false; }
        }
#endif
        #endregion

#endif

    }
}

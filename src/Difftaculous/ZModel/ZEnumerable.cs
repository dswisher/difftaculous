
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Difftaculous.ZModel
{
    /// <summary>
    /// Represents a collection of <see cref="ZToken"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of token</typeparam>
    internal interface IZEnumerable<out T> : IEnumerable<T> where T : ZToken
    {
        /// <summary>
        /// Gets the <see cref="IZEnumerable{ZToken}"/> with the specified key.
        /// </summary>
        /// <value></value>
        IZEnumerable<ZToken> this[object key] { get; }
    }


    internal struct ZEnumerable<T> : IZEnumerable<T> where T : ZToken
    {
                /// <summary>
        /// An empty collection of <see cref="ZToken"/> objects.
        /// </summary>
        public static readonly ZEnumerable<T> Empty = new ZEnumerable<T>(Enumerable.Empty<T>());

        private readonly IEnumerable<T> _enumerable;

                /// <summary>
        /// Initializes a new instance of the <see cref="ZEnumerable{T}"/> struct.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public ZEnumerable(IEnumerable<T> enumerable)
        {
            // TODO - add this back!
            //ValidationUtils.ArgumentNotNull(enumerable, "enumerable");

            _enumerable = enumerable;
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }



        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Gets the <see cref="IZEnumerable{JToken}"/> with the specified key.
        /// </summary>
        /// <value></value>
        public IZEnumerable<ZToken> this[object key]
        {
            get { return new ZEnumerable<ZToken>(Extensions.Values<T, ZToken>(_enumerable, key)); }
        }



#if false
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is JEnumerable<T>)
                return _enumerable.Equals(((JEnumerable<T>)obj)._enumerable);

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _enumerable.GetHashCode();
        }
#endif
    }
}

using System;
using System.Collections.ObjectModel;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a collection of public address reflection services.
    /// </summary>
    public sealed class PublicAddressReflectionServices : KeyedCollection<Uri, IPublicAddressReflectionService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicAddressReflectionServices"/> class.
        /// </summary>
        /// <param name="item">
        /// The item to add to the collection.
        /// </param>
        /// <returns>
        /// The key for the specified item.
        /// </returns>
        protected override Uri GetKeyForItem(IPublicAddressReflectionService item) => item.ServiceUri;
    }
}

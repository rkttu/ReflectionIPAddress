using System.Net;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents detailed information about a public IP address, including geographic and network metadata.
    /// </summary>
    public sealed class IPAddressInfo
    {
        /// <summary>
        /// Gets or sets the public IP address.
        /// </summary>
        public IPAddress Address { get; set; }

        /// <summary>
        /// Gets or sets the country name (e.g. "South Korea").
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the ISO country code (e.g. "KR").
        /// </summary>
        public string CountryISO { get; set; }

        /// <summary>
        /// Gets or sets the city name.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the hostname associated with the IP address.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the autonomous system number (ASN).
        /// </summary>
        public int? ASN { get; set; }

        /// <summary>
        /// Gets or sets the autonomous system organization name.
        /// </summary>
        public string ASNOrganization { get; set; }

        /// <summary>
        /// Gets or sets the time zone (e.g. "Asia/Seoul").
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double? Longitude { get; set; }
    }
}

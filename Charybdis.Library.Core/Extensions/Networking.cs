using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// Provides network information with caching for the local machine.
    /// </summary>
    public static class Networking
    {
        private static string _cachedIPAddresses;
        /// <summary>
        /// IP addresses of the current machine (caches them for performance).
        /// </summary>
        public static string IPAddresses
        {
            get
            {
                if (_cachedIPAddresses == null)
                    _cachedIPAddresses = Dns.GetHostAddresses(Dns.GetHostName()).Join(";");
                return _cachedIPAddresses;
            }
        }

        private static string _cachedHostname;
        /// <summary>
        /// Hostname of the current machine (cached for performance).
        /// </summary>
        public static string Hostname
        {
            get
            {
                if (_cachedHostname == null)
                    _cachedHostname = Dns.GetHostName();
                return _cachedHostname;
            }
        }

        private static string _cachedHostnameAndIPAddresses;
        /// <summary>
        /// Hostname and IP addresses for the current machine (cached for performance).
        /// </summary>
        public static string HostnameAndIPAddresses
        {
            get
            {
                if (_cachedHostnameAndIPAddresses == null)
                    _cachedHostnameAndIPAddresses = Hostname + '(' +IPAddresses + ')';
                return _cachedHostnameAndIPAddresses;
            }
        }

        /// <summary>
        /// Forces the various caches of network information to be emptied, allowing new values to be gathered.
        /// </summary>
        public static void Refresh()
        {
            _cachedHostname = null;
            _cachedIPAddresses = null;
            _cachedHostnameAndIPAddresses = null;
        }
    }
}

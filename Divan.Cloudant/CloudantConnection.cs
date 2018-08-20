using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Divan.Cloudant
{
    public sealed class CloudantConnection
    {
        public Uri Uri { get; set; }
        public NetworkCredential Credential { get; set; }

        public CloudantConnection(Uri uri, NetworkCredential credential)
        {
            Uri = uri;
            Credential = credential;
        }

        public CloudantConnection(string uri, string userName, string password)
        {
            Uri = new Uri(uri);
            Credential = new NetworkCredential(userName, password);
        }
    }
}

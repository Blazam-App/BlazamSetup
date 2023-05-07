using System.Security.Cryptography.X509Certificates;

namespace BlazamSetup
{
    public class WebHostConfiguration
    {
        public string ListeningAddress { get; set; } = "*";
        public int HttpPort { get; set; } = 80;
        public int HttpsPort { get; set; } = 443;
        public X509Certificate2 SSLCert { get; set; }

    }
}

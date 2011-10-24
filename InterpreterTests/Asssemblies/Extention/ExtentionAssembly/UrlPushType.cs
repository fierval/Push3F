using System;
using System.Globalization;
using push.exceptions;
using push.types;
using Type = push.types.Type;

namespace ExtensionAssembly
{
    [PushType("URL")]
    public class UrlPushType : Type.PushTypeBase
    {
        static UrlPushType UrlParse(string url)
        {
            try
            {
                if (!url.Trim().StartsWith("http://", true, CultureInfo.InvariantCulture))
                {
                    return null;
                }

                Uri uri = new Uri(url);
                if (uri.HostNameType != UriHostNameType.Dns)
                {
                    return null;
                }

                return new UrlPushType(uri);

            }
            catch (Exception)
            {
                return null;                
            }
        }

        public override Type.ExtendedTypeParser Parser
        {
            get 
            {
                return new Type.ExtendedTypeParser(UrlParse);
            }
        }

        public UrlPushType() : base() {}
        public UrlPushType(Uri url) : base(url) {}

        [PushOperation("DOMAIN", Description="Extract domain name from the URL")]
        static void ExtractDomain()
        {
            var arg = TypeFactory.processArgs1("URL");
            if (arg == null)
            {
                return;
            }

            var uri = arg.Raw<Uri>();
            var newUri = new UrlPushType(new Uri(uri.Host));
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

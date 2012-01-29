using System;
using System.Globalization;
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
            // pop the URL from the URL stack, if anything is there
            var arg = TypeFactory.processArgs1("URL");
            if (arg == null)
            {
                return;
            }

            // extract the underlying data
            var uri = arg.Raw<Uri>();

            //create the new URI
            var newUri = new UrlPushType(new Uri(uri.Host));
            
            // push it back to the URL stack.
            TypeFactory.pushResult(newUri);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

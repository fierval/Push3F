using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using push.types;
using Type = push.types.Type;
using push.exceptions;
using System.Globalization;

namespace ExtentionAssembly
{
    [TypeAttributes.PushType("URL")]
    public class UrlPushType : Type.PushTypeBase
    {
        static UrlPushType UrlParse(string url)
        {
            if (!url.Trim().StartsWith("http://", true, CultureInfo.InvariantCulture))
            {
                url = "http://" + url;
            }

            Uri uri = new Uri(url);
            if (uri.HostNameType != UriHostNameType.Dns)
            {
                throw new PushExceptions.PushException("Unknown scheme");
            }

            return new UrlPushType(uri);
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

        [TypeAttributes.PushOperation("DOMAIN", Description="Extract domain name from the URL")]
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

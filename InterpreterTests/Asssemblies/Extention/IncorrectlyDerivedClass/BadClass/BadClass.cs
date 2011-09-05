using System;
using System.Globalization;
using push.exceptions;
using push.types;
using Type = push.types.Type;

namespace BadClass
{
    [TypeAttributes.PushType("BADCLASS")]
    public class BadClass
    {

        [TypeAttributes.PushOperation("DOMAIN", Description = "Extract domain name from the URL")]
        static void ExtractDomain()
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

}


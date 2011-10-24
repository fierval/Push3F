using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using push.types;
using push.types.stock;

namespace ExtensionAssembly
{
    [GenericPushType]
    public class IntegerOpExtension
    {
        [GenericPushOperation("TOSTRING", 
            Description="Converts top integer to a literal", 
            AppliesTo= new string [] {"INTEGER"})]
        public static void ConvertToString()
        {
            if(TypeFactory.isEmptyStack("INTEGER"))
            {
                return;
            }

            long val = TypeFactory.processArgs1("INTEGER").Raw<long>();

            TypeFactory.pushResult (new StockTypesLiteral.Literal(val.ToString()));
        }

    }
}

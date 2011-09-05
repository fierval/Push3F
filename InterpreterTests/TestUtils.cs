using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using push.types;
using push.types.stock;
using push.stack;
using push.parser;

namespace InterpreterTests
{
    class TestUtils
    {
        internal static void PushVal<T>(object val)
        {
            if (typeof(T) == typeof(StockTypesBool.Bool))
            {
                TypeFactory.pushResult(new StockTypesBool.Bool((bool)val));
            }

            if (typeof(T) == typeof(StockTypesInteger.Integer))
            {
                TypeFactory.pushResult(new StockTypesInteger.Integer(unchecked((long)val)));
            }

            if (typeof(T) == typeof(StockTypesFloat.Float))
            {
                TypeFactory.pushResult(new StockTypesFloat.Float((double)val));
            }

            if (typeof(T) == typeof(StockTypesIdentifier.Identifier))
            {
                TypeFactory.pushResult(new StockTypesIdentifier.Identifier((string)val));
            }

            if (typeof(T) == typeof(StockTypesCode.Code))
            {
                TypeFactory.pushResult(new StockTypesCode.Code((Ast.Push)val));
            }

        }


        internal static Stack.Stack<push.types.Type.PushTypeBase> StackOf(string item)
        {
            return TypeFactory.stockTypes.Stacks[item];
        }

        internal static List<push.types.Type.PushTypeBase> ListOf(string item)
        {
            return StackOf(item).asList.ToList();
        }

        internal static bool IsEmpty(string item)
        {
            return LengthOf(item) == 0;
        }

        internal static T Top<T>(string item)
        {
            return StackOf(item).asList.First().Raw<T>();
        }

        internal static int LengthOf(string item)
        {
            return StackOf(item).length;
        }
    }
}

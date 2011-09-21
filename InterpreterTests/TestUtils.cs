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
        /// <summary>
        /// Pushes a value on the stack
        /// </summary>
        /// <typeparam name="T">Push type</typeparam>
        /// <param name="val">actual value</param>
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

            if (typeof(T) == typeof(StockTypesName.Name))
            {
                TypeFactory.pushResult(new StockTypesName.Name((string)val));
            }

            if (typeof(T) == typeof(StockTypesCode.Code))
            {
                TypeFactory.pushResult(new StockTypesCode.Code((Ast.Push)val));
            }

        }

        /// <summary>
        /// Returns the stack of indicated type
        /// </summary>
        /// <param name="item">Name of the Push type</param>
        /// <returns>The stack</returns>
        internal static Stack.Stack<push.types.Type.PushTypeBase> StackOf(string item)
        {
            return TypeFactory.stockTypes.Stacks[item];
        }

        /// <summary>
        /// Returns the stack as a list
        /// </summary>
        /// <param name="item">Name of push type</param>
        /// <returns>List representing the stack</returns>
        internal static List<push.types.Type.PushTypeBase> ListOf(string item)
        {
            return StackOf(item).asList.ToList();
        }

        /// <summary>
        /// Is the stack of designated type empty
        /// </summary>
        /// <param name="item">Name of push type</param>
        /// <returns>true if the stack is empty</returns>
        internal static bool IsEmpty(string item)
        {
            return LengthOf(item) == 0;
        }

        /// <summary>
        ///  Peeks top of the designated stack
        /// </summary>
        /// <typeparam name="T">Underlying .NET type of the expected item</typeparam>
        /// <param name="item">Name of push type</param>
        /// <returns>The value of the top item</returns>
        internal static T Top<T>(string item)
        {
            return StackOf(item).asList.First().Raw<T>();
        }

        /// <summary>
        /// Length of the designated stack
        /// </summary>
        /// <param name="item">Name of push type</param>
        /// <returns>Integer length of the stack</returns>
        internal static int LengthOf(string item)
        {
            return StackOf(item).length;
        }

        /// <summary>
        /// Runs Push parser and extracts results
        /// </summary>
        /// <param name="str">Push program</param>
        /// <returns>Parsed program or error tuple: (str, error)</returns>
        internal static dynamic RunParser(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException("Please set str argument to something meaningful");
            }
            var pres = Parser.parsePushString(str);
            dynamic res = Parser.extractResult(pres);

            return res;

        }
    }
}

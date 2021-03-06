﻿using System;
using System.Collections.Generic;
using System.Linq;
using push.parser;
using push.stack;
using push.types;
using push.types.stock;
using Push = push.types.Ast.Push;
using PushTypeBase = push.types.Type.PushTypeBase;

namespace InterpreterTests
{
    public class TestUtils
    {
        /// <summary>
        /// Pushes a value on the stack
        /// </summary>
        /// <typeparam name="T">Push type</typeparam>
        /// <param name="val">actual value</param>
        public static void PushVal<T>(object val)
        {
            if (typeof(T) == typeof(Bool))
            {
                TypeFactory.pushResult(new Bool((bool)val));
            }

            if (typeof(T) == typeof(Integer))
            {
                TypeFactory.pushResult(new Integer(unchecked((long)val)));
            }

            if (typeof(T) == typeof(Float))
            {
                TypeFactory.pushResult(new Float((double)val));
            }

            if (typeof(T) == typeof(Name))
            {
                TypeFactory.pushResult(new Name((string)val));
            }

            if (typeof(T) == typeof(Code))
            {
                TypeFactory.pushResult(new Code((Ast.Push)val));
            }

        }

        /// <summary>
        /// Returns the stack of indicated type
        /// </summary>
        /// <param name="item">Name of the Push type</param>
        /// <returns>The stack</returns>
        public static Stack.Stack<PushTypeBase> StackOf(string item)
        {
            return TypeFactory.stockTypes.Stacks[item];
        }

        /// <summary>
        /// Returns the stack as a list
        /// </summary>
        /// <param name="item">Name of push type</param>
        /// <returns>List representing the stack</returns>
        public static List<push.types.Type.PushTypeBase> ListOf(string item)
        {
            return StackOf(item).ToList();
        }

        /// <summary>
        /// Is the stack of designated type empty
        /// </summary>
        /// <param name="item">Name of push type</param>
        /// <returns>true if the stack is empty</returns>
        public static bool IsEmpty(string item)
        {
            return LengthOf(item) == 0;
        }

        /// <summary>
        ///  Peeks top of the designated stack
        /// </summary>
        /// <typeparam name="T">Underlying .NET type of the expected item</typeparam>
        /// <param name="item">Name of push type</param>
        /// <returns>The value of the top item</returns>
        public static T Top<T>(string item)
        {
            if (IsEmpty(item))
            {
                return default (T);
            }
            return Elem<T>(item, 0);
        }

        /// <summary>
        /// Length of the designated stack
        /// </summary>
        /// <param name="item">Name of push type</param>
        /// <returns>Integer length of the stack</returns>
        public static int LengthOf(string item)
        {
            return StackOf(item).length;
        }

        /// <summary>
        /// Returns the top of the CODE stack in string representation
        /// </summary>
        public static string GetTopCodeString(string name = "CODE")
        {
            if (IsEmpty(name))
            {
                return string.Empty;
            }
            return TestUtils.Top<Push>(name).StructuredFormatDisplay as string;
        }
        /// <summary>
        /// Runs Push parser and extracts results
        /// </summary>
        /// <param name="str">Push program</param>
        /// <returns>Parsed program or error tuple: (str, error)</returns>
        public static dynamic RunParser(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException("Please set str argument to something meaningful");
            }
            var pres = Parser.parsePushString(str);
            dynamic res = Parser.extractResult(pres);

            return res;

        }

        /// <summary>
        /// Returns the n-the element of the stack
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="stack">Name of the stack</param>
        /// <param name="index">Index of the element from the top of the stack</param>
        /// <returns>Contained value</returns>
        public static T Elem<T>(string stack, int index)
        {
            return ListOf(stack)[index].Raw<T>();
        }
    }
}

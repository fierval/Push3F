﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Core;
using FParsec;
using Microsoft.FSharp.Reflection;
using push.exceptions;

using push.parser;

namespace InterpreterTests
{
    [TestClass]
    public class ParserTest
    {
        string str;

        [TestMethod]
        [Description("Tests parsing a single boolean")]
        public void SimpleBoolean()
        {
            str = "TRUE";

            var res = RunPushParser(str);
            Assert.IsTrue(res.Item.Value);

            str = "FALSE";
            res = RunPushParser(str);
            Assert.IsFalse(res.Item.Value);
        }

        [TestMethod]
        [Description("Tests parsing a single integer value")]
        public void SimpleInt()
        {
            str = "12345";
            var res = RunPushParser(str);
            Assert.AreEqual<long>(12345L, res.Item.Value);
        }

        [TestMethod]
        [Description("Tests parsing a single float value")]
        public void SimpleFloat()
        {
            str = "   1234.53  ";
            var res = RunPushParser(str);
            Assert.AreEqual<double>(1234.53d, res.Item.Value);
        }

        [TestMethod]
        [Description("Tests the pushType parser directly")]
        public void SimpleTypeDirect()
        {
            this.str = "INTEGER";
            dynamic res = CharParsers.run((FSharpFunc<CharStream<Unit>, Reply<string>>)(Parser.pushType), str);
            Assert.AreEqual<string>(str, res.Item1);
        }

        [TestMethod]
        [Description("Parses out a simple operation directly")]
        public void SimpleOperationDirect()
        {
            this.str = "INTEGER.*";
            dynamic res = CharParsers.run(Parser.pushOperation, str);


            Assert.AreEqual<string>("Multiply", res.Item1.Item2.Name);
            Assert.AreEqual<string>("Integer", res.Item1.Item2.DeclaringType.Name);
        }

        [TestMethod]
        [Description("Parsing out a type")]
        public void SimpleTypeUsedAsIdentifierFailTest()
        {
            str = "   INTEGER  ";
            var res = RunPushParser(str);

            Type type = res.GetType();
            if (!FSharpType.IsTuple(type))
            {
                Assert.Fail("The parser should not have succeeded");
            }

            Assert.IsTrue(res.Item1.IndexOf("Reserved keyword") > -1);
        }

        [TestMethod]
        [Description("Parses out a simple operation directly")]
        public void SimpleOperation()
        {
            str = "\t   INTEGER.*  ";
            var res = RunPushParser(str);

            Assert.AreEqual<string>("Multiply", res.Item2.Name);
            Assert.AreEqual<string>("Integer", res.Item2.DeclaringType.Name);
        }

        [TestMethod]
        public void SimpleNameTest()
        {
            str = " SOMENAME  ";
            var res = RunPushParser(str);
            Assert.AreEqual<string>("SOMENAME", res.Item.Value);
        }

        [TestMethod]
        [Description("Parses out a simple a list of Push primitives")]
        public void SimpleList()
        {
            this.str = "(12 35 INTEGER.* true false)";
            var res = RunPushParser(this.str);

            Assert.AreEqual(12L, (long)res.Item[0].Item.Raw<Int64>());
        }

        [TestMethod]
        [Description("Parses out a recursed list")]
        public void RecursiveList()
        {
            this.str = @"( 12 (  35 61 INTEGER.+   )  (5.6 7.8 FLOAT.+)
                            INTEGER.*  )";
            var res = RunPushParser(this.str);

            Type type = res.GetType();
            if (FSharpType.IsTuple(type))
            {
                throw new PushExceptions.PushException(res.Item1);
            }
        }

        [TestMethod]
        [Description("Parses an empty list")]
        public void EmptyListTest()
        {
            this.str = @"()";
            var res = RunPushParser(this.str);

            Type type = res.GetType();
            if (FSharpType.IsTuple(type))
            {
                throw new PushExceptions.PushException(res.Item1);
            }
            Assert.AreEqual(0, res.Item.Length);

            this.str = @"(        )";
            var res1 = RunPushParser(this.str);

            Type type1 = res.GetType();
            if (FSharpType.IsTuple(type1))
            {
                throw new PushExceptions.PushException(res1.Item1);
            }
            Assert.AreEqual(0, res1.Item.Length);

        }

        [TestMethod]
        [Description("Incorrect type")]
        public void IncorrectType()
        {
            this.str = "(12 32 DOUBLE.*)";
            var res = RunPushParser(this.str);
            Assert.AreNotEqual<int>(-1, res.Item1.IndexOf("Unknown type: DOUBLE"));

        }

        dynamic RunPushParser(string str)
        {
            return TestUtils.RunParser(str);
        }
    }
}

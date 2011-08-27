using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Core;
using FParsec;

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
            str = "true";

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

            Assert.AreEqual<string>("INTEGER", res.Item1.Item1);
            Assert.AreEqual<string>("*", res.Item1.Item2);
        }

        [TestMethod]
        [Description("Parsing out a type")]
        public void SimpleType()
        {
            str = "   INTEGER  ";
            var res = RunPushParser(str);
            Assert.AreEqual<string>(str.Trim(), res.Item);
        }

        [TestMethod]
        [Description("Parses out a simple operation directly")]
        public void SimpleOperation()
        {
            str = "\t   INTEGER.*  ";
            var res = RunPushParser(str);
            Assert.AreEqual<string>(str.Trim(), res.Item1 + "." + res.Item2);
        }

        [TestMethod]
        [Description("Parses out a simple a list of Push primitives")]
        public void SimpleList()
        {
            this.str = "(12 35 INTEGER.* true false)";
            var res = RunPushParser(this.str);

            Assert.IsNotInstanceOfType(res, typeof(Int32));
        }

        [TestMethod]
        [Description("Parses out a recursed list")]
        public void RecursiveList()
        {
            this.str = @"( 12 (  35 61 INTEGER.+   )  (5.6 7.8 FLOAT.+)
                            INTEGER.*  )";
            var res = RunPushParser(this.str);

            Assert.IsNotInstanceOfType(res, typeof(Int32));
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
            if(string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException("Please set str argument to something meaningful");
            }
            var pres = Parser.parsePushString(str);
            var res = Parser.extractResult(pres);

            return res;
        }
    }
}

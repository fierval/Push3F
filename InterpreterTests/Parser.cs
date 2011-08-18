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
            Assert.IsTrue(res.Item);

            str = "FALSE";
            res = RunPushParser(str);
            Assert.IsFalse(res.Item);
        }

        [TestMethod]
        [Description("Tests parsing a single integer value")]
        public void SimpleInt()
        {
            str = "12345";
            var res = RunPushParser(str);
            Assert.AreEqual<long>(res.Item, 12345L);
        }

        [TestMethod]
        [Description("Tests parsing a single float value")]
        public void SimpleFloat()
        {
            str = "   1234.53  ";
            var res = RunPushParser(str);
            Assert.AreEqual<double>(res.Item, 1234.53d);
        }

        dynamic RunPushParser(string str)
        {
            if(string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException("Please set str argument to something meaningful");
            }
            var pres = Parser.parsePushString(str);
            var res = Parser.extractResult(pres);

            if(res.GetType() == typeof(Int32) && (Int32) res == Int32.MinValue)
            {
                throw new ApplicationException("Failed to parse string " + str);
            }

            return res;
        }
    }
}

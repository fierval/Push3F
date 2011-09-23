using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using push.core;
using push.parser;
using Push = push.parser.Ast.Push;

namespace InterpreterTests
{
    [TestClass]
    public class InsertTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void InsertSimple()
        {
            var prog = "(CODE.QUOTE (a b c d e f) CODE.QUOTE (k l) 2 CODE.INSERT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(a (k l) b c d e f)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void InsertZeroIndexTest()
        {
            var prog = "(CODE.QUOTE (a b c d e f) CODE.QUOTE (k l) -14 CODE.INSERT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(k l)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void InsertNoIntegerArgumentTest()
        {
            var prog = "(CODE.QUOTE (a b c d e f) CODE.QUOTE (k l) CODE.INSERT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(k l)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void InsertNoSecondArgumentTest()
        {
            var prog = "(CODE.QUOTE (a b c d e f) 5 CODE.INSERT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(a b c d e f)", TestUtils.GetTopCodeString());
        }

    }
}

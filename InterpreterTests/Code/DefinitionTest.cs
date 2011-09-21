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
    public class DefinitionTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void ConsimpleTest()
        {
            var prog = "(a CODE.QUOTE ((b c d) CODE.FIRST) CODE.DEFINE CODE.DEFINITION)";
            Program.ExecPush(prog);

            Assert.AreEqual("((b c d) CODE.CAR)", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
            Assert.AreEqual(2, TestUtils.LengthOf("CODE"));
        }

        [TestMethod]
        public void ConsEmptyListFirstTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE (a b c) CODE.CONS)";
            Program.ExecPush(prog);
            Assert.AreEqual("(() a b c)", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }
        [TestMethod]
        public void ConsEmptyListSecondTest()
        {
            var prog = "(CODE.QUOTE (a b) CODE.QUOTE () CODE.CONS)";
            Program.ExecPush(prog);

            Assert.AreEqual("((a b))", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }

        [TestMethod]
        public void ConsWithAtomTest()
        {
            var prog = "(CODE.QUOTE (a b) CODE.QUOTE c CODE.CONS)";
            Program.ExecPush(prog);

            Assert.AreEqual("((a b) c)", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }

    }
}

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
    public class DiscrepancyTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void DiscrepancySimpleTest()
        {
            var prog = "(CODE.QUOTE (b c d e f g) CODE.QUOTE (b c d e f g) CODE.DISCREPANCY)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void DiscrepancyNonZeroTest()
        {
            var prog = "(CODE.QUOTE (b b c c c d d e f g g) CODE.QUOTE (b c d e f g) CODE.DISCREPANCY)";
            Program.ExecPush(prog);

            Assert.AreEqual(5, TestUtils.Top<long>("INTEGER"));
        }
        [TestMethod]
        public void DiscrepancyListsWithinListsTest()
        {
            var prog = "(CODE.QUOTE (b c (d e) f g) CODE.QUOTE (b c (d e) f g) CODE.DISCREPANCY)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
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

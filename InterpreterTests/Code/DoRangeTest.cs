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
    public class DoRangeTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        [Description("Computes factorial of a number pushed on top of the integer stack")]
        public void DoRangeSimple()
        {
            var prog = "(1 5 CODE.QUOTE INTEGER.* CODE.DO*RANGE)";
            Program.ExecPush(prog);

            Assert.AreEqual(120, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void DoEmptyRangeTest()
        {
            var prog = "(CODE.QUOTE INTEGER.* CODE.DO*RANGE)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
            Assert.AreEqual(2, TestUtils.LengthOf("CODE"));
        }

        [TestMethod]
        public void OnlyOneIntegerArgDoRangeTest()
        {
            var prog = "(CODE.QUOTE INTEGER.* 1 CODE.DO*RANGE)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
            Assert.AreEqual(2, TestUtils.LengthOf("CODE"));
        }

        [TestMethod]
        [Description("Table of squares from 4 to 8")]
        public void ListOfOperationsDoRangeTest()
        {
            var prog = "(CODE.QUOTE (INTEGER.DUP INTEGER.*) 4 8 CODE.DO*RANGE)";
            Program.ExecPush(prog);

            Assert.AreEqual(64, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(49, TestUtils.StackOf("INTEGER").Item[1].Raw<long>());
            Assert.AreEqual(5, TestUtils.LengthOf("INTEGER"));
        }

        [TestMethod]
        [Description("Table of squares from 4 to 8, backwards")]
        public void ListOfOperationsDoRangeBackwardsTest()
        {
            var prog = "(CODE.QUOTE (INTEGER.DUP INTEGER.*) 8 4 CODE.DO*RANGE)";
            Program.ExecPush(prog);

            Assert.AreEqual(16, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(25, TestUtils.StackOf("INTEGER").Item[1].Raw<long>());
            Assert.AreEqual(5, TestUtils.LengthOf("INTEGER"));
        }

    }
}

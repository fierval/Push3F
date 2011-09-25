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
    public class DoCountTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        [Description("Computes factorial of a number pushed on top of the integer stack")]
        public void DoCountSimple()
        {
            var prog = "(5 CODE.QUOTE (INTEGER.DUP INTEGER.*) CODE.DO*COUNT)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(16, TestUtils.Elem<long>("INTEGER", 4));
        }

        [TestMethod]
        public void DoEmptyCountTest()
        {
            var prog = "(CODE.QUOTE INTEGER.* CODE.DO*COUNT)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
            Assert.AreEqual(2, TestUtils.LengthOf("CODE"));
        }
    }
}

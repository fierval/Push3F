﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class ExecCountTest
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
            var prog = "(5 EXEC.DO*COUNT (INTEGER.DUP INTEGER.*))";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(16, TestUtils.Elem<long>("INTEGER", 4));
        }

        [TestMethod]
        public void DoEmptyCountTest()
        {
            var prog = "(EXEC.DO*COUNT INTEGER.*)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
        }
    }
}

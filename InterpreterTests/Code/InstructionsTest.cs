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
    public class InstructionsTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void InsertSimple()
        {
            var prog = "CODE.INSTRUCTIONS";
            Program.ExecPush(prog);

            Assert.AreEqual(162, TestUtils.LengthOf("CODE"));
        }
    }
}

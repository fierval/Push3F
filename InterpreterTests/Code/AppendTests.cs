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
    public class AppendTests
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void AppendTwoListsTest()
        {
            var prog = "(CODE.QUOTE (a b) CODE.QUOTE (f g) CODE.APPEND)";
            Program.ExecPush(prog);

            var res = TestUtils.Top<Push>("CODE");

            var str = res.StructuredFormatDisplay;

            Assert.AreEqual("(a b f g)", str);
        }

        [TestMethod]
        public void AppendNameListTest()
        {
            var prog = "(CODE.QUOTE a CODE.QUOTE (f g) CODE.APPEND)";
            Program.ExecPush(prog);

            var res = TestUtils.Top<Push>("CODE");

            var str = res.StructuredFormatDisplay;

            Assert.AreEqual("(a f g)", str);
        }

        [TestMethod]
        public void AppendTwoEmptyListsTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE () CODE.APPEND)";
            Program.ExecPush(prog);

            var res = TestUtils.Top<Push>("CODE");

            var str = res.StructuredFormatDisplay;

            Assert.AreEqual("()", str);
        }

    }
}

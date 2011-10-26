using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;
using Push = push.types.Ast.Push;

namespace InterpreterTests
{
    [TestClass]
    public class CdrTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void CdrSimpleTest()
        {
            var prog = "(CODE.QUOTE (a b c d) CODE.CDR)";
            Program.ExecPush(prog);

            Assert.AreEqual("(b c d)", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }

        [TestMethod]
        public void CdrEmptyListTest()
        {
            var prog = "(CODE.QUOTE () CODE.FIRST)";
            Program.ExecPush(prog);
            Assert.AreEqual("()", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }
        [TestMethod]
        public void CdrFirstListTest()
        {
            var prog = "(CODE.QUOTE ((a b) (c d) e f) CODE.CDR)";
            Program.ExecPush(prog);

            Assert.AreEqual("((c d) e f)", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }

        [TestMethod]
        public void TwoCdrsTest()
        {
            var prog = "(CODE.QUOTE ((a b) (c d) e f) CODE.CDR CODE.REST)";
            Program.ExecPush(prog);

            Assert.AreEqual("(e f)", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;
using Push = push.parser.Ast.Push;

namespace InterpreterTests
{
    [TestClass]
    public class CarTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void CarSimpleTest()
        {
            var prog = "(CODE.QUOTE (a b c d) CODE.CAR)";
            Program.ExecPush(prog);

            Assert.AreEqual("a", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }

        [TestMethod]
        public void CarEmptyListTest()
        {
            var prog = "(CODE.QUOTE () CODE.FIRST)";
            Program.ExecPush(prog);
            Assert.AreEqual("()", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }
        [TestMethod]
        public void CarFirstListTest()
        {
            var prog = "(CODE.QUOTE ((a b) (c d) e f) CODE.CAR)";
            Program.ExecPush(prog);

            Assert.AreEqual("(a b)", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }

        [TestMethod]
        public void TwoCarsTest()
        {
            var prog = "(CODE.QUOTE ((a b) (c d) e f) CODE.CAR CODE.FIRST)";
            Program.ExecPush(prog);

            Assert.AreEqual("a", TestUtils.Top<Push>("CODE").StructuredFormatDisplay);
        }

    }
}

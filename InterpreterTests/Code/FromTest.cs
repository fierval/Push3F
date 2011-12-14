using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class FromTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void FromIntegerTest()
        {
            var prog = "(1 CODE.FROMINTEGER)";
            Program.ExecPush(prog);

            Assert.AreEqual("1", TestUtils.GetTopCodeString());
        }

         [TestMethod]
        public void FromBoolTest()
        {
            var prog = "(TRUE CODE.FROMBOOLEAN)";
            Program.ExecPush(prog);

            Assert.AreEqual("True", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void FromFloatTest()
        {
            var prog = "(9.32 CODE.FROMFLOAT)";
            Program.ExecPush(prog);

            Assert.AreEqual("9.32", TestUtils.GetTopCodeString());

        }

        [TestMethod]
        public void FromNameTest()
        {
            var prog = "(RandomName CODE.FROMNAME)";
            Program.ExecPush(prog);

            Assert.AreEqual("RandomName", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void ExtractWithNegativeArgumentTest()
        {
            var prog = "(-10 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("c", TestUtils.GetTopCodeString());
        }

    }
}

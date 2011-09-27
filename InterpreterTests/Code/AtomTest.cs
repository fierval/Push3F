using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class AtomTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void AtomFalseTest()
        {
            var prog = "(CODE.QUOTE (a b) CODE.ATOM)";
            Program.ExecPush(prog);

            Assert.IsFalse(TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void AtomEmptyFalseTest()
        {
            var prog = "(CODE.QUOTE () CODE.ATOM)";
            Program.ExecPush(prog);

            Assert.IsFalse(TestUtils.Top<bool>("BOOLEAN"));
        }
        [TestMethod]
        public void AtomTrueTest()
        {
            var prog = "(CODE.QUOTE 12 CODE.ATOM)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.Top<bool>("BOOLEAN"));
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class DoTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void DoSimple()
        {
            var prog = "(CODE.QUOTE (5 INTEGER.+) CODE.QUOTE (3 5 INTEGER.*) CODE.DO CODE.DO)";
            Program.ExecPush(prog);

            Assert.AreEqual(20, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void DoEmptyTest()
        {
            var prog = "(CODE.DO)";
            Program.ExecPush(prog);

            Assert.AreEqual(string.Empty, TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void DoStarTest()
        {
            var prog = "(CODE.QUOTE (5 INTEGER.+) CODE.QUOTE (3 5 INTEGER.*) CODE.DO* CODE.DO*)";
            Program.ExecPush(prog);

            Assert.AreEqual(20, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void DoStarEmptyTest()
        {
            var prog = "(CODE.POP CODE.DO*)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("CODE"));
        }
    }
}

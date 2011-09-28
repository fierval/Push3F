using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class ExecTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void DoSimple()
        {
            var prog = "(EXEC.DO (3 5 INTEGER.*) EXEC.DO (5 INTEGER.+))";
            Program.ExecPush(prog);

            Assert.AreEqual(20, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void DoEmptyTest()
        {
            var prog = "(EXEC.DO)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
        }

        [TestMethod]
        public void InsertNoIntegerArgumentTest()
        {
            var prog = "(EXEC.DO* (3 5 INTEGER.*) EXEC.DO* (5 INTEGER.+))";
            Program.ExecPush(prog);

            Assert.AreEqual(25, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void DoStarEmptyTest()
        {
            var prog = "(EXEC.DO*)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
        }
    }
}

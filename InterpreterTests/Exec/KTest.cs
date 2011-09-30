using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class KTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void KSimple()
        {
            var prog = "(2 3 EXEC.K CODE.QUOTE INTEGER.+)";
            Program.ExecPush(prog);

            Assert.AreEqual(3, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void KEmptyTest()
        {
            var prog = "(EXEC.K a)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
        }
    }
}

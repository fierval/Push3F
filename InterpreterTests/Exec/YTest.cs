using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class YTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void YSimple()
        {
            var prog = "(5 2.0 EXEC.Y (INTEGER.DUP INTEGER.* 1. FLOAT.- FLOAT.DUP 0. FLOAT.> EXEC.IF () EXEC.POP))";
            Program.ExecPush(prog);

            Assert.AreEqual(625, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void YEmptyTest()
        {
            var prog = "(EXEC.Y)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
        }

   }
}

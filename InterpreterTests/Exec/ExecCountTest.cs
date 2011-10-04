using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class ExecCountTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void DoCountSimple()
        {
            var prog = "(5 EXEC.DO*COUNT (INTEGER.DUP INTEGER.*))";
            Program.ExecPush(prog);

            Assert.AreEqual(16, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(0, TestUtils.Elem<long>("INTEGER", 4));
        }

        [TestMethod]
        public void DoEmptyCountTest()
        {
            var prog = "(EXEC.DO*COUNT INTEGER.*)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
        }
    }
}

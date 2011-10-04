using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class DoCountTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void DoCountSimple()
        {
            var prog = "(5 CODE.QUOTE (INTEGER.DUP INTEGER.*) CODE.DO*COUNT)";
            Program.ExecPush(prog);

            Assert.AreEqual(16, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(0, TestUtils.Elem<long>("INTEGER", 4));
        }

        [TestMethod]
        public void DoEmptyCountTest()
        {
            var prog = "(CODE.QUOTE INTEGER.* CODE.DO*COUNT)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
            Assert.AreEqual(2, TestUtils.LengthOf("CODE"));
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class InstructionsTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void InstructionsSimple()
        {
            var prog = "CODE.INSTRUCTIONS";
            Program.ExecPush(prog);

            Assert.AreEqual(181, TestUtils.LengthOf("CODE"));
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class STest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void SSimple()
        {
            var prog = "(EXEC.S CODE.QUOTE b c)";
            Program.ExecPush(prog);

            Assert.AreEqual("c", TestUtils.GetTopCodeString());
            Assert.AreEqual("c", TestUtils.Top<string>("NAME"));
            Assert.AreEqual("b", TestUtils.Elem<string>("NAME", 1));
        }

        [TestMethod]
        public void SEmptyTest()
        {
            var prog = "(EXEC.S a)";
            Program.ExecPush(prog);

            Assert.AreEqual("a", TestUtils.Top<string>("NAME"));
        }
    }
}

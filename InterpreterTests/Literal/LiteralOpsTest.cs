using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class LiteralTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void EqTest()
        {
            var prog = "(\"a\" \"a\" LITERAL.= )";
            Program.ExecPush(prog);

            Assert.AreEqual(true, TestUtils.Top<bool>("BOOLEAN"));
        }
      }
}

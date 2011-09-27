using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class DoTimesTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        [Description("Computes the Fibonacci sequence")]
        public void DoTimesSimple()
        {
            var prog = "(1 1 CODE.QUOTE (INTEGER.DUP 2 INTEGER.YANKDUP INTEGER.+) 5 CODE.DO*TIMES)";
            Program.ExecPush(prog);

            Assert.AreEqual(13, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(7, TestUtils.LengthOf("INTEGER"));
        }

        [TestMethod]
        public void DoEmptyTimesTest()
        {
            var prog = "(CODE.QUOTE INTEGER.* CODE.DO*TIMES)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
            Assert.AreEqual(2, TestUtils.LengthOf("CODE"));
        }
    }
}

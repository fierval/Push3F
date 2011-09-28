using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class ExecTimesTest
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
            var prog = "(1 1 5 EXEC.DO*TIMES (INTEGER.DUP 2 INTEGER.YANKDUP INTEGER.+) )";
            Program.ExecPush(prog);

            Assert.AreEqual(13, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(7, TestUtils.LengthOf("INTEGER"));
        }

        [TestMethod]
        public void DoEmptyTimesTest()
        {
            var prog = "(EXEC.DO*TIMES INTEGER.*)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class InsertTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void InsertSimple()
        {
            var prog = "(CODE.QUOTE (a b c d e f) CODE.QUOTE (k l) 2 CODE.INSERT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(a (k l) c d e f)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void InsertLayerd()
        {
            var prog = "(CODE.QUOTE (a b (c d e) f) CODE.QUOTE (k l) 5 CODE.INSERT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(a b (c (k l) e) f)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void InsertZeroIndexTest()
        {
            var prog = "(CODE.QUOTE (a b c d e f) CODE.QUOTE (k l) -14 CODE.INSERT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(k l)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void InsertNoIntegerArgumentTest()
        {
            var prog = "(CODE.QUOTE (a b c d e f) CODE.QUOTE (k l) CODE.INSERT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(k l)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void InsertNoSecondArgumentTest()
        {
            var prog = "(CODE.QUOTE (a b c d e f) 5 CODE.INSERT)";
            Program.ExecPushProgram(prog, Program.ExecutionFlags.None);

            Assert.AreEqual("(a b c d e f)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void InsertIntoEmptyTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE (a b c d e f) 5 CODE.INSERT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(a b c d e f)", TestUtils.GetTopCodeString());
        }


    }
}

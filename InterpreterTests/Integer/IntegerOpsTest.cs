using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class IntegerTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void LtTest()
        {
            var prog = "(5 3 INTEGER.< EXEC.IF 5 3)";
            Program.ExecPush(prog);

            Assert.AreEqual(3, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void GtTest()
        {
            var prog = "(5 3 INTEGER.> EXEC.IF 5 3)";
            Program.ExecPush(prog);

            Assert.AreEqual(5, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void EqTest()
        {
            var prog = "(5 5 INTEGER.= EXEC.IF 15 3)";
            Program.ExecPush(prog);

            Assert.AreEqual(15, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void DivTest()
        {
            var prog = "(5 0 INTEGER./)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));

            prog = "(10 5 INTEGER./)";
            Program.ExecPush(prog);

            Assert.AreEqual(2, TestUtils.Top<long>("INTEGER"));
       }

        [TestMethod]
        public void ModTest()
        {
            var prog = "(5 0 INTEGER.%)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));

            prog = "(10 6 INTEGER.%)";
            Program.ExecPush(prog);

            Assert.AreEqual(4, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void MultTest()
        {
            var prog = "(5 6 INTEGER.*)";
            Program.ExecPush(prog);

            Assert.AreEqual(30, TestUtils.Top<long>("INTEGER"));

        }

        [TestMethod]
        public void MinMaxTest()
        {
            var prog = "(3 5 INTEGER.MIN)";
            Program.ExecPush(prog);

            Assert.AreEqual(3, TestUtils.Top<long>("INTEGER"));

            TypeFactory.stockTypes.cleanAllStacks();

            prog = "(3 10 INTEGER.MAX)";
            Program.ExecPush(prog);
            Assert.AreEqual(10, TestUtils.Top<long>("INTEGER"));

        }

        [TestMethod]
        public void RandTest()
        {
            var prog = "(5 EXEC.DO*TIMES INTEGER.RAND)";
            Program.ExecPush(prog);

            Assert.AreEqual(5, TestUtils.LengthOf("INTEGER"));
        }

        [TestMethod]
        public void FromBooleanTest()
        {
            var prog = "(TRUE INTEGER.FROMBOOLEAN)";
            Program.ExecPush(prog);

            Assert.AreEqual(1, TestUtils.Top<long>("INTEGER"));

            prog = "(FALSE INTEGER.FROMBOOLEAN)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void FromFloatTest()
        {
            var prog = "(5.6 INTEGER.FROMFLOAT)";
            Program.ExecPush(prog);

            Assert.AreEqual(5, TestUtils.Top<long>("INTEGER"));
        }
    }
}

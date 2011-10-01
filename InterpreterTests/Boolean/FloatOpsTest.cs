using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class BooleanTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void LtTest()
        {
            var prog = "(5. 3. FLOAT.< EXEC.IF 5. 3.)";
            Program.ExecPush(prog);

            Assert.AreEqual(3.0, TestUtils.Top<double>("FLOAT"));
        }

        [TestMethod]
        public void GtTest()
        {
            var prog = "(5. 3. FLOAT.> EXEC.IF 5. 3.)";
            Program.ExecPush(prog);

            Assert.AreEqual(5.0, TestUtils.Top<double>("FLOAT"));
        }

        [TestMethod]
        public void EqTest()
        {
            var prog = "(5. 5. FLOAT.= EXEC.IF 15. 3.)";
            Program.ExecPush(prog);

            Assert.AreEqual(15.0, TestUtils.Top<double>("FLOAT"));
        }

        [TestMethod]
        public void DivTest()
        {
            var prog = "(5. 0. FLOAT./)";
            Program.ExecPush(prog);

            Assert.AreEqual(0.0, TestUtils.Top<double>("FLOAT"));

            prog = "(10. 5. FLOAT./)";
            Program.ExecPush(prog);

            Assert.AreEqual(2.0, TestUtils.Top<double>("FLOAT"));
       }

        [TestMethod]
        public void ModTest()
        {
            var prog = "(5. 0. FLOAT.%)";
            Program.ExecPush(prog);

            Assert.AreEqual(0.0, TestUtils.Top<double>("FLOAT"));

            prog = "(10. 6. FLOAT.%)";
            Program.ExecPush(prog);

            Assert.AreEqual(4.0, TestUtils.Top<double>("FLOAT"));
        }

        [TestMethod]
        public void MultTest()
        {
            var prog = "(5. 6. FLOAT.*)";
            Program.ExecPush(prog);

            Assert.AreEqual(30.0, TestUtils.Top<double>("FLOAT"));

        }

        [TestMethod]
        public void MinMaxTest()
        {
            var prog = "(3. 5. FLOAT.MIN)";
            Program.ExecPush(prog);

            Assert.AreEqual(3.0, TestUtils.Top<double>("FLOAT"));

            TypeFactory.stockTypes.cleanAllStacks();

            prog = "(3. 10. FLOAT.MAX)";
            Program.ExecPush(prog);
            Assert.AreEqual(10.0, TestUtils.Top<double>("FLOAT"));

        }

        [TestMethod]
        public void RandTest()
        {
            var prog = "(5 EXEC.DO*TIMES FLOAT.RAND)";
            Program.ExecPush(prog);

            Assert.AreEqual(5.0, TestUtils.LengthOf("FLOAT"));
        }

        [TestMethod]
        public void FromBooleanTest()
        {
            var prog = "(TRUE FLOAT.FROMBOOLEAN)";
            Program.ExecPush(prog);

            Assert.AreEqual(1.0, TestUtils.Top<double>("FLOAT"));

            prog = "(FALSE FLOAT.FROMBOOLEAN)";
            Program.ExecPush(prog);

            Assert.AreEqual(0.0, TestUtils.Top<double>("FLOAT"));
        }

        [TestMethod]
        public void FromIntegerTest()
        {
            var prog = "(5 FLOAT.FROMINTEGER)";
            Program.ExecPush(prog);

            Assert.AreEqual(5.0, TestUtils.Top<double>("FLOAT"));
        }
    }
}

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
        public void EqTest()
        {
            var prog = "(FALSE TRUE BOOLEAN.= )";
            Program.ExecPush(prog);

            Assert.AreEqual(false, TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void OrTest()
        {
            var prog = "(FALSE TRUE BOOLEAN.OR )";
            Program.ExecPush(prog);

            Assert.AreEqual(true, TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void AndTest()
        {
            var prog = "(FALSE TRUE BOOLEAN.AND )";
            Program.ExecPush(prog);

            Assert.AreEqual(false, TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void NotTest()
        {
            var prog = "(FALSE BOOLEAN.NOT )";
            Program.ExecPush(prog);

            Assert.AreEqual(true, TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void RandTest()
        {
            var prog = "(BOOLEAN.RAND)";
            Program.ExecPush(prog);

            Assert.AreEqual(1, TestUtils.LengthOf("BOOLEAN"));
        }

        [TestMethod]
        public void FromFloatTest()
        {
            var prog = "(5.0 BOOLEAN.FROMFLOAT)";
            Program.ExecPush(prog);

            Assert.AreEqual(true, TestUtils.Top<bool>("BOOLEAN"));

            prog = "(0.0 BOOLEAN.FROMFLOAT)";
            Program.ExecPush(prog);

            Assert.AreEqual(false, TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void FromIntegerTest()
        {
            var prog = "(5 BOOLEAN.FROMINTEGER)";
            Program.ExecPush(prog);

            Assert.AreEqual(true, TestUtils.Top<bool>("BOOLEAN"));

            prog = "(0 BOOLEAN.FROMINTEGER)";
            Program.ExecPush(prog);

            Assert.AreEqual(false, TestUtils.Top<bool>("BOOLEAN"));
        }
    }
}

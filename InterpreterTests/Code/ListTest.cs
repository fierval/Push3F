using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class LengthTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void LengthSimpleTest()
        {
            var prog = "(CODE.QUOTE a CODE.LENGTH)";
            Program.ExecPush(prog);

            Assert.AreEqual(1, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void LengthNestedTest()
        {
            var prog = "(CODE.QUOTE (d a  (b (c ( d)))) CODE.LENGTH)";
            Program.ExecPush(prog);

            Assert.AreEqual(3, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void LengthEmptyTest()
        {
            var prog = "(CODE.QUOTE () CODE.LENGTH)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void LengthNoneTest()
        {
            // The first item is pushed onto the code stack by default, so pop it.
            var prog = "(CODE.POP CODE.LENGTH)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.LengthOf("INTEGER"));
        }

    }
}

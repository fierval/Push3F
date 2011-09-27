using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class ListTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void ListTwoEmptyTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE () CODE.LIST)";
            Program.ExecPush(prog);

            Assert.AreEqual("(() ())", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void ListSimpleTest()
        {
            var prog = "(CODE.QUOTE a CODE.QUOTE b CODE.LIST)";
            Program.ExecPush(prog);

            Assert.AreEqual("(a b)", TestUtils.GetTopCodeString());
        }


        [TestMethod]
        public void ListNoneTest()
        {
            // The first item is pushed onto the code stack by default, so pop it.
            var prog = "(CODE.POP CODE.LIST)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.LengthOf("CODE"));
        }

    }
}

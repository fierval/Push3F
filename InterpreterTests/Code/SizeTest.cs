using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using Code = push.types.stock.StockTypesCode.Code;
using Push = push.types.Ast.Push;
using push.core;

namespace InterpreterTests
{
    [TestClass]
    public class SizeTest
    {

        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }
 
        [TestMethod]
        [Description ("Tests Container operation: most basic test")]
        public void SimpleSizeTest()
        {
            var prog = "(CODE.QUOTE(a b (c d) e) CODE.SIZE)";
            Program.ExecPush(prog);
            Assert.AreEqual(7, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void SizeEmptyListTest()
        {
            var prog = "(CODE.QUOTE () CODE.SIZE)";
            Program.ExecPush(prog);
            Assert.AreEqual(1, TestUtils.Top<long>("INTEGER"));
        }


        [TestMethod]
        public void SizeValueTest()
        {
            var prog = "(CODE.QUOTE 10 CODE.SIZE)";
            Program.ExecPush(prog);
            Assert.AreEqual(1, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void SizeEmptyTest()
        {
            var prog = "(CODE.POP CODE.SIZE)";
            Program.ExecPush(prog);
            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
        }

    }
}

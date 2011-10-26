using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using Code = push.types.stock.StockTypesCode.Code;
using Push = push.types.Ast.Push;
using push.core;

namespace InterpreterTests
{
    [TestClass]
    public class PositionTest
    {

        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }
 
        [TestMethod]
        public void SimplePositionTest()
        {
            var prog = @"(CODE.QUOTE(a b (c d) e) 
                            CODE.QUOTE(c d) CODE.POSITION)";
            Program.ExecPush(prog);
            Assert.AreEqual(2, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void PositionOfEmptyListTest()
        {
            var prog = "(CODE.QUOTE () a CODE.POSITION)";
            Program.ExecPush(prog);
            Assert.AreEqual(1, TestUtils.Top<long>("INTEGER"));
        }


        [TestMethod]
        public void EmptyArgListTest()
        {
            var prog = "(CODE.POSITION)";
            Program.ExecPush(prog);
            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
        }

    }
}

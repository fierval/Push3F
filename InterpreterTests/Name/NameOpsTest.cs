using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;
using System.Linq;

namespace InterpreterTests
{
    [TestClass]
    public class NameTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void EqTest()
        {
            var prog = "(ONE ONE NAME.=)";
            Program.ExecPush(prog);

            Assert.AreEqual(true, TestUtils.Top<bool>("BOOLEAN"));

            prog = "(ONE TWO NAME.=)";
            Program.ExecPush(prog);

            Assert.AreEqual(false, TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void RandTest()
        {
            var prog = "(NAME.RAND)";
            Program.ExecPush(prog);

            Assert.AreEqual(1, TestUtils.LengthOf("NAME"));

        }

        [TestMethod]
        public void QuoteTest()
        {
            var prog = "(a 5 INTEGER.DEFINE a NAME.QUOTE a)";
            Program.ExecPush(prog);

            Assert.AreEqual(5, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual("a", TestUtils.Top<string>("NAME"));
        }

        [TestMethod]
        public void RandomPositionTest()
        {
            var prog = @"(a 5 INTEGER.DEFINE b 6 INTEGER.DEFINE d 7 INTEGER.DEFINE e 8 INTEGER.DEFINE NAME.RANDBOUNDNAME CODE.DEFINITION)";
            Program.ExecPush(prog);

            Assert.IsTrue(Enumerable.Range(5, 3).Where(i => i == (int) TestUtils.Top<long>("INTEGER")).Count() == 1);
        }

    }
}

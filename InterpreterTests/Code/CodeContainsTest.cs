using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using Program = push.core.Program;

namespace InterpreterTests
{
    [TestClass]
    public class CodeContainsTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }
 
        [TestMethod]
        public void ContainsSimpleTest()
        {
            var prog = "(CODE.QUOTE (a b (c d) e) CODE.QUOTE (c d) CODE.CONTAINS)";
            Program.ExecPush(prog);
            Assert.IsTrue(TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void ContainsEmptyTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE () CODE.CONTAINS)";
            Program.ExecPush(prog);
            Assert.IsTrue(TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void ContainsNameTest()
        {
            var prog = "(CODE.QUOTE a CODE.QUOTE b CODE.CONTAINS)";
            Program.ExecPush(prog);
            Assert.IsFalse(TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void ContainsEmptyNameTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE (a) CODE.CONTAINS)";
            Program.ExecPush(prog);
            Assert.IsFalse(TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void ContainsNameEmptyTest()
        {
            var prog = "(CODE.QUOTE (a) CODE.QUOTE () CODE.CONTAINS)";
            Program.ExecPush(prog);
            Assert.IsTrue(TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void ContainsDeepTest()
        {
            var prog = "(CODE.QUOTE (a (b c ( d e) k)) CODE.QUOTE (d e) CODE.CONTAINS)";
            Program.ExecPush(prog);
            Assert.IsTrue(TestUtils.Top<bool>("BOOLEAN"));
        }

    }
}

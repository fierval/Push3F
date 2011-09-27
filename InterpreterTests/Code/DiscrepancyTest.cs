using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class DiscrepancyTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void DiscrepancySimpleTest()
        {
            var prog = "(CODE.QUOTE (b c d e f g) CODE.QUOTE (b c d e f g) CODE.DISCREPANCY)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void DiscrepancyNonZeroTest()
        {
            var prog = "(CODE.QUOTE (b b c c c d d e f g g) CODE.QUOTE (b c d e f g) CODE.DISCREPANCY)";
            Program.ExecPush(prog);

            Assert.AreEqual(5, TestUtils.Top<long>("INTEGER"));
        }
        [TestMethod]
        public void DiscrepancyListsWithinListsTest()
        {
            var prog = "(CODE.QUOTE (b c (d e) f g) CODE.QUOTE (b c (d e) f g) CODE.DISCREPANCY)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void DiscrepancyListsWithinListsDifferentTest()
        {
            var prog = "(CODE.QUOTE (b c (d e) f g) CODE.QUOTE (b c (e d) f g) CODE.DISCREPANCY)";
            Program.ExecPush(prog);

            Assert.AreEqual(2, TestUtils.Top<long>("INTEGER"));

        }

        [TestMethod]
        public void OneEmptyListTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE (b c (e d) f g) CODE.DISCREPANCY)";
            Program.ExecPush(prog);

            Assert.AreEqual(5, TestUtils.Top<long>("INTEGER"));


        }

        [TestMethod]
        public void TwoEmptyListsTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE () CODE.DISCREPANCY)";
            Program.ExecPush(prog);

            Assert.AreEqual(0, TestUtils.Top<long>("INTEGER"));
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class ExecRangeTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        [Description("Computes factorial of a number pushed on top of the integer stack")]
        public void ExecRangeSimple()
        {
            var prog = "(1 5 EXEC.DO*RANGE INTEGER.*)";
            Program.ExecPush(prog);

            Assert.AreEqual(120, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void ExecEmptyRangeTest()
        {
            var prog = "(EXEC.DO*RANGE INTEGER.*)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
        }

        [TestMethod]
        public void OnlyOneIntegerArgDoRangeTest()
        {
            var prog = "(1 EXEC.DO*RANGE INTEGER.*)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));
            Assert.AreEqual(1, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        [Description("Table of squares from 4 to 8")]
        public void ListOfOperationsDoRangeTest()
        {
            var prog = "(4 8 EXEC.DO*RANGE (INTEGER.DUP INTEGER.*))";
            Program.ExecPush(prog);

            Assert.AreEqual(64, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(49, TestUtils.StackOf("INTEGER").Item[1].Raw<long>());
            Assert.AreEqual(5, TestUtils.LengthOf("INTEGER"));
        }

        [TestMethod]
        [Description("Table of squares from 4 to 8, backwards")]
        public void ListOfOperationsDoRangeBackwardsTest()
        {
            var prog = "(8 4 EXEC.DO*RANGE (INTEGER.DUP INTEGER.*))";
            Program.ExecPush(prog);

            Assert.AreEqual(16, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(25, TestUtils.StackOf("INTEGER").Item[1].Raw<long>());
            Assert.AreEqual(5, TestUtils.LengthOf("INTEGER"));
        }

    }
}

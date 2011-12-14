using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class ExtractTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void ExtractSimpleTest()
        {
            var prog = "(1 CODE.QUOTE (b c d e f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("b", TestUtils.GetTopCodeString());
        }

         [TestMethod]
        public void ExtractEntireListTest()
        {
            var prog = "(0 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(b c (d e) f g)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void ExtractIndexByModuleTest()
        {
            var prog = "(9 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("b", TestUtils.GetTopCodeString());

        }

        [TestMethod]
        public void ExtractFromEmptyListTest()
        {
            var prog = "(9 CODE.QUOTE () CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("()", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void ExtractAllWithNegativeArgumentTest()
        {
            var prog = "(-8 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(b c (d e) f g)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void ExtractElementWithNegativeArgumentTest()
        {
            var prog = "(-10 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("c", TestUtils.GetTopCodeString());
        }


        [TestMethod]
        public void ExtractListArgumentTest()
        {
            var prog = "(3 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(d e)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void ExtractInDepthArgumentTest()
        {
            var prog = "(4 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("d", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void ExtractPostInDepthArgumentTest()
        {
            var prog = "(7 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("g", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void ExtractNoIntegerArgumentTest()
        {
            var prog = "(CODE.QUOTE (b c) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(b c)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void ExtractCompound()
        {
            var prog = "(7 CODE.QUOTE (a (b (c (d e) f) g) h) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("d", TestUtils.GetTopCodeString());
            
        }

        [TestMethod]
        public void ExtractCompoundList()
        {
            var prog = "(6 CODE.QUOTE (a (b (c (d e) f) g) h) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(d e)", TestUtils.GetTopCodeString());

        }

        [TestMethod]
        public void ExtractCompoundLast()
        {
            var prog = "(7 CODE.QUOTE (a (b (c d) e)) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("e", TestUtils.GetTopCodeString());

        }

    }
}

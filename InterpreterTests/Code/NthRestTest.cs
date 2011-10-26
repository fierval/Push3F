using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using Push = push.types.Ast.Push;
using push.core;

namespace InterpreterTests
{
    [TestClass]
    public class NthRestTest
    {

        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }
 
        [TestMethod]
        [Description ("Tests Container operation: most basic test")]
        public void SimpleNthRestTest()
        {
            var prog = "(CODE.QUOTE(a b (c d) e) 3 CODE.NTHREST)";
            Program.ExecPush(prog);
            Assert.AreEqual("e", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void NthRestEmptyTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE () 0 CODE.NTHREST)";
            Program.ExecPush(prog);
            Assert.AreEqual("()", TestUtils.GetTopCodeString());
        }


        [TestMethod]
        public void NthRestValueTest()
        {
            var prog = "(CODE.QUOTE a 10 CODE.NTHREST)";
            Program.ExecPush(prog);
            Assert.AreEqual("()", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void NthRestValueValueItselfResultTest()
        {
            var prog = "(CODE.QUOTE a 1 CODE.NTHREST)";
            Program.ExecPush(prog);
            Assert.AreEqual("()", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void NoIntegerArgumentNthRestTest()
        {
            var prog = "(CODE.QUOTE (a b c) CODE.NTHREST)";
            Program.ExecPush(prog);
            Assert.AreEqual(2, TestUtils.LengthOf("CODE"));
        }

        [TestMethod]
        public void NoCodeArgumentNthRestTest()
        {
            var prog = "(1 CODE.NTHREST)";
            Program.ExecPushProgram(prog, Program.ExecutionFlags.None);
            Assert.AreEqual(0, TestUtils.LengthOf("CODE"));
            Assert.AreEqual(1L, TestUtils.Top<long>("INTEGER"));
        }


    }
}

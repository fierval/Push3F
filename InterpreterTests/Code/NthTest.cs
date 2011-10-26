using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using Push = push.types.Ast.Push;
using push.core;

namespace InterpreterTests
{
    [TestClass]
    public class NthTest
    {

        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }
 
        [TestMethod]
        [Description ("Tests Container operation: most basic test")]
        public void SimpleNthTest()
        {
            var prog = "(CODE.QUOTE(a b (c d) e) 3 CODE.NTH)";
            Program.ExecPush(prog);
            Assert.AreEqual("(c d)", TestUtils.GetTopCodeString());
        }

        [TestMethod]    
        public void NthEmptyTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE () 0 CODE.NTH)";
            Program.ExecPush(prog);
            Assert.AreEqual("()", TestUtils.GetTopCodeString());
        }


        [TestMethod]
        public void NthValueTest()
        {
            var prog = "(CODE.QUOTE a 10 CODE.NTH)";
            Program.ExecPush(prog);
            Assert.AreEqual("(a)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void NthValueValueItselfResultTest()
        {
            var prog = "(CODE.QUOTE a 1 CODE.NTH)";
            Program.ExecPush(prog);
            Assert.AreEqual("a", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void NoIntegerArgumentNthTest()
        {
            var prog = "(CODE.QUOTE (a b c) CODE.NTH)";
            Program.ExecPush(prog);
            Assert.AreEqual(2, TestUtils.LengthOf("CODE"));
        }

    }
}

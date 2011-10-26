using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using Push = push.types.Ast.Push;
using push.core;

namespace InterpreterTests
{
    [TestClass]
    public class MemberTest
    {

        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }
 
        [TestMethod]
        [Description ("Tests Container operation: most basic test")]
        public void BasicMemberTest()
        {
            var prog = "(CODE.QUOTE(a b (c d) e)CODE.QUOTE(c d)CODE.MEMBER)";
            Program.ExecPush(prog);
            Assert.IsTrue(TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void MemberEmptyTest()
        {
            var prog = "(CODE.QUOTE () CODE.QUOTE () CODE.MEMBER)";
            Program.ExecPush(prog);
            Assert.IsTrue(TestUtils.Top<bool>("BOOLEAN"));
        }


        [TestMethod]
        public void EmptyMemberEmptyTest()
        {
            var prog = "(CODE.QUOTE (a b c) CODE.QUOTE () CODE.MEMBER)";
            Program.ExecPush(prog);
            Assert.IsTrue(TestUtils.Top<bool>("BOOLEAN"));
        }


        [TestMethod]
        public void BothItemsEqualListsTest()
        {
            var prog = "(CODE.QUOTE (a b c) CODE.QUOTE (a b c) CODE.MEMBER)";
            Program.ExecPush(prog);
            Assert.IsFalse(TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void BothItemsEqualValuesTest()
        {
            var prog = "(CODE.QUOTE b CODE.QUOTE b CODE.MEMBER)";
            Program.ExecPush(prog);
            Assert.IsFalse(TestUtils.Top<bool>("BOOLEAN"));
        }

        [TestMethod]
        public void MemberWithinMemberTest()
        {
            var prog = "(CODE.QUOTE (a b (c d (e f))) CODE.QUOTE (e f) CODE.MEMBER)";
            Program.ExecPush(prog);
            Assert.IsFalse(TestUtils.Top<bool>("BOOLEAN"));
        }


    }
}

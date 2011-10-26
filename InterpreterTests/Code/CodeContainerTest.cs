using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using push.types.stock;
using Push = push.types.Ast.Push;

namespace InterpreterTests
{
    [TestClass]
    public class ContainerTest
    {

        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }
 
        [TestMethod]
        [Description ("Tests Container operation: most basic test")]
        public void BasicContainerTest()
        {
            Push lst2 = TestUtils.RunParser("(a b (c d) e)");
            Push lst1 = TestUtils.RunParser("(c d)");
            
            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");
           
            Assert.AreEqual(4, res.toList.Length);
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
            Assert.AreEqual("a", ((Push.Value)res.toList[0]).Item.Raw<string>());
        }

        [TestMethod]
        [Description("Tests Container operation: empty second item")]
        public void EmptySecondItemTest()
        {
            Push lst2 = TestUtils.RunParser("(a b (c d) e)");
            Push lst1 = TestUtils.RunParser("()");

            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");

            Assert.AreEqual(4, res.toList.Length);
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
            Assert.AreEqual("a", ((Push.Value)res.toList[0]).Item.Raw<string>());
        }

        [TestMethod]
        [Description("Tests Container operation: value type second item")]
        public void ValueSecondItemTest()
        {
            Push lst2 = TestUtils.RunParser("(a b (c d) e)");
            Push lst1 = TestUtils.RunParser("c");

            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");

            Assert.AreEqual(2, res.toList.Length);
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
            Assert.AreEqual("c", ((Push.Value)res.toList[0]).Item.Raw<string>());
        }


        [TestMethod]
        [Description("Tests Container operation: value type second item")]
        public void FirstItemEmptyTest()
        {
            Push lst2 = TestUtils.RunParser("()");
            Push lst1 = TestUtils.RunParser("c");

            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");

            Assert.AreEqual(0, res.toList.Length);
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
        }

        [TestMethod]
        [Description("Tests Container operation: both items empty lists")]
        public void BothItemsEmptyTest()
        {
            Push lst2 = TestUtils.RunParser("()");
            Push lst1 = TestUtils.RunParser("()");

            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");

            Assert.AreEqual(0, res.toList.Length);
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
        }

        [TestMethod]
        [Description("Tests Container operation: both items equal lists")]
        public void BothItemsEqualListsTest()
        {
            Push lst2 = TestUtils.RunParser("(a b c d)");
            Push lst1 = TestUtils.RunParser("(a b c d)");

            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");

            Assert.AreEqual(0, res.toList.Length);
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
        }

        [TestMethod]
        [Description("Tests Container operation: first item value")]
        public void FirstItemValueTest()
        {
            Push lst2 = TestUtils.RunParser("b");
            Push lst1 = TestUtils.RunParser("b");

            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");

            Assert.AreEqual("b", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        [Description("Tests Container operation: second item occurs multple times in sublists")]
        public void SecondItemMultipleSublistsTest()
        {
            Push lst2 = TestUtils.RunParser("(a (b c) d (e f (g h (b c) k) l) p n)");
            Push lst1 = TestUtils.RunParser("(b c)");

            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");

            Assert.AreEqual(4, res.toList.Length);
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
            Assert.AreEqual("g", ((Push.Value)res.toList[0]).Item.Raw<string>());
        }

        [TestMethod]
        [Description("Tests Container operation: second item contains sublists")]
        public void ComplexSecondItemTest()
        {
            Push lst2 = TestUtils.RunParser("(a (b c) d (e f (g h (b c (k (l)))) p ) n)");
            Push lst1 = TestUtils.RunParser("(b c (k (l)))");

            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");

            Assert.AreEqual(3, res.toList.Length);
            Assert.AreEqual(1, TestUtils.LengthOf("CODE"));
            Assert.AreEqual("g", ((Push.Value)res.toList[0]).Item.Raw<string>());
        }

        [TestMethod]
        [Description("Tests Container operation: no match")]
        public void NoMatchContainerTest()
        {
            Push lst2 = TestUtils.RunParser("(a b c d e)");
            Push lst1 = TestUtils.RunParser("(c d)");

            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");

            Assert.AreEqual(0, res.toList.Length);
        }

    }
}

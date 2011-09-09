using System.Linq;
using System.Reflection;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using push.types.stock;
using ExtensionAssembly;
using BadClass;
using NonstaticOp;
using Type = push.types.Type;
using push.parser;
using Push = push.parser.Ast.Push;
using Code = push.types.stock.StockTypesCode.Code;

namespace InterpreterTests
{
    [TestClass]
    public class CodeTest
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
            Push lst1 = TestUtils.RunParser("(a b (c d) e)");
            Push lst2 = TestUtils.RunParser("(c d)");
            
            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");
           
            Assert.AreEqual(4, res.asPushList.Length);
            Assert.AreEqual("a", ((Push.Value)res.asPushList[0]).Item.Raw<string>());
        }

        [TestMethod]
        [Description("Tests Container operation: no match")]
        public void NoMatchContainerTest()
        {
            Push lst1 = TestUtils.RunParser("(a b c d e)");
            Push lst2 = TestUtils.RunParser("(c d)");

            TestUtils.PushVal<Code>(lst2);
            TestUtils.PushVal<Code>(lst1);

            Code.Container();
            var res = TestUtils.Top<Push>("CODE");

            Assert.AreEqual(0, res.asPushList.Length);
        }

    }
}

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
using push.core;
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
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;

namespace InterpreterTests
{
    [TestClass]
    public class E2ETests
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        [Description("Compute factorial the hard way. Depends on code being pushed to code stack first")]
        public void FactorialComplexTest()
        {
            var prog = "5";
            Program.ExecPush(prog);

            prog = @"( CODE.QUOTE ( INTEGER.POP 1 )  
                            CODE.QUOTE ( CODE.DUP INTEGER.DUP 1 INTEGER.- CODE.DO INTEGER.* )  
                        INTEGER.DUP 2 INTEGER.< CODE.IF )";

            Program.ExecPush(prog);
            Assert.AreEqual(120, TestUtils.Top<long>("INTEGER"));
        }
    }
}

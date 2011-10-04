using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;
using push.types.stock;

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

        [TestMethod]
        [Description("Compute Fibonacci sequence")]
        public void FibonacciSequence1Test()
        {
            var prog = "8";
            Program.ExecPush(prog);

            prog = @"(1 INTEGER.+ EXEC.DO*TIMES EXEC.S INTEGER./ INTEGER.STACKDEPTH ())";

            Program.ExecPush(prog);
            Assert.AreEqual(21, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        [Description("Compute Fibonacci sequence")]
        public void FibonacciSequence2Test()
        {
            TypeFactory.pushResult(new StockTypesInteger.Integer(8L));

            var prog = @"(EXEC.DO*TIMES (CODE.LENGTH EXEC.S)
                            INTEGER.STACKDEPTH CODE.YANKDUP)";

            Program.ExecPush(prog);
            Assert.AreEqual(21, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        [Description("Compute Fibonacci sequence")]
        public void FibonacciSequence3Test()
        {
            TypeFactory.pushResult(new StockTypesInteger.Integer(8L));

            var prog = @"(EXEC.DO*COUNT EXEC.S CODE.QUOTE NAME.=
                            CODE.DO*COUNT CODE.YANKDUP CODE.DO*COUNT
                            CODE.CONS CODE.STACKDEPTH)";

            Program.ExecPush(prog);
            Assert.AreEqual(21, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        [Description("Reverse list")]
        public void ReverseListTest()
        {
            var prog = @"(10 20 30 40 50 60 70 80)";
            Program.ExecPush(prog);

            prog = @"(CODE.DO* INTEGER.STACKDEPTH EXEC.DO*TIMES
                            CODE.FROMINTEGER CODE.STACKDEPTH EXEC.DO*TIMES
                            CODE.CONS)";

            Program.ExecPush(prog);
        }
    }
}

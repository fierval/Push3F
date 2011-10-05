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
            var prog = @"(CODE.QUOTE (10 20 30 40 50 60 70 80))";
            Program.ExecPushProgram(prog, Program.ExecutionFlags.None);

            prog = @"(CODE.DO* INTEGER.STACKDEPTH EXEC.DO*TIMES
                            CODE.FROMINTEGER CODE.STACKDEPTH EXEC.DO*TIMES
                            CODE.CONS)";

            Program.ExecPushProgram(prog, Program.ExecutionFlags.None);
            Assert.AreEqual("(80 70 60 50 40 30 20 10)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        [Description("Reverse list")]
        public void ReverseList2Test()
        {
            var prog = @"(CODE.QUOTE (10 20 30 40 50 60 70 80))";
            Program.ExecPushProgram(prog, Program.ExecutionFlags.None);

            prog = @"(CODE.DO*TIMES (CODE.DO* CODE.LIST
                        (((INTEGER.STACKDEPTH EXEC.DO*TIMES)
                        (BOOLEAN.YANKDUP CODE.FROMINTEGER))
                        CODE.FROMINTEGER INTEGER.SWAP)
                        (CODE.YANKDUP INTEGER.% (BOOLEAN.AND)
                        CODE.STACKDEPTH EXEC.DO*TIMES)) (CODE.CONS)
                        (BOOLEAN.SHOVE (CODE.EXTRACT EXEC.S
                        (EXEC.FLUSH CODE.IF BOOLEAN.YANK
                        (CODE.FROMINTEGER CODE.ATOM (CODE.SWAP
                        BOOLEAN.SHOVE (INTEGER.MAX) (CODE.QUOTE
                        CODE.APPEND CODE.IF)) ((CODE.ATOM CODE.SHOVE
                        EXEC.POP (CODE.DO*TIMES BOOLEAN.SHOVE) INTEGER.ROT)
                        (INTEGER.> BOOLEAN.AND CODE.DO* INTEGER.ROT)
                        CODE.CONS INTEGER.ROT ((CODE.NTHCDR) INTEGER.ROT
                        BOOLEAN.DUP) INTEGER.SHOVE (CODE.FROMNAME
                        (CODE.CONS CODE.FROMINTEGER)))) CODE.LENGTH
                        INTEGER.MAX EXEC.Y)) (BOOLEAN.= (CODE.QUOTE
                        INTEGER.SWAP) CODE.POP) INTEGER.FLUSH))";

            Program.ExecPushProgram(prog, Program.ExecutionFlags.None);
            Assert.AreEqual("(80 70 60 50 40 30 20 10)", TestUtils.GetTopCodeString());
        }

         [TestMethod]
        [Description("Reverse list")]
        public void ParityTest()
        {
            var prog = @"(TRUE TRUE FALSE FALSE TRUE TRUE FALSE TRUE FALSE TRUE FALSE FALSE)";
            Program.ExecPushProgram(prog, Program.ExecutionFlags.None);

            prog = @"((BOOLEAN.STACKDEPTH)
                        (EXEC.DO*TIMES) (BOOLEAN.= BOOLEAN.NOT)
                        )";

            Program.ExecPushProgram(prog, Program.ExecutionFlags.None);
            Assert.IsTrue(TestUtils.Top<bool>("BOOLEAN"));

            prog = @"(BOOLEAN.POP (TRUE TRUE FALSE FALSE TRUE TRUE FALSE TRUE FALSE FALSE) (BOOLEAN.STACKDEPTH)
                        (EXEC.DO*TIMES) (BOOLEAN.= BOOLEAN.NOT))";

            Program.ExecPushProgram(prog, Program.ExecutionFlags.None);
            Assert.IsFalse(TestUtils.Top<bool>("BOOLEAN"));

        }

   }
}

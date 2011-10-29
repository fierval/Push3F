using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using Push = push.types.Ast.Push;
using push.core;
using System.Threading;
using push.parser;

namespace InterpreterTests
{
    [TestClass]
    public class RandTest
    {

        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }
 
        [TestMethod]
        [Description ("Tests Container operation: most basic test")]
        public void SimpleRandTest()
        {
            var prog = "CODE.RAND";
            for (int i = 0; i < 1000; i++)
            {
                Program.ExecPushProgram(prog, Program.ExecutionFlags.None);
            }

            Eval.maxSteps = 1000; // don't linger

            prog = "(CODE.STACKDEPTH CODE.DO*COUNT)";
            Program.ExecPushProgram(prog, Program.ExecutionFlags.None);
        }
    }
}

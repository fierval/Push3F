using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using Push = push.types.Ast.Push;
using push.core;
using System.Threading;

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
            var prog = "(CODE.POP CODE.RAND)";
            for (int i = 0; i < 1000; i++)
            {
                Program.ExecPush(prog);
            }
        }
    }
}

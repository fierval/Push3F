using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.core;
using push.types;
using System.IO;

namespace InterpreterTests
{
    [TestClass]
    public class ExecOpenSaveTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        [Description("Computes first n squares, starting from 0")]
        public void SaveOpenSimpleTest()
        {
            var prog = "(\"mycode.push\" EXEC.SAVE (5 EXEC.DO*COUNT (INTEGER.DUP INTEGER.*)))";
            Program.ExecPush(prog);

            Assert.IsTrue(File.Exists(@"mycode.push"));

            prog = "(\"mycode.push\" EXEC.OPEN)";
            Program.ExecPush(prog);

            Assert.AreEqual(16, TestUtils.Top<long>("INTEGER"));
            Assert.AreEqual(0, TestUtils.Elem<long>("INTEGER", 4));
        }

        [TestMethod]
        public void IOErrorsRobustnessTest()
        {
            var prog = "(LITERAL.RAND EXEC.OPEN)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));

            prog = "(EXEC.SAVE EXEC.OPEN)";
            Program.ExecPush(prog);

            Assert.IsTrue(TestUtils.IsEmpty("EXEC"));

            // save twice to the same file
            prog = @"(file LITERAL.RAND 
                        LITERAL.DEFINE 
                        file EXEC.SAVE(5 EXEC.DO*COUNT (INTEGER.DUP INTEGER.*)) 
                        file EXEC.SAVE(100)
                        file EXEC.OPEN)";

            Program.ExecPush(prog);
            Assert.AreEqual(100, TestUtils.Top<long>("INTEGER"));
        }
    }
}

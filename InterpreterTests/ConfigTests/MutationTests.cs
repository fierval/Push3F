using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Core;
using push.core;
using push.types;
using push.types.stock;
using push.genetics;
using push.parser;

namespace GeneticTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MutationTests
    {
        public MutationTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
            Eval.maxSteps = 1000; // don't linger
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void PickRandomProgram()
        {
            Enumerable.Range(0, 99).ToList().ForEach(i => Assert.AreNotEqual(i, Mutations.randomPick(100, i)));
        }

        [TestMethod]
        public void RemoveRandomPieceTest()
        {
            var prog = Code.rand(300, FSharpOption<string>.None);
            int len = Mutations.length(prog);
            prog = Mutations.removeRandomPiece(prog);
            Assert.IsTrue(len >= Mutations.length(prog));
            Program.execProgram(prog, false);
        }

        [TestMethod]
        public void TrimExtraCodeTest()
        {
            int len = 0;
            var prog = Code.rand(300, FSharpOption<string>.None);
            len = Mutations.length(prog);
            len = len - 5;

            prog = Mutations.trimExtraCodePoints(len, prog);
            Assert.IsTrue(len >= Mutations.length(prog));
            Program.execProgram(prog, false);
        }

        [TestMethod]
        public void InsertRandomCodeTest()
        {
            var prog = Code.rand(30, FSharpOption<string>.None);
            int len = Mutations.length(prog);
            prog = Mutations.insertRandomPiece(prog, 30);
            Assert.IsTrue(len <= 30);
            Program.execProgram(prog, false);
        }

        [TestMethod]
        public void XoverSubtreeTest()
        {
            var mom = Code.rand(20, FSharpOption<string>.None);
            var dad = Code.rand(20, FSharpOption<string>.None);
            var progTuple = Mutations.xoverSubtree(mom, dad, 20);

            Program.execProgram(progTuple.Item1, false);
            Program.execProgram(progTuple.Item2, false);
        }

    }
}

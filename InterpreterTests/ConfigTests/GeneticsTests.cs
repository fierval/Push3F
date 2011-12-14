using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using push.types;
using push.types.stock;
using push.core;
using push.genetics;
using Push;
using push.parser;
using InterpreterTests;

namespace GeneticTests
{
    /// <summary>
    /// Summary description for GeneticsTests
    /// </summary>
    [TestClass]
    public class GeneticsTests
    {
        GenConfig.GenConfig config = null;
        FSharpList<Ast.Push> population = null;
        Genetics genetics = null;

        public GeneticsTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        [TestInitialize()]
        public void TestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
            Eval.maxSteps = 1000; // don't linger

            this.config = Push.Genetic.readConfig("sampleConfig.xml");
            Code.Me.MaxCodePoints = config.maxCodePoints;
            this.population = Microsoft.FSharp.Collections.ListModule.Initialize<Ast.Push>(config.populSize, FSharpFunc<int, Ast.Push>.FromConverter(i => Code.rand(config.maxCodePoints)));
            this.genetics = new Genetics(this.config, this.population);
        }

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
        [DeploymentItem("sampleConfig.xml")]
        public void InitializationTest()
        {

        }

        [TestMethod]
        [DeploymentItem("sampleConfig.xml")]
        public void RunMemberEvalFitnessTest()
        {
            var prog = Parser.parseGetCode(@"( CODE.QUOTE ( INTEGER.POP 1 )  
                            CODE.QUOTE ( CODE.DUP INTEGER.DUP 1 INTEGER.- CODE.DO INTEGER.* )  
                        INTEGER.DUP 2 INTEGER.< CODE.IF )");

            this.genetics.runMemberAndEvalFitness(prog, this.config.fitnessValues[3]);

            Assert.AreEqual(0D, TestUtils.Top<double>("FLOAT"));
        }
    }
}

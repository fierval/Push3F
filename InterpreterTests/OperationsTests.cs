using System.Linq;
using System.Reflection;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using push.types.stock;
using Type = push.types.Type;
using push.core;

namespace InterpreterTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class OperationsTest
    {
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
        [TestInitialize()]
        public void OpsTestInitialize() 
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void FlushTest()
        {
            TypeFactory.pushResult(new Integer(32L));
            TypeFactory.pushResult(new Integer(64L));

            TypeFactory.exec("INTEGER", "FLUSH");
            var res = TypeFactory.stockTypes.Stacks["INTEGER"];

            Assert.AreEqual(0, res.length);
        }

        [TestMethod]
        public void DefineTest()
        {
            TypeFactory.pushResult(new Float(345.67));
            TypeFactory.pushResult(new Name("SomeFloat"));

            TypeFactory.exec("FLOAT", "DEFINE");
            Assert.AreEqual(345.67, TypeFactory.stockTypes.Bindings["SomeFloat"].Raw<double>());
        }

        [TestMethod]
        public void DupTest()
        {
            TypeFactory.pushResult(new Name("SomeId"));

            TypeFactory.exec("NAME", "DUP");
            Assert.AreEqual(2, TypeFactory.stockTypes.Stacks["NAME"].length);
            Assert.AreEqual("SomeId", TestUtils.Top<string>("NAME"));
            Assert.AreEqual("SomeId", TestUtils.ListOf("NAME")[1].Raw<string>());

        }

        [TestMethod]
        public void PopTest()
        {
            TypeFactory.pushResult(new Name("SomeId"));

            TypeFactory.exec("NAME", "POP");
            Assert.IsTrue(TestUtils.IsEmpty("NAME"));

        }


        [TestMethod]
        public void RotTest()
        {
            TestUtils.PushVal<Integer>(35L);
            TestUtils.PushVal<Integer>(34L);
            TestUtils.PushVal<Integer>(33L);

            TypeFactory.exec("INTEGER", "ROT");

            Assert.AreEqual(3, TestUtils.StackOf("INTEGER").length);
            Assert.AreEqual(35, TestUtils.StackOf("INTEGER").asList.Head.Raw<long>());
        }

        [TestMethod]
        public void ShoveTest()
        {
            string prog = "(35 34 33 32 31 3 INTEGER.SHOVE)";

            Program.ExecPush(prog);

            Assert.AreEqual(5, TestUtils.StackOf("INTEGER").length);
            Assert.AreEqual(31L, TestUtils.Elem<long>("INTEGER", 3));
        }

        [TestMethod]
        public void SwapTest()
        {
            TestUtils.PushVal<Integer>(35L);
            TestUtils.PushVal<Integer>(34L);

            TypeFactory.exec("INTEGER", "SWAP");
            Assert.AreEqual(35, TestUtils.Top<long>("INTEGER"));
        }

        [TestMethod]
        public void StackDepth()
        {
            TestUtils.PushVal<Integer>(35L);
            TestUtils.PushVal<Integer>(34L);
            TestUtils.PushVal<Integer>(33L);
            TestUtils.PushVal<Integer>(32L);
            TestUtils.PushVal<Integer>(31L);
            TestUtils.PushVal<Integer>(3L);

            TypeFactory.exec("INTEGER", "STACKDEPTH");

            Assert.AreEqual(7, TestUtils.LengthOf("INTEGER"));

            TestUtils.PushVal<Float>(35.0);
            TestUtils.PushVal<Float>(34.1);
            TestUtils.PushVal<Float>(33.2);
            TestUtils.PushVal<Float>(32.3);
            TestUtils.PushVal<Float>(31.5);
            TestUtils.PushVal<Float>(3.8);

            TypeFactory.exec("FLOAT", "STACKDEPTH");

            Assert.AreEqual(6, TestUtils.LengthOf("FLOAT"));

        }

        [TestMethod]
        public void YankOperationTest()
        {
            TestUtils.PushVal<Integer>(3L);

            TestUtils.PushVal<Float>(35.0);
            TestUtils.PushVal<Float>(34.1);
            TestUtils.PushVal<Float>(33.2);
            TestUtils.PushVal<Float>(32.3);
            TestUtils.PushVal<Float>(31.5);
            TestUtils.PushVal<Float>(3.8);

            TypeFactory.exec("FLOAT", "YANK");

            Assert.AreEqual(33.2, TestUtils.Top<double>("FLOAT"));
            Assert.AreEqual(6, TestUtils.LengthOf("FLOAT"));
            Assert.IsTrue(TestUtils.IsEmpty("INTEGER"));
        }

        [TestMethod]
        public void YankDupTest()
        {
            TestUtils.PushVal<Integer>(3L);

            TestUtils.PushVal<Float>(35.0);
            TestUtils.PushVal<Float>(34.1);
            TestUtils.PushVal<Float>(33.2);
            TestUtils.PushVal<Float>(32.3);
            TestUtils.PushVal<Float>(31.5);
            TestUtils.PushVal<Float>(3.8);

            TypeFactory.exec("FLOAT", "YANKDUP");

            Assert.AreEqual(33.2, TestUtils.Top<double>("FLOAT"));
            Assert.AreEqual(33.2, TestUtils.ListOf("FLOAT")[4].Raw<double>());
            Assert.AreEqual(7, TestUtils.LengthOf("FLOAT"));
            Assert.IsTrue(TestUtils.IsEmpty("INTEGER"));
        }

    }
}

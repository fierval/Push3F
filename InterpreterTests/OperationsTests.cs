using System.Linq;
using System.Reflection;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using push.types.stock;
using Type = push.types.Type;

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
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void FlushTest()
        {
            TypeFactory.stockTypes.cleanAllStacks();

            TypeFactory.pushResult(new StockTypesInteger.Integer(32L));
            TypeFactory.pushResult(new StockTypesInteger.Integer(64L));

            TypeFactory.exec("INTEGER", "FLUSH");
            var res = TypeFactory.stockTypes.Stacks["INTEGER"];

            Assert.AreEqual(0, res.length);
        }
    }
}

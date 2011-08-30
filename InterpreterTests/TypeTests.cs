using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Reflection;
using push.types;
using push.types.stock;
using Microsoft.FSharp;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

namespace InterpreterTests
{
    /// <summary>
    /// Summary description for TypeTests
    /// </summary>
    [TestClass]
    public class TypeTests
    {
        public TypeTests()
        {
            //
            // TODO: Add constructor logic here
            //
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
        //[TestCleanup()]
        //public void CleanupStacks()
        //{
        //}
        //
        #endregion

        [TestMethod]
        [Description("load push types using shared functions")]
        public void LoadPushTypesSharedTest()
        {
            var types = TypesShared.loadTypes(FSharpOption<string>.Some("Interpreter.dll"));

            var sysTypes = TypesShared.getAnnotatedTypes(typeof(push.types.TypeAttributes.PushTypeAttribute), types);

            Assert.AreEqual<int>(3, sysTypes.Count());
        }

        [TestMethod]
        [Description("create a push object")]
        public void CreatePushObjectTest()
        {
            var types = TypesShared.loadTypes(FSharpOption<string>.Some("Interpreter.dll"));

            var sysTypes = TypesShared.getAnnotatedTypes(typeof(push.types.TypeAttributes.PushTypeAttribute), types).ToList();

            var names = sysTypes.Select(t => (t.GetCustomAttributes(typeof(push.types.TypeAttributes.PushTypeAttribute), false).Single() as push.types.TypeAttributes.PushTypeAttribute).Name).ToList();

            var obj = TypeFactory.createPushObject<push.types.Type.PushTypeBase>(sysTypes[0]);

            Assert.IsTrue(names.Where(n => n == obj.Item2).Count() == 1);
        }

        [TestMethod]
        [Description("Extract operations from a type")]
        public void GetOperationsForTypeTest()
        {
            var integer = new StockTypesInteger.Integer(35L);
            var ops = StockTypesInteger.Integer.GetOperations(integer);
            Assert.IsTrue(ops.ContainsKey("+"));
            Assert.AreEqual<string>("Add", ops["+"].Name);
        }

        [TestMethod]
        [Description("stacks creation")]
        public void CreateStacksTest()
        {
            var ptypes = TypeFactory.stockTypes.ptypes;
            var stacks = TypeFactory.stockTypes.stacks;
            Assert.AreEqual<int>(3, stacks.Count);
        }

        [TestMethod]
        [Description("execute a simple operation")]
        public void PerformOperationTest()
        {
            TypeFactory.stockTypes.cleanAllStacks();

            TypeFactory.pushResult(new StockTypesInteger.Integer(32L));
            TypeFactory.pushResult(new StockTypesInteger.Integer(64L));

            StockTypesInteger.Integer.Subtract();
            var res = push.stack.Stack.peek(TypeFactory.stockTypes.stacks["Integer"]);
            Assert.AreEqual<long>(32L, (long)res.Value);
        }

        [TestMethod]
        [Description("execute a simple operation")]
        public void PerformOperationFloatTest()
        {
            TypeFactory.stockTypes.cleanAllStacks();
            var ops = StockTypesInteger.Integer.GetOperations(new StockTypesInteger.Integer());
            var opsFloat = StockTypesFloat.Float.GetOperations(new StockTypesFloat.Float());
            Assert.IsTrue(opsFloat.ContainsKey("*"));
            Assert.AreEqual<string>("Float", opsFloat["*"].DeclaringType.Name);
        }

        [TestMethod]
        [Description("execute a simple operation")]
        public void PerformOperationOneArgMissingTest()
        {
            TypeFactory.stockTypes.cleanAllStacks();

            TypeFactory.pushResult(new StockTypesInteger.Integer(32L));

            StockTypesInteger.Integer.Subtract();

            // make sure that whatever is on the stack still remains there after
            // the operation has been executed
            var res = push.stack.Stack.peek(TypeFactory.stockTypes.stacks["Integer"]);
            Assert.AreEqual<long>(32L, (long)res.Value);
        }

        [TestMethod]
        [Description("execute a simple operation")]
        public void PerformOperationArgsMissingTest()
        {
            TypeFactory.stockTypes.cleanAllStacks();

            StockTypesInteger.Integer.Subtract();

            // make sure that whatever is on the stack still remains there after
            // the operation has been executed
            var res = push.stack.Stack.peek(TypeFactory.stockTypes.stacks["Integer"]);
            Assert.IsTrue(res == default(push.types.Type.PushTypeBase));
        }

    }
}

﻿using System.Linq;
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
    /// Summary description for TypeTests
    /// </summary>
    [TestClass]
    public class TypeTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

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
            var types = TypesShared.loadTypes(FSharpOption<string>.Some("Push.Parser.dll"));

            var sysTypes = TypesShared.getAnnotatedTypes(typeof(push.types.TypeAttributes.PushTypeAttribute), types);

            Assert.AreEqual(5, sysTypes.Count());
        }

        [TestMethod]
        [Description("create a push object")]
        public void CreatePushObjectTest()
        {
            var types = TypesShared.loadTypes(FSharpOption<string>.Some("Push.Parser.dll"));

            var sysTypes = TypesShared.getAnnotatedTypes(typeof(push.types.TypeAttributes.PushTypeAttribute), types).ToList();

            var names = sysTypes.Select(t => (t.GetCustomAttributes(typeof(push.types.TypeAttributes.PushTypeAttribute), false).Single() as push.types.TypeAttributes.PushTypeAttribute).Name).ToList();

            var obj = TypeFactory.createPushObject<Type.PushTypeBase>(sysTypes[0]);

            Assert.IsTrue(names.Where(n => n == obj.Item2).Count() == 1);
        }

        [TestMethod]
        [Description("Extract operations from a type")]
        public void GetOperationsForTypeTest()
        {
            var integer = new StockTypesInteger.Integer(35L);
            FSharpMap<string, MethodInfo> ops = Type.PushTypeBase.GetOperations(integer);
            Assert.IsTrue(ops.ContainsKey("+"));
            Assert.AreEqual("Add", ops["+"].Name);
        }

        [TestMethod]
        [Description("stacks creation")]
        public void CreateStacksTest()
        {
            var stacks = TypeFactory.stockTypes.stacks;
            Assert.AreEqual(5, stacks.Count);
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
            Assert.AreEqual(32L, (long)res.Value);
        }

        [TestMethod]
        [Description("execute a simple operation")]
        public void PerformOperationFloatTest()
        {
            TypeFactory.stockTypes.cleanAllStacks();
            var ops = new StockTypesInteger.Integer().Operations;
            var opsFloat = new StockTypesFloat.Float().Operations;

            Assert.IsTrue(opsFloat.ContainsKey("*"));
            Assert.AreEqual("Float", opsFloat["*"].DeclaringType.Name);

            Assert.IsTrue(ops.ContainsKey("*"));
            Assert.AreEqual("Integer", ops["*"].DeclaringType.Name);

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
            Assert.AreEqual(32L, (long)res.Value);
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
            Assert.IsTrue(res == default(Type.PushTypeBase));
        }

    }
}

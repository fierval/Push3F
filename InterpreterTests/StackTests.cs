﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using push.stack;

namespace InterpreterTests
{
    /// <summary>
    /// Summary description for StackTests
    /// </summary>
    [TestClass]
    public class StackTests
    {
        public StackTests()
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
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [Description("Tests push/pop")]
        public void PushPopTest()
        {
            Stack.Stack<string> stack = Stack.empty<string>();

            stack = Stack.push("a", stack);
            stack = Stack.push("b", stack);
            
            var res = Stack.pop(stack);

            Assert.AreEqual<string>("b", res.Item1);

            res = Stack.pop(res.Item2);

            Assert.AreEqual<string>("a", res.Item1);

        }

        [TestMethod]
        [Description("Popping from an empty stack. Expect an empty stack exception")]
        [ExpectedException(typeof(push.exceptions.PushExceptions.EmptyStackException))]
        public void PopEmptyTest()
        {
            Stack.Stack<int> stack = Stack.empty<int>();
            Stack.pop(stack);
        }

        [TestMethod]
        [Description("Testing reverse function")]
        public void ReverseEmptyTest()
        {
            var stack = Stack.empty<int>();

            var rev = Stack.reverse(stack);

            Assert.AreEqual<int>(0, stack.length);
        }

        [TestMethod]
        [Description("Testing length function")]
        public void LengthTest()
        {
            var stack = Stack.empty<int>();
            stack = Stack.push(1, stack);
            stack = Stack.push(2, stack);
            stack = Stack.push(3, stack);

            Assert.AreEqual<int>(3, stack.length);

            stack = Stack.push(4, stack);

            Assert.AreEqual<int>(4, stack.length);

        }

        [TestMethod]
        [Description("Testing reverse function")]
        public void ReverseTest()
        {
            var stack = Stack.empty<int>();
            stack = Stack.push(1, stack);
            stack = Stack.push(2, stack);
            stack = Stack.push(3, stack);

            var rev = Stack.reverse(stack);

            Assert.AreEqual<int>(1, Stack.peek(rev));
            Assert.AreEqual<int>(3, rev.length);
        }
    }
}

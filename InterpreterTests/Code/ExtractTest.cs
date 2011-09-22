﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using push.core;
using push.parser;
using Push = push.parser.Ast.Push;

namespace InterpreterTests
{
    [TestClass]
    public class ExtractTest
    {
        [TestInitialize()]
        public void OpsTestInitialize()
        {
            TypeFactory.stockTypes.cleanAllStacks();
        }

        [TestMethod]
        public void ExtractSimpleTest()
        {
            var prog = "(1 CODE.QUOTE (b c d e f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("b", TestUtils.GetTopCodeString());
        }

         [TestMethod]
        public void ExtractEntireListTest()
        {
            var prog = "(0 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(b c (d e) f g)", TestUtils.GetTopCodeString());
        }

        [TestMethod]
        public void ExtractIndexByModuleTest()
        {
            var prog = "(9 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("(d e)", TestUtils.GetTopCodeString());

        }

        [TestMethod]
        public void ExtractFromEmptyListTest()
        {
            var prog = "(9 CODE.QUOTE () CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("()", TestUtils.GetTopCodeString());



        }

        [TestMethod]
        public void ExtractWithNegativeArgumentTest()
        {
            var prog = "(-10 CODE.QUOTE (b c (d e) f g) CODE.EXTRACT)";
            Program.ExecPush(prog);

            Assert.AreEqual("f", TestUtils.GetTopCodeString());
        }

    }
}
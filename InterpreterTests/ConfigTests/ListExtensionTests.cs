using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using push.types;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace GeneticTests
{
    [TestClass]
    public class ListExtensionTests
    {
        FSharpList<int> lst = FSharpList<int>.Empty;

        [TestInitialize()]
        public void TestInitialize()
        {
            lst = FSharpList<int>.Empty;
            lst = FSharpList<int>.Cons(5, lst);
            lst = FSharpList<int>.Cons(4, lst);
            lst = FSharpList<int>.Cons(3, lst);
            lst = FSharpList<int>.Cons(2, lst);
            lst = FSharpList<int>.Cons(1, lst);
        }

        [TestMethod]
        public void ListReplaceSimple()
        {
            lst = TypeExensions.replace(1, 100, lst);
            Assert.AreEqual(100, lst[1]);
        }

        [TestMethod]
        public void ListReplaceZero()
        {
            lst = TypeExensions.replace(0, 100, lst);
            Assert.AreEqual(100, lst[0]);
        }

        [TestMethod]
        public void ListReplaceLast()
        {
            lst = TypeExensions.replace(4, 100, lst);
            Assert.AreEqual(100, lst[4]);
        }

        [TestMethod]
        public void ListRemove()
        {
            lst = TypeExensions.remove(2, lst);
            Assert.AreEqual(lst.Length, 4);
        }

    }
}

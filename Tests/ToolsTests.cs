using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TravellingSalesman;

namespace Tests
{
    [TestClass]
    public class ToolsTests
    {
        private static String ArrayToString( int[] arr )
        {
            return "{" + String.Join( ", ", arr ) + "}";
        }

        [TestMethod]
        public void PartitionTest1()
        {
            int[] arr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            int p = arr.Partition( 4 );
            Assert.AreEqual( 4, p, "Array contents: " + ArrayToString( arr ) );
            Assert.AreEqual( 5, arr[p], "Array contents: " + ArrayToString( arr ) );
        }

        [TestMethod]
        public void PartitionTest2()
        {
            int[] arr = new int[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            int p = arr.Partition( 4 );
            Assert.AreEqual( 3, p, "Array contents: " + ArrayToString( arr ) );
            Assert.AreEqual( 4, arr[p], "Array contents: " + ArrayToString( arr ) );
        }

        [TestMethod]
        public void SelectionTest1()
        {
            int[] arr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            int i = arr.SelectStatisticIndex( 0 );
            Assert.AreEqual( 1, arr[i], "Array contents: " + ArrayToString( arr ) );

            arr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            i = arr.SelectStatisticIndex( 7 );
            Assert.AreEqual( 8, arr[i], "Array contents: " + ArrayToString( arr ) );
        }

        [TestMethod]
        public void SelectionTest2()
        {
            int[] arr = new int[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            int i = arr.SelectStatisticIndex( 0 );
            Assert.AreEqual( 1, arr[i], "Array contents: " + ArrayToString( arr ) );

            arr = new int[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            i = arr.SelectStatisticIndex( 7 );
            Assert.AreEqual( 8, arr[i], "Array contents: " + ArrayToString( arr ) );
        }
    }
}

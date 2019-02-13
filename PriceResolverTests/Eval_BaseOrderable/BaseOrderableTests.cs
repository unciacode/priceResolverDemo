using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PriceResolver.Models;
using PriceResolver.Models.Oderable;

namespace PriceResolverTests.Eval_BaseOrderable {
    [TestClass]
    public class BaseOrderable_Tests {

        const double DBL_PRECISION = 0.001;
        const int RAND_INT_LENGTH = 35;

        static List<int> randTestInts = new List<int>();
        static List<int> randTestInts_nonZero = new List<int>();
        static List<int> randTestInts_neg = new List<int>();
        static List<int> allRandIntRanges = new List<int>();

        static BaseOrderable testTarget;

        [ClassInitialize]
        public static void setup(TestContext context) {

            var rand = new Random();
            while (randTestInts.Count < RAND_INT_LENGTH)
                randTestInts.Add(rand.Next());

            while (randTestInts_nonZero.Count < RAND_INT_LENGTH)
                randTestInts_nonZero.Add(rand.Next() + 1);

            while (randTestInts_nonZero.Count < RAND_INT_LENGTH)
                randTestInts_neg.Add(rand.Next() * -1);

            allRandIntRanges.AddRange(randTestInts);
            allRandIntRanges.AddRange(randTestInts_nonZero);
            allRandIntRanges.AddRange(randTestInts_neg);
        }


        [TestInitialize]
        public void preflight() {
            testTarget = new BaseOrderable();

            testTarget.ID = "This is the Test Object?";

            testTarget.PriceBreakList = new List<PriceBreak>() {
                new PriceBreak{ qty=1,  unitPrice = 1.0},
                new PriceBreak{ qty=10,  unitPrice = 0.1},
                new PriceBreak{ qty=100,  unitPrice = 0.01},
                new PriceBreak{ qty=1000,  unitPrice = 0.001}
            };

            testTarget.QtyStock = 750;
        }

        [TestMethod]
        public void BreakList_GetUnitPrice_noRequest() {

            Assert.AreEqual(0D, testTarget.GetUnitPriceForQty(), DBL_PRECISION);
            Assert.AreEqual(0D, testTarget.GetUnitPriceForQty(-1), DBL_PRECISION);
            Assert.AreEqual(0D, testTarget.GetUnitPriceForQty(0), DBL_PRECISION);

            Assert.AreEqual(1.0D, testTarget.GetUnitPriceForQty(1), DBL_PRECISION);
            Assert.AreEqual(1.0D, testTarget.GetUnitPriceForQty(5), DBL_PRECISION);
            Assert.AreEqual(1.0D, testTarget.GetUnitPriceForQty(9), DBL_PRECISION);

            Assert.AreEqual(0.1D, testTarget.GetUnitPriceForQty(10), DBL_PRECISION);
            Assert.AreEqual(0.1D, testTarget.GetUnitPriceForQty(12), DBL_PRECISION);
            Assert.AreEqual(0.1D, testTarget.GetUnitPriceForQty(19), DBL_PRECISION);

            Assert.AreEqual(0.01D, testTarget.GetUnitPriceForQty(100), DBL_PRECISION);
            Assert.AreEqual(0.01D, testTarget.GetUnitPriceForQty(101), DBL_PRECISION);
            Assert.AreEqual(0.01D, testTarget.GetUnitPriceForQty(750), DBL_PRECISION);

            Assert.AreEqual(0.001D, testTarget.GetUnitPriceForQty(1000), DBL_PRECISION);
            Assert.AreEqual(0.001D, testTarget.GetUnitPriceForQty(50000), DBL_PRECISION);
        }

        [TestMethod]
        public void BreakList_GetUnitPrice_Request() {
            testTarget.QtyRequested = 15;

            Assert.AreEqual(0.1D, testTarget.GetUnitPriceForQty(), DBL_PRECISION);

            Assert.AreEqual(0D, testTarget.GetUnitPriceForQty(-1), DBL_PRECISION);
            Assert.AreEqual(0D, testTarget.GetUnitPriceForQty(0), DBL_PRECISION);
        }


        [TestMethod]
        public void BreakList_GetRequestedUnitPrice() {
            Assert.AreEqual(0D, testTarget.GetCurrentReqestedUnitPrice(), DBL_PRECISION);
        }

        [TestMethod]
        public void BreakList_GetRequestedUnitPrice_Request() {
            testTarget.QtyRequested = 15;
            Assert.AreEqual(0.1D, testTarget.GetCurrentReqestedUnitPrice(), DBL_PRECISION);
        }


        [TestMethod]
        public void BreakList_GetMaxOrderableQty_NoMin_NoInt() {

            foreach (var i in allRandIntRanges) {
                if(i<=0)
                    Assert.AreEqual(0L, testTarget.GetMaxOrderableQty(i));
                if (i <= testTarget.QtyStock)
                    Assert.AreEqual(i, testTarget.GetMaxOrderableQty(i));
                else if(i > testTarget.QtyStock)
                    Assert.AreEqual(testTarget.QtyStock, testTarget.GetMaxOrderableQty(i));

            }
        }
        [TestMethod]
        public void BreakList_GetMaxOrderableQty_Min_NoInt() {
            testTarget.QtyMinimum = 5;
            foreach (var i in allRandIntRanges) {
                if (i <= 0)
                    Assert.AreEqual(0L, testTarget.GetMaxOrderableQty(i));
                else if (i <= testTarget.QtyMinimum)
                    Assert.AreEqual(0L, testTarget.GetMaxOrderableQty(i));
                if (i <= testTarget.QtyStock)
                    Assert.AreEqual(i, testTarget.GetMaxOrderableQty(i));
                else if (i > testTarget.QtyStock)
                    Assert.AreEqual(testTarget.QtyStock, testTarget.GetMaxOrderableQty(i));

            }
        }

        [TestMethod]
        public void BreakList_GetMaxOrderableQty_Min_Int() {
            testTarget.QtyMinimum = 5;
            testTarget.QtyInterval = 5;

            Assert.AreEqual(0L, testTarget.GetMaxOrderableQty(-1));
            Assert.AreEqual(0L, testTarget.GetMaxOrderableQty(0));
            Assert.AreEqual(0L, testTarget.GetMaxOrderableQty(3));

            Assert.AreEqual(5L, testTarget.GetMaxOrderableQty(5));
            Assert.AreEqual(5L, testTarget.GetMaxOrderableQty(6));

            Assert.AreEqual(10L, testTarget.GetMaxOrderableQty(10));

            Assert.AreEqual(250L, testTarget.GetMaxOrderableQty(250));
            Assert.AreEqual(250L, testTarget.GetMaxOrderableQty(253));

            Assert.AreEqual(750L, testTarget.GetMaxOrderableQty(750));
            Assert.AreEqual(750L, testTarget.GetMaxOrderableQty(751));
            Assert.AreEqual(750L, testTarget.GetMaxOrderableQty(850));
        }

        [TestMethod]
        public void BreakList_GetPriceBreak() {

            var tmpPriceBreak = testTarget.GetNearestBreakForQty(1);
            Assert.AreEqual(1L, tmpPriceBreak.qty);
            Assert.AreEqual(1.0D, tmpPriceBreak.unitPrice, DBL_PRECISION);

            tmpPriceBreak = testTarget.GetNearestBreakForQty(2);
            Assert.AreEqual(1L, tmpPriceBreak.qty);
            Assert.AreEqual(1.0D, tmpPriceBreak.unitPrice, DBL_PRECISION);

            tmpPriceBreak = testTarget.GetNearestBreakForQty(9);
            Assert.AreEqual(1L, tmpPriceBreak.qty);
            Assert.AreEqual(1.0D, tmpPriceBreak.unitPrice, DBL_PRECISION);

            tmpPriceBreak = testTarget.GetNearestBreakForQty(10);
            Assert.AreEqual(10L, tmpPriceBreak.qty);
            Assert.AreEqual(0.1D, tmpPriceBreak.unitPrice, DBL_PRECISION);

            tmpPriceBreak = testTarget.GetNearestBreakForQty(11);
            Assert.AreEqual(10L, tmpPriceBreak.qty);
            Assert.AreEqual(0.1D, tmpPriceBreak.unitPrice, DBL_PRECISION);

            tmpPriceBreak = testTarget.GetNearestBreakForQty(100);
            Assert.AreEqual(100L, tmpPriceBreak.qty);
            Assert.AreEqual(0.01D, tmpPriceBreak.unitPrice, DBL_PRECISION);

            tmpPriceBreak = testTarget.GetNearestBreakForQty(1000);
            Assert.AreEqual(1000L, tmpPriceBreak.qty);
            Assert.AreEqual(0.001D, tmpPriceBreak.unitPrice, DBL_PRECISION);

            tmpPriceBreak = testTarget.GetNearestBreakForQty(1010);
            Assert.AreEqual(1000L, tmpPriceBreak.qty);
            Assert.AreEqual(0.001D, tmpPriceBreak.unitPrice, DBL_PRECISION);
        }

        [TestMethod]
        public void BreakList_GetMinAmount() { //should always be 0 since there's 0 stock
            testTarget.QtyInterval = 5;

            Assert.AreEqual(0L, testTarget.GetMinimumAmountToFulfillInterval(5));
            Assert.AreEqual(4L, testTarget.GetMinimumAmountToFulfillInterval(1));
            Assert.AreEqual(3L, testTarget.GetMinimumAmountToFulfillInterval(7));
        }

    }
}
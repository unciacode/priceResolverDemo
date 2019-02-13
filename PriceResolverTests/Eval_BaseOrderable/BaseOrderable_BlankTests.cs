using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PriceResolver.Models.Oderable;

namespace PriceResolverTests.Eval_BaseOrderable {
    [TestClass]
    public class BaseOrderable_BlankTests {

        const double DBL_PRECISION = 0.001;
        const int RAND_INT_LENGTH = 25;

        static List<int> randTestInts = new List<int>();
        static List<int> randTestInts_nonZero = new List<int>();
        static List<int> randTestInts_neg = new List<int>();
        static List<int> allRandIntRanges = new List<int>();

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

        }

        [TestMethod]
        public void AutoIDGeneration() {
            string testIdName = "testIdName";

            var tmp = new BaseOrderable();
            Assert.AreNotEqual(String.Empty, tmp.ID);

            tmp.ID = null;
            Assert.AreNotEqual(String.Empty, tmp.ID);

            tmp.ID = null;
            Assert.IsTrue(Guid.TryParse(tmp.ID, out Guid throwAway));


            tmp.ID = testIdName;
            Assert.AreEqual(testIdName, tmp.ID);

        }

        [TestMethod]
        public void UnitializedBreakList_GetUnitPrice() {
            var tmp = new BaseOrderable();

            Assert.AreEqual(0D, tmp.GetUnitPriceForQty(), DBL_PRECISION);
            foreach (int i in allRandIntRanges)
                Assert.AreEqual(0D, tmp.GetUnitPriceForQty(i), DBL_PRECISION);

        }
        [TestMethod]
        public void UnitializedBreakList_GetRequestedUnitPrice() {
            var tmp = new BaseOrderable();
            Assert.AreEqual(0D, tmp.GetCurrentReqestedUnitPrice(), DBL_PRECISION);
        }

        [TestMethod]
        public void UninitializedBreakList_GetMaxOrderableQty() { //should always be 0 since there's 0 stock
            var tmp = new BaseOrderable();

            Assert.AreEqual(0L, tmp.GetMaxOrderableQty());
            foreach (int i in allRandIntRanges)
                Assert.AreEqual(0L, tmp.GetMaxOrderableQty(i));
        }

        [TestMethod]
        public void UnitializedBreakList_GetPriceBreak() {
            var tmp = new BaseOrderable();

            foreach (int i in allRandIntRanges) {
                var tmpPriceBreak = tmp.GetNearestBreakForQty(i);

                Assert.AreEqual(0L, tmpPriceBreak.qty);
                Assert.AreEqual(0D, tmpPriceBreak.unitPrice, DBL_PRECISION);
            }
        }

        [TestMethod]
        public void UninitializedBreakList_GetMinAmount() { //should always be 0 since there's 0 stock
            var tmp = new BaseOrderable();

            foreach (int i in allRandIntRanges)
                Assert.AreEqual(0L, tmp.GetMinimumAmountToFulfillInterval(i));
        }

    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WSUniversalLib;

namespace WSUniversalLibTest
{
    [TestClass]
    public class TestingModul
    {
        [TestMethod]
        public void GetQuantityForProduct_NonExistentProductType() 
        {
            int expected = -1;
            int actual = MaterialsCount.GetQuantityForProduct(15, 10, 20, 5, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_NonExistentMaterialType()
        {
            int expected = -1;
            int actual = MaterialsCount.GetQuantityForProduct(15, 10, 20, 2, 6);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_ZeroWidthProduct()
        {
            int expected = -1;
            int actual = MaterialsCount.GetQuantityForProduct(15, 0, 20, 2, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_ZeroLenghtProduct()
        {
            int expected = -1;
            int actual = MaterialsCount.GetQuantityForProduct(15, 10, 0, 2, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_ZeroProductCount()
        {
            int expected = -1;
            int actual = MaterialsCount.GetQuantityForProduct(0, 10, 20, 2, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_NedativeProductCount()
        {
            int expected = -1;
            int actual = MaterialsCount.GetQuantityForProduct(-8, 10, 20, 2, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_ZeroMaterialType()
        {
            int expected = -1;
            int actual = MaterialsCount.GetQuantityForProduct(15, 10, 20, 2, 0);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_ZeroProductType()
        {
            int expected = -1;
            int actual = MaterialsCount.GetQuantityForProduct(15, 10, 20, 0, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_AllZero()
        {
            int expected = -1;
            int actual = MaterialsCount.GetQuantityForProduct(0, 0, 0, 0, 0);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_ProductType1_MaterialType1()
        {
            int expected = 331;
            int actual = MaterialsCount.GetQuantityForProduct(10, 5, 6, 1, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_ProductType2_MaterialType2()
        {
            int expected = 151;
            int actual = MaterialsCount.GetQuantityForProduct(3, 4, 5, 2, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_ProductType3_MaterialType1_LargeValues()
        {
            int expected = 1691074;
            int actual = MaterialsCount.GetQuantityForProduct(100, 50, 40, 3, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_MinimalValidInputs()
        {
            int expected = 2;
            int actual = MaterialsCount.GetQuantityForProduct(1, 1, 1, 1, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_ProductType3_MaterialType2_FractionalResult()
        {
            int expected = 51;
            int actual = MaterialsCount.GetQuantityForProduct(1, 3, 2, 3, 2);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetQuantityForProduct_ProductType3_MaterialType1_MediumValues()
        {
            int expected = 114148; 
            int actual = MaterialsCount.GetQuantityForProduct(15, 20, 45, 3, 1);
            Assert.AreEqual(expected, actual);
        }
    }
}

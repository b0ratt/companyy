using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using companyy;

namespace CompanyTeest
{
    [TestClass]
    public class UpdateCommandHandler
    {
        [TestMethod]
        public void UpdateCommandHandlerTest()
        {
            // Arrange
            Customers newCustomer = new Customers
            {
                Address = "Mikolajczyka",
                City = "Rzeszow",
                CompanyName = "TestingCompany",
                ContactName = "Jan",
                ContactTitle = "Prezes",
                Country = "Poland",
                CustomerID = "1234567891111",
                Fax = "-",
                Phone = "511-522-533",
                PostalCode = "32323",
                Region = "Podkarpackie"
            };

            // Act
            int id_expected = 9;

            // Assert
            Assert.IsFalse((id_expected < newCustomer.CustomerID.Length), "Too long CustomerID");
        }

        [TestMethod]
        public void UpdateCommandOrderTest()
        {
            Customers newCustomer = new Customers
            {
                Address = "Mikolajczyka",
                City = "Rzeszow",
                CompanyName = "TestingCompany",
                ContactName = "Jan",
                ContactTitle = "Prezes",
                Country = "Poland",
                CustomerID = "1234567891111",
                Fax = "-",
                Phone = "511-522-533",
                PostalCode = "32323",
                Region = "Podkarpackie"
            };

            Orders newOrder = new Orders()
            {
                CustomerID = newCustomer.CustomerID,
                ShipAddress = newCustomer.Address,
                ShipCity = newCustomer.City,
                ShipCountry = newCustomer.Country,
                ShipName = newCustomer.CompanyName,
                ShipPostalCode = newCustomer.PostalCode,
                ShipRegion = newCustomer.Region
            };

            int id_customer = 9;

            Assert.IsTrue((id_customer < newCustomer.CustomerID.Length), "eeeee");


        }
    }
}

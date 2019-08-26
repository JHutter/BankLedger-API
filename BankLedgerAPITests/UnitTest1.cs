using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace BankLedgerAPITests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            var testProducts = GetTestUsers();
            var controller = new UsersController(testProducts);

            var result = await controller.GetAllProductsAsync() as List<User>;
            Assert.AreEqual(testProducts.Count, result.Count);
        }

        private List<User> GetTestUsers()
        {
            var testProducts = new List<User>();
            testProducts.Add(new Product { Id = 1, Name = "Demo1", Price = 1 });
            testProducts.Add(new Product { Id = 2, Name = "Demo2", Price = 3.75M });
            testProducts.Add(new Product { Id = 3, Name = "Demo3", Price = 16.99M });
            testProducts.Add(new Product { Id = 4, Name = "Demo4", Price = 11.00M });

            return testProducts;
        }
    }
}

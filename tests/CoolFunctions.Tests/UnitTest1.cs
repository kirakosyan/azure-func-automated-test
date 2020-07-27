using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace CoolFunctions.Tests
{
    [TestClass]
    public class UnitTest1 : Base
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            string parsedData = "";
            var result = await Client.GetAsync("api/Function1?name=armen");

            if (result.IsSuccessStatusCode)
            {
                parsedData = await result.Content.ReadAsStringAsync();
            }
            else
            {
                Assert.Fail(result.StatusCode.ToString());
            }

            Assert.AreNotEqual("", parsedData);
            Assert.AreEqual("Hello, armen. This HTTP triggered function executed successfully.", parsedData);
        }
    }
}

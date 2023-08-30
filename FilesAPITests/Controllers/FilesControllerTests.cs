using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace FilesAPI.Controllers.Tests
{
    [TestClass()]
    public class FilesControllerTests
    {
        [TestMethod()]
        public async Task PostTest()
        {
            System.IO.File.WriteAllText("testfile.txt", "This is a test file.");
            var controller = new FilesController();
            using (var stream = System.IO.File.OpenRead("testfile.txt"))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/plain"
                };
                var actionResult = await controller.Post(file);
                Assert.IsNotNull(actionResult);

                var result = actionResult as CreatedAtRouteResult;
                Assert.IsNotNull(result);

                var fileResult = result.Value as File;
                Assert.IsNotNull(fileResult);

                Assert.IsTrue(fileResult.Id > 0);

                await controller.Delete(fileResult.Id);
            }
        }
    }
}
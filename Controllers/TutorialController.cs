using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApisTutorial.Models;

namespace WebApisTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorialController : ControllerBase
    {

        private string item1 = "{\"category\": \"vegetables\",\"Items\": [\"item1\",\"item2\"]}";
        private string item2 = "{\"category\": \"fruits\",\"Items\": [\"item12\",\"item22\"]}";


        //https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
        //https://docs.microsoft.com/en-us/azure/storage/common/storage-samples-dotnet?toc=%2fazure%2fstorage%2fblobs%2ftoc.json
        //Sample : https://docs.microsoft.com/en-us/samples/azure-samples/storage-blob-dotnet-getting-started/storage-blob-dotnet-getting-started/
        [Route("GetListItems")]
        [HttpGet]
        public async Task<IEnumerable<ItemList>> GetItems()
        {
            string connectionString = $"";

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

            var blobClient = cloudStorageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("tutorial");

            //if (!await container.ExistsAsync())
            //{
            await container.CreateIfNotExistsAsync();
            //}

            var x = await container.ListBlobsSegmentedAsync(null);
            var obj1 = JsonConvert.DeserializeObject<ItemList>(item1);
            if (!x.Results.Any(x => x.Container.GetBlobReference(obj1.Category).ExistsAsync().Result))
            {
                await container.GetBlockBlobReference(obj1.Category).UploadTextAsync(item1);
            }

            var obj2 = JsonConvert.DeserializeObject<ItemList>(item2);
            if (!x.Results.Any(x => x.Container.GetBlobReference(obj2.Category).ExistsAsync().Result))
            {
                await container.GetBlockBlobReference(obj2.Category).UploadTextAsync(item2);
            }


            List<ItemList> myItems = new List<ItemList>();
            var itemsss = JsonConvert.DeserializeObject<ItemList>(await container.GetBlockBlobReference(obj1.Category).DownloadTextAsync());
            myItems.Add(itemsss);
            itemsss = JsonConvert.DeserializeObject<ItemList>(await container.GetBlockBlobReference(obj2.Category).DownloadTextAsync());
            myItems.Add(itemsss);


            return myItems;
        }

        //https://docs.microsoft.com/bs-latn-ba/azure/visual-studio/vs-storage-aspnet5-getting-started-tables?toc=/aspnet/core/toc.json&bc=/aspnet/core/breadcrumb/toc.json&view=aspnetcore-3.0
        [Route("GetTableDetails")]
        [HttpGet]
        public async Task<string> GetTableDetails()
        {
            string connectionString = $"";

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

            var tableClient = cloudStorageAccount.CreateCloudTableClient();

            var tableDetails = tableClient.GetTableReference("Account");

            if (!await tableDetails.ExistsAsync())
            {
                await tableDetails.CreateAsync();
                var insertOperation = TableOperation.Insert(new Account { EmailAddress = "avais@mine.com", Password = "password1" });
                await tableDetails.ExecuteAsync(insertOperation);
            }

            TableOperation retrieveOperation = TableOperation.Retrieve<Account>("Account", "avais@mine.com");

            var res = await tableDetails.ExecuteAsync(retrieveOperation);

            return JsonConvert.SerializeObject(res.Result);

        }


    }
}
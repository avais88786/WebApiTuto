using Microsoft.WindowsAzure.Storage.Table;

namespace WebApisTutorial.Models
{
    public class Account : TableEntity
    {
        public Account()
        {
            this.PartitionKey = "Account";
        }
        public string EmailAddress { get { return RowKey; } set { RowKey = value; } }

        public string Password { get; set; }
    }
}

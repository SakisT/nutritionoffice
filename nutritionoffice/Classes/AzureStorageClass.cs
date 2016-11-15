using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace nutritionoffice.Classes
{
    public class AzureStorageClass
    {
        public static async Task<TextReader> GetRDLC(string blobcontainer, string blobfile)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=nutritionoffice;AccountKey=uuksyz755IFe4IBj8v7j1g2ah4BN4BCtAu9USz8WTvtUekOZkqkYmEfYY/eY1L0gPRuZYi2icZJiZALmFdAhwA==");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(blobcontainer);
            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (StorageException)
            {
                throw;
            }
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobfile);

            Stream m =new  System.IO.MemoryStream();

            await blockBlob.DownloadToStreamAsync(m);
            m.Position = 0;
            return new StreamReader(m);
        }

        public static void StoreData(string blobcontainer, Stream blobfile, string filename)
        {
            CloudStorageAccount storageAccount =CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=nutritionoffice;AccountKey=uuksyz755IFe4IBj8v7j1g2ah4BN4BCtAu9USz8WTvtUekOZkqkYmEfYY/eY1L0gPRuZYi2icZJiZALmFdAhwA==");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(blobcontainer);
            try
            {
                container.CreateIfNotExists();
            }
            catch (StorageException)
            {
                throw;
            }
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);
            blockBlob.UploadFromStream(blobfile);
        }
    }
}
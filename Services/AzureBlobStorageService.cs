using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ImageClassificationProject.Services
{
    public class AzureBlobStorageService : IStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            // appsettings.json dosyasından bağlantı dizisini oku
            var connectionString = configuration["AzureStorageSettings:ConnectionString"];
            _containerName = configuration["AzureStorageSettings:ContainerName"];

            // BlobServiceClient'i bağlantı dizesi ile başlat
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty.", nameof(file));
            }

            // Konteyner referansını al ve eğer yoksa oluştur
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Dosya adını benzersiz hale getir (Guid + orijinal dosya uzantısı)
            var extension = Path.GetExtension(file.FileName);
            var blobFileName = $"{Guid.NewGuid()}{extension}";

            // BlobClient referansını al
            var blobClient = containerClient.GetBlobClient(blobFileName);

            // Dosyayı Blob'a yükle (Stream kullanarak)
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            // Yüklenen dosyanın URI'sini döndür
            return blobClient.Uri.ToString();
        }
    }
}
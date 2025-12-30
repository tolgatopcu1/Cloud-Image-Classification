using Microsoft.AspNetCore.Http;

namespace ImageClassificationProject.Services
{
    public interface IStorageService
    {
        /// <summary>
        /// Uploads a file to Azure Blob Storage.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <returns>The full URI of the uploaded blob.</returns>
        Task<string> UploadFileAsync(IFormFile file);
    }
}
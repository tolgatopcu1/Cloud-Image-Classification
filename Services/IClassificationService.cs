namespace ImageClassificationProject.Services
{
    public interface IClassificationService
    {
        /// <summary>
        /// Classifies an image using its publicly accessible URI.
        /// </summary>
        /// <param name="imageUri">The URI of the image hosted on Azure Blob Storage.</param>
        /// <returns>A tuple containing the classification result string and the confidence score.</returns>
        Task<(string Result, double Confidence)> ClassifyImageAsync(string imageUri);
    }
}
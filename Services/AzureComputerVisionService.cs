using Microsoft.Azure.CognitiveServices.Vision.ComputerVision; // Değişen paket
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models; // Yeni modeller
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ImageClassificationProject.Services
{
    public class AzureComputerVisionService : IClassificationService
    {
        private readonly ComputerVisionClient _client;
        
        private static readonly List<VisualFeatureTypes?> Features = new List<VisualFeatureTypes?> { VisualFeatureTypes.Tags };

        public AzureComputerVisionService(IConfiguration configuration)
        {
            var endpoint = configuration["AzureComputerVisionSettings:Endpoint"];
            var key = configuration["AzureComputerVisionSettings:Key"];

            _client = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(key), 
                new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = endpoint
            };
        }

        public async Task<(string Result, double Confidence)> ClassifyImageAsync(string imageUri)
        {
            if (string.IsNullOrEmpty(imageUri))
            {
                throw new ArgumentException("Image URI cannot be null or empty.", nameof(imageUri));
            }

            try
            {
                ImageAnalysis result = await _client.AnalyzeImageAsync(imageUri, Features);

                if (result.Tags != null && result.Tags.Any())
                {
                    var topTag = result.Tags.OrderByDescending(t => t.Confidence).First();
                    
                    return (topTag.Name, topTag.Confidence);
                }
                else
                {
                    return ("No classification found", 0.0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Computer Vision API Error: {ex.Message}");
                throw new Exception("Classification failed due to an error in the Azure Computer Vision API call.", ex);
            }
        }
    }
}
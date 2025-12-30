using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ImageClassificationProject.Models.ViewModels
{
    public class ClassificationViewModel
    {
        // Kullanıcının yükleyeceği dosya
        [Required(ErrorMessage = "Please select an image file.")]
        public IFormFile ImageFile { get; set; }

        // Sınıflandırma sonucu (Azure'dan geldikten sonra doldurulacak)
        public string ResultMessage { get; set; } = string.Empty;
    }
}
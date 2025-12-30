namespace ImageClassificationProject.Models
{
    public class ClassificationHistory
    {
        public int Id { get; set; } 

        public string FileName { get; set; } = string.Empty;

        public string BlobUri { get; set; } = string.Empty;

        public string ClassificationResult { get; set; } = string.Empty;

        public double ConfidenceScore { get; set; } 

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
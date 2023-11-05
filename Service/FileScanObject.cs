using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace AzureCognitiveServicesImageAnalysis_ConsoleApp.Service
{
    public class FileScanObject
    {
        public ImageAnalysis imageAnalysis { get; set; }
        public DetectResult detectResult { get; set; }
        public ReadResult readResult { get; set; }
    }

    public class ReadResult
    {
        public ReadOperationResult readOperationResult { get; set; }
        public List<string> readItems { get; set; }

        public ReadResult()
        {
            readOperationResult = new ReadOperationResult();
            readItems = new List<string>();
        }
    }
}

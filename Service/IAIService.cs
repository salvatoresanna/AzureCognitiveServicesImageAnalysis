using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace AzureCognitiveServicesImageAnalysis_ConsoleApp.Service
{
    public interface IAIServiceScan
    {
        Task<FileScanObject> ScanFile(FileStream fileStream);
        Task<FileScanObject> ScanFile(string filePath);
    }

    public interface IAIServiceAnalysis
    {
        Task<ImageAnalysis> GetAnalysisFromStream(FileStream fileStream);
        Task<ImageAnalysis> GetAnalysisFromLocalImage(string filePath);
    }

    public interface IAIServiceObjects
    {
        Task<DetectResult> GetObjectsFromStream(FileStream fileStream);
        Task<DetectResult> GetObjectsFromLocalImage(string filePath);
    }

    public interface IAIServiceRead
    {
        Task<ReadResult> ReadFromStream(FileStream fileStream);
        Task<ReadResult> ReadFromLocalImage(string filePath);
        Task<ReadResult> ReadFromOnlineImage(string fileUrl);
    }
}
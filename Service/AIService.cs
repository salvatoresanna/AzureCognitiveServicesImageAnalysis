using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using NLog;

namespace AzureCognitiveServicesImageAnalysis_ConsoleApp.Service
{
    public class AIServiceScan : IAIServiceScan
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IAIServiceAnalysis _aIServiceAnalysis;
        private readonly IAIServiceObjects _aIServiceObjects;
        private readonly IAIServiceRead _aIServiceRead;
        public FileScanObject fileScanObject { get; set; }



        public AIServiceScan(IAIServiceAnalysis aIServiceAnalysis,
                             IAIServiceObjects aIServiceObjects,
                             IAIServiceRead aIServiceRead)
        {
            _aIServiceAnalysis = aIServiceAnalysis ?? throw new ArgumentNullException(nameof(aIServiceAnalysis));
            _aIServiceObjects = aIServiceObjects ?? throw new ArgumentNullException(nameof(aIServiceObjects));
            _aIServiceRead = aIServiceRead ?? throw new ArgumentNullException(nameof(aIServiceRead));
            fileScanObject = new FileScanObject();
        }



        public async Task<FileScanObject> ScanFile(FileStream fileStream)
        {
            _logger.Info("Scanning file...");
            fileScanObject.imageAnalysis = await _aIServiceAnalysis.GetAnalysisFromStream(fileStream);
            fileScanObject.detectResult = await _aIServiceObjects.GetObjectsFromStream(fileStream);
            fileScanObject.readResult = await _aIServiceRead.ReadFromStream(fileStream);

            return fileScanObject;
        }

        public async Task<FileScanObject> ScanFile(string filePath)
        {
            _logger.Info("Scanning file...");
            fileScanObject.imageAnalysis = await _aIServiceAnalysis.GetAnalysisFromLocalImage(filePath);
            fileScanObject.detectResult = await _aIServiceObjects.GetObjectsFromLocalImage(filePath);
            fileScanObject.readResult = await _aIServiceRead.ReadFromLocalImage(filePath);

            return fileScanObject;
        }

    }


    public class AIServiceAnalysis : IAIServiceAnalysis
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IComputerVisionClient _computerVisionClient;



        public AIServiceAnalysis(IComputerVisionClient computerVisionClient)
        {
            _computerVisionClient = computerVisionClient ?? throw new ArgumentNullException(nameof(computerVisionClient));
        }

        public async Task<ImageAnalysis> GetAnalysisFromStream(FileStream fileStream)
        {
            ImageAnalysis imageAnalysis;

            using (fileStream)
            {
                var featureList = new List<VisualFeatureTypes?>()
                {
                    VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                    VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                    VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                    VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                    VisualFeatureTypes.Objects
                };

                try
                {
                    imageAnalysis = await _computerVisionClient.AnalyzeImageInStreamAsync(fileStream, featureList);
                    _logger.Info("Image analysis from stream succeeded.");
                }
                catch (Exception ex)
                {
                    _logger.Error("Error analyzing image from stream: " + ex.Message);
                    throw;
                }
            }

            return imageAnalysis;
        }

        public async Task<ImageAnalysis> GetAnalysisFromLocalImage(string filePath)
        {
            ImageAnalysis imageAnalysis;
            FileStream stream;

            using (stream = File.OpenRead(filePath))
            {
                var featureList = new List<VisualFeatureTypes?>()
                {
                    VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                    VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                    VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                    VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                    VisualFeatureTypes.Objects
                };

                try
                {
                    imageAnalysis = await _computerVisionClient.AnalyzeImageInStreamAsync(stream, featureList);
                    _logger.Info("Image analysis from local image succeeded.");
                }
                catch (Exception ex)
                {
                    _logger.Error("Error analyzing local image: " + ex.Message);
                    throw;
                }
            }

            return imageAnalysis;
        }


    }

    public class AIServiceObjects : IAIServiceObjects
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IComputerVisionClient _computerVisionClient;



        public AIServiceObjects(IComputerVisionClient computerVisionClient)
        {
            _computerVisionClient = computerVisionClient ?? throw new ArgumentNullException(nameof(computerVisionClient));
        }



        public async Task<DetectResult> GetObjectsFromStream(FileStream fileStream)
        {
            _logger.Info("Detecting objects from stream.");
            var startTime = DateTime.UtcNow;
            DetectResult detectResult;

            try
            {
                using (fileStream)
                {
                    detectResult = await _computerVisionClient.DetectObjectsInStreamAsync(fileStream);
                }

                _logger.Info($"Objects detected from stream in {(DateTime.UtcNow - startTime).TotalMilliseconds} ms.");

                return detectResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Error detecting objects from stream: " + ex.Message);
                throw;
            }
        }

        public async Task<DetectResult> GetObjectsFromLocalImage(string filePath)
        {
            _logger.Info("Detecting objects from local image.");
            var startTime = DateTime.UtcNow;
            DetectResult detectResult;

            try
            {
                using (var stream = File.OpenRead(filePath))
                {
                    detectResult = await _computerVisionClient.DetectObjectsInStreamAsync(stream);
                }

                _logger.Info($"Objects detected from local image in {(DateTime.UtcNow - startTime).TotalMilliseconds} ms.");

                return detectResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Error detecting objects from local image: " + ex.Message);
                throw;
            }
        }
    }

    public class AIServiceRead : IAIServiceRead
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IComputerVisionClient _computerVisionClient;



        public AIServiceRead(IComputerVisionClient computerVisionClient)
        {
            _computerVisionClient = computerVisionClient ?? throw new ArgumentNullException(nameof(computerVisionClient));
        }



        public async Task<ReadResult> ReadFromStream(FileStream fileStream)
        {
            ReadResult readResult = new ReadResult();

            using (fileStream)
            {
                _logger.Info("Starting reading from stream.");
                var startTime = DateTime.UtcNow;

                try
                {
                    await ReadResults(readResult, fileStream);
                    _logger.Info($"Finished reading from stream in {(DateTime.UtcNow - startTime).TotalMilliseconds} ms.");
                }
                catch (Exception ex)
                {
                    _logger.Error("Error reading from stream: " + ex.Message);
                    throw;
                }
            }

            return readResult;
        }

        public async Task<ReadResult> ReadFromLocalImage(string filePath)
        {
            ReadResult readResult = new ReadResult();

            using (var stream = File.OpenRead(filePath))
            {
                _logger.Info("Starting reading from stream.");
                var startTime = DateTime.UtcNow;

                try
                {
                    await ReadResults(readResult, stream);
                    _logger.Info($"Finished reading from stream in {(DateTime.UtcNow - startTime).TotalMilliseconds} ms.");
                }
                catch (Exception ex)
                {
                    _logger.Error("Error reading from stream: " + ex.Message);
                    throw;
                }
            }

            return readResult;
        }

        public async Task<ReadResult> ReadFromOnlineImage(string fileUrl)
        {
            _logger.Info("Starting online image reading.");
            var startTime = DateTime.UtcNow;

            try
            {
                var textHeaders = await _computerVisionClient.ReadAsync(fileUrl);
                var operationLocation = textHeaders.OperationLocation;
                var operationId = GetOperationId(operationLocation);

                ReadResult readResult = new ReadResult();

                do
                {
                    readResult.readOperationResult = await _computerVisionClient.GetReadResultAsync(Guid.Parse(operationId));
                }
                while (readResult.readOperationResult.Status == OperationStatusCodes.Running || readResult.readOperationResult.Status == OperationStatusCodes.NotStarted);

                var textUrlFileResults = readResult.readOperationResult.AnalyzeResult.ReadResults;
                foreach (var page in textUrlFileResults)
                {
                    foreach (var line in page.Lines)
                    {
                        readResult.readItems.Add(line.Text);
                    }
                }

                _logger.Info($"Finished online image reading in {(DateTime.UtcNow - startTime).TotalMilliseconds} ms.");

                return readResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Error reading online image: " + ex.Message);
                throw;
            }
        }

        private async Task ReadResults(ReadResult readResult, FileStream stream)
        {
            _logger.Info("Starting to read results.");
            var startTime = DateTime.UtcNow;

            try
            {
                var textHeaders = await _computerVisionClient.ReadInStreamAsync(stream);
                var operationLocation = textHeaders.OperationLocation;
                var operationId = GetOperationId(operationLocation);

                do
                {
                    readResult.readOperationResult = await _computerVisionClient.GetReadResultAsync(Guid.Parse(operationId));
                }
                while (readResult.readOperationResult.Status == OperationStatusCodes.Running || readResult.readOperationResult.Status == OperationStatusCodes.NotStarted);

                var textUrlFileResults = readResult.readOperationResult.AnalyzeResult.ReadResults;
                foreach (var page in textUrlFileResults)
                {
                    foreach (var line in page.Lines)
                    {
                        readResult.readItems.Add(line.Text);
                    }
                }

                _logger.Info($"Finished reading results in {(DateTime.UtcNow - startTime).TotalMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error reading results: " + ex.Message);
                throw;
            }
        }

        private string GetOperationId(string operationLocation)
        {
            var numberOfCharsInOperationId = 36;
            var operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);
            return operationId;
        }
    }
}

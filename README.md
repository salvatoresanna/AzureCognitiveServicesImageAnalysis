# AzureCognitiveServicesImageAnalysis
Use of Azure Cognitive Services for simple image analysis.


Example of a simple App Console with which you can scan an image using the Azure Cognitive Services.
By scanning you can:
- perform the analysis (Microsoft.Azure.CognitiveServices.Vision.ComputerVision.AnalyzeImageInStreamAsync)
- make the recognition of the objects present (Microsoft.Azure.CognitiveServices.Vision.ComputerVision.DetectObjectsInStreamAsync)
- obtain the OCR (Microsoft.Azure.CognitiveServices.Vision.ComputerVision.GetReadResultAsync)

The CA implements:
- appSettings.json configuration file
- logging with NLog
- dependency injectionz

The result of the complete analysis is given in a JSON structured in three main sections (one for each process):
- imageAnalysis
- detectResult
- readResult

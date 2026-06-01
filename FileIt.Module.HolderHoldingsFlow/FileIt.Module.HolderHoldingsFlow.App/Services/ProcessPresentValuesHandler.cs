namespace FileIt.Module.HolderHoldingsFlow.App.Services;

public class ProcessPresentValuesHandler
{
    private readonly PresentValueIngestionService _ingestionService;

    public ProcessPresentValuesHandler(PresentValueIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    public async Task HandleAsync(/* trigger input */)
    {
        // Orchestrate the flow
        var list = new List<string>(); // This would come from the trigger input in a real implementation
        await _ingestionService.LoadPresentValuesAsync(list);
    }
}





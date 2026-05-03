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
        await _ingestionService.LoadPresentValuesAsync();
    }
}





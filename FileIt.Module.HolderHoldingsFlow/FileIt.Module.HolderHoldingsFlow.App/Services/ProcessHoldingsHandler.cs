namespace FileIt.Module.HolderHoldingsFlow.App.Services;

public class ProcessHoldingsHandler
{
    private readonly HoldingIngestionService _ingestionService;

    public ProcessHoldingsHandler(HoldingIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    public async Task HandleAsync(/* trigger input */)
    {
        // Orchestrate the flow
        await _ingestionService.LoadHoldingsAsync();
    }
}

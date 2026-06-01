namespace FileIt.Module.HolderHoldingsFlow.App.Services;

public class ProcessHoldersHandler
{
    private readonly HolderIngestionService _ingestionService;

    public ProcessHoldersHandler(HolderIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    public async Task HandleAsync(/* trigger input */)
    {
        // Orchestrate the flow
        await _ingestionService.LoadHoldersAsync();
    }
}

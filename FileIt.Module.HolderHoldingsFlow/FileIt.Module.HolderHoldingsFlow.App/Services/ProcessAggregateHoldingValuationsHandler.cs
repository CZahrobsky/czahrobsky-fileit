namespace FileIt.Module.HolderHoldingsFlow.App.Services;

public class ProcessAggregateHoldingValuationsHandler
{
    private readonly AggregateHoldingValuationsService _ingestionService;

    public ProcessAggregateHoldingValuationsHandler(AggregateHoldingValuationsService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    public async Task HandleAsync(/* trigger input */)
    {
        // Orchestrate the flow
        await _ingestionService.LoadAggregateHoldingValuationsAsync();
    }
}

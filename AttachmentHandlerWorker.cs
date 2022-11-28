using Koala.AttachmentHandlerService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.AttachmentHandlerService;

public class AttachmentHandlerWorker : IHostedService
{
    private readonly IServiceBusHandler _serviceBusHandler;

    public AttachmentHandlerWorker(IServiceBusHandler serviceBusHandler)
    {
        _serviceBusHandler = serviceBusHandler;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _serviceBusHandler.InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _serviceBusHandler.DisposeAsync()!;
        await _serviceBusHandler.CloseQueueAsync()!;
    }
}
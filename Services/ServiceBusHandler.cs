using Koala.AttachmentHandlerService.Services.Interfaces;

namespace Koala.AttachmentHandlerService.Services;

public class ServiceBusHandler : IServiceBusHandler
{
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task? CloseQueueAsync()
    {
        return Task.CompletedTask;
    }

    public Task? DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
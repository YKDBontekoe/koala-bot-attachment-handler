namespace Koala.AttachmentHandlerService.Services.Interfaces;

public interface IServiceBusHandler
{
    public Task InitializeAsync();
    Task? CloseQueueAsync();
    Task? DisposeAsync();
}
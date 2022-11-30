using Azure.Messaging.ServiceBus;
using Koala.AttachmentHandlerService.Models;
using Koala.AttachmentHandlerService.Options;
using Koala.AttachmentHandlerService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Koala.AttachmentHandlerService.Services;

public class ServiceBusHandler : IServiceBusHandler
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ServiceBusOptions _serviceBusOptions;
    private ServiceBusProcessor? _processor;

    public ServiceBusHandler(ServiceBusClient serviceBusClient, IOptions<ServiceBusOptions> serviceBusOptions)
    {
        _serviceBusClient = serviceBusClient;
        _serviceBusOptions = serviceBusOptions != null ? serviceBusOptions.Value : throw new ArgumentNullException(nameof(serviceBusOptions));
    }

    public async Task InitializeAsync()
    {
        _processor = _serviceBusClient.CreateProcessor(_serviceBusOptions.AttachmentHandlerQueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(15),
            PrefetchCount = 100,
        });
        
        try
        {
            // add handler to process messages
            _processor.ProcessMessageAsync += MessagesHandler;

            // add handler to process any errors
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task CloseQueueAsync()
    {
        if (_processor != null) await _processor.CloseAsync();
    }

    public async Task DisposeAsync()
    {
        if (_processor != null) await _processor.DisposeAsync();
    }

    private async Task MessagesHandler(ProcessMessageEventArgs args)
    {
        // Process the message.
        var body = args.Message.Body.ToString();
        var message = JsonConvert.DeserializeObject<Message>(body);

        if (message is null) return;
        if (message.Channel.IsNsfw) return;

        var sender = _serviceBusClient.CreateSender(_serviceBusOptions.AttachmentConsumerQueueName);
        await sender.SendMessageAsync(new ServiceBusMessage(body));
    }
    
    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        // Process the error.
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}
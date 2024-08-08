using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using AWS.Lambda.Powertools.BatchProcessing;
using AWS.Lambda.Powertools.BatchProcessing.Sqs;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Tracing;
using Microsoft.Extensions.DependencyInjection;
using MyCoolFunction.Sqs;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyCoolFunction;

public class Function
{
    private readonly IServiceProvider _serviceProvider;

    public Function()
    {
        _serviceProvider = Services.Provider;
    }
    
    [Logging(LogEvent = true, ClearState= true)]
    [Tracing]
    public async Task<BatchItemFailuresResponse> FunctionHandler(SQSEvent @event, ILambdaContext _context)
    {
        var batchProcessor = _serviceProvider.GetRequiredService<ISqsBatchProcessor>();
        var recordHandler = _serviceProvider.GetRequiredService<ISqsRecordHandler>();

        var result = await batchProcessor.ProcessAsync(@event, recordHandler);

        return result.BatchItemFailuresResponse;
    }
}

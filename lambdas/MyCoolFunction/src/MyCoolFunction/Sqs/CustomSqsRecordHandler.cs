using System.Text.Json;
using Amazon.Lambda.SQSEvents;
using AWS.Lambda.Powertools.BatchProcessing;
using AWS.Lambda.Powertools.BatchProcessing.Sqs;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Tracing;
using MyCoolFunction.Models;

namespace MyCoolFunction.Sqs;
public class CustomSqsRecordHandler : ISqsRecordHandler
{
    [Tracing]
    public async Task<RecordHandlerResult> HandleAsync(SQSEvent.SQSMessage record, CancellationToken cancellationToken)
    {
        /*
         * Your business logic.
         * If an exception is thrown, the item will be marked as a partial batch item failure.
         */
        Logger.LogInformation($"Handling SQS record with message id: '{record.MessageId}'.");

        Logger.LogInformation($"Handling record with body: {record.Body}");

        var product = JsonSerializer.Deserialize<Product>(record.Body);

        SomeCoolFunction();
        SomeCoolFunction();

        Logger.LogInformation($"Handling product with id: {product!.SomeCoolField}");

        return await Task.FromResult(RecordHandlerResult.None);
    }

    [Tracing]
    private static bool SomeCoolFunction()
    {
        return true;
    }
}
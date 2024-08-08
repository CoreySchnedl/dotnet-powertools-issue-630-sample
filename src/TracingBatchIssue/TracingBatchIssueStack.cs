using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;

namespace TracingBatchIssue
{
    public class TracingBatchIssueStack : Stack
    {
        internal TracingBatchIssueStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var role = new Role(this, "LambdaRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });

            role.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new[] { "logs:CreateLogGroup", "logs:CreateLogStream", "logs:PutLogEvents" },
                Resources = new[] { "arn:aws:logs:*:*:*" }
            }));

            role.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new[] { "xray:PutTraceSegments", "xray:PutTelemetryRecords" },
                Resources = new[] { "*" }
            }));

            var buildOption = new BundlingOptions()
            {
                Image = Runtime.DOTNET_8.BundlingImage,
                User = "root",
                OutputType = BundlingOutput.ARCHIVED,
                Command = new string[]{
                    "/bin/sh",
                    "-c",
                    " dotnet tool install -g Amazon.Lambda.Tools"+
                    " && dotnet build"+
                    " && dotnet lambda package --output-package /asset-output/function.zip"
                }
            };

            var myCoolFunction = new Function(this, "MyCoolFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "MyCoolFunction::MyCoolFunction.Function::FunctionHandler",
                Code = Code.FromAsset("./lambdas/MyCoolFunction/src/MyCoolFunction", new Amazon.CDK.AWS.S3.Assets.AssetOptions
                {
                    Bundling = buildOption
                }),
                MemorySize = 512,
                Timeout = Duration.Seconds(30),
                Tracing = Tracing.ACTIVE,
                Environment = new Dictionary<string, string>()
                {
                    { "POWERTOOLS_SERVICE_NAME", "CoolService" },
                    { "POWERTOOLS_TRACER_CAPTURE_RESPONSE", "true" },
                    { "POWERTOOLS_TRACER_CAPTURE_ERROR", "true" },
                    { "POWERTOOLS_LOG_LEVEL", "Debug" },
                },
                Role = role
            });

            var queue = new Amazon.CDK.AWS.SQS.Queue(this, "MyQueue");

            myCoolFunction.AddEventSource(new Amazon.CDK.AWS.Lambda.EventSources.SqsEventSource(queue));
        }
    }
}

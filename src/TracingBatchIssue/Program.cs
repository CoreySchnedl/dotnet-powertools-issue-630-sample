using Amazon.CDK;

namespace TracingBatchIssue
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            _ = new TracingBatchIssueStack(app, "TracingBatchIssueStack", new StackProps { });
            app.Synth();
        }
    }
}

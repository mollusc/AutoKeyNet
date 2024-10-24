using BenchmarkDotNet.Running;

namespace BenchmarkConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var test = new GetChunkOfListInputs();
            test.ListTest();
            //var summary = BenchmarkRunner.Run<GetChunkOfListInputs>();
        }
    }
}

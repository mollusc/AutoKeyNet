using System.Diagnostics;
using AutoKeyNet.WindowsHooks.Helper;
using AutoKeyNet.WindowsHooks.WindowsEnums;
using AutoKeyNet.WindowsHooks.WindowsStruct;
using BenchmarkDotNet.Attributes;

namespace BenchmarkConsoleApp
{
    [ShortRunJob]
    [MemoryDiagnoser]
    public class GetChunkOfListInputs
    {
        private readonly VirtualKey[] _virtualKeys;
        private readonly Random _random;

        public GetChunkOfListInputs()
        {
            _virtualKeys = Enum.GetValues<VirtualKey>();
            _random = new Random();

        }
        private const int Capaciti = 10;
        private const int ChunkSize = 4;
        private const int InputSizeFlow = 100;

        private Input GetRandomInput()
        {
            VirtualKey virtualKey = (VirtualKey)_virtualKeys.GetValue(_random.Next(_virtualKeys.Length));
            return virtualKey.ToInput(KeyEventFlags.KEYDOWN);
        }


        [Benchmark]
        public void StackTest()
        {
            Stack<Input> inputs = new Stack<Input>(Capaciti);
            for (int i = 0; i < InputSizeFlow; i++)
            {
                inputs.Push(GetRandomInput());
                var chunk = inputs.SkipLast(Capaciti-ChunkSize).Take(ChunkSize);
                int t = chunk.Count();
                Debug.WriteLine($"Full: {string.Join(" ", inputs.Select(x => $"[{x}]"))}\nChunk:{string.Join(" ", chunk.Select(x => $"[{x}]"))} ");
            }
        }

        [Benchmark]
        public void ListTest()
        {
            List<Input> inputs = new List<Input>(Capaciti);
            for (int i = 0; i < InputSizeFlow; i++)
            {
                inputs.Add(GetRandomInput());
                var chunk = inputs.Skip(Capaciti-ChunkSize).Take(ChunkSize);
                int t = chunk.Count();
                Debug.WriteLine($"Full: {string.Join(" ", inputs.Select(x => $"[{x}]"))}\nChunk:{string.Join(" ", chunk.Select(x => $"[{x}]"))} ");
            }
        }


    }
}

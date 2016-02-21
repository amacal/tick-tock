using F2F.Sandbox;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace TickTock.Runner.Tests
{
    public class JobTaskManagerFixture : IDisposable
    {
        private readonly FileSandbox sandbox;
        private readonly JobTaskManager manager;

        public JobTaskManagerFixture()
        {
            sandbox = new FileSandbox(new EmptyFileLocator());
            manager = new JobTaskManager();
        }

        [Fact]
        public void NotStartedTaskReturnsFalseOnIsWorking()
        {
            JobTask job = CreateTask("5");

            CompileExecutable(job);
            bool isWorking = manager.IsWorking(job);

            isWorking.Should().BeFalse();
        }

        [Fact]
        public void StartedAndStoppedTaskReturnsFalseOnIsWorking()
        {
            JobTask job = CreateTask("5");

            CompileExecutable(job);
            manager.Start(job).Wait();

            bool isWorking = manager.IsWorking(job);

            isWorking.Should().BeFalse();
        }

        [Fact]
        public void StartedButNotStoppedTaskReturnsTrueOnIsWorking()
        {
            bool isWorking = false;
            JobTask job = CreateTask("5");

            CompileExecutable(job);
            Task task = manager.Start(job);

            do
            {
                isWorking |= manager.IsWorking(job);
            }
            while (task.Wait(TimeSpan.FromMilliseconds(100)) == false);

            isWorking.Should().BeTrue();
        }

        public void Dispose()
        {
            sandbox.Dispose();
        }

        private JobTask CreateTask(string seconds)
        {
            return new JobTask
            {
                Path = sandbox.Directory,
                Executable = Path.Combine(sandbox.Directory, "tick-tock.exe"),
                Arguments = seconds,
                WorkingDirectory = sandbox.Directory
            };
        }

        private void CompileExecutable(JobTask task)
        {
            string code = @"using System;
                            using System.Threading;

                            namespace TickTock
                            {
                                public static class Program
                                {
                                    static void Main(string[] args)
                                    {
                                        Thread.Sleep(TimeSpan.FromSeconds(Int32.Parse(args[0])));
                                    }
                                }
                            }";

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            MetadataReference reference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            CSharpCompilation compilation = CSharpCompilation.Create("tick-tock").AddReferences(reference).AddSyntaxTrees(tree);
            EmitResult result = compilation.Emit(task.Executable);
        }
    }
}
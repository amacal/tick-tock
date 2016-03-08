using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TickTock.Core.Blobs;
using TickTock.Core.Executions;
using TickTock.Core.Extensions;

namespace TickTock.Runner
{
    public static class JobTaskFactory
    {
        public static JobTask Build(Action<JobTaskFactoryContext> with)
        {
            return with.Apply(context =>
            {
                return new JobTask
                {
                    Identifier = context.Execution.Identifier,
                    Start = Start(context),
                    Statistics = new JobTaskStatistics
                    {
                        Publish = PublishStatistics(context)
                    }
                };
            });
        }

        private static Action Start(JobTaskFactoryContext context)
        {
            return () =>
            {
                context.Job.Extract(data =>
                {
                    Blob blob = context.Blobs.GetById(data.Blob);
                    BlobDeployment deployment = context.Execution.Deploy(blob);

                    if (deployment == null)
                    {
                        context.Execution.Progress.OnFailed("Blob deployment failed.");
                        return;
                    }

                    string executable = deployment.Find(data.Executable);
                    string workingDirectory = Path.GetDirectoryName(executable);

                    ProcessStartInfo info = new ProcessStartInfo
                    {
                        FileName = executable,
                        Arguments = data.Arguments,
                        WorkingDirectory = workingDirectory,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    Process process = new Process
                    {
                        StartInfo = info,
                        EnableRaisingEvents = true
                    };

                    process.Exited += (sender, args) =>
                    {
                        context.Execution.Progress.OnCompleted();
                        process.Dispose();
                    };

                    context.Execution.Progress.OnScheduled();

                    try
                    {
                        if (process.Start() == false)
                        {
                            context.Execution.Progress.OnFailed("Process starting failed.");
                            return;
                        }
                    }
                    catch (Win32Exception ex)
                    {
                        context.Execution.Progress.OnFailed($"Process starting failed: {ex.Message}");
                        return;
                    }

                    context.Execution.Progress.OnStarted(process.Id);
                });
            };
        }

        private static Action PublishStatistics(JobTaskFactoryContext context)
        {
            return () =>
            {
                OnProcess(context.Execution, process =>
                {
                    context.Execution.Metrics.OnMemory(new JobMemoryUsage
                    {
                        NonpagedSystemMemorySize = process.NonpagedSystemMemorySize64
                    });

                    context.Execution.Metrics.OnProcessor(new JobProcessorUsage
                    {
                        TotalProcessorTime = process.TotalProcessorTime,
                        UserProcessorTime = process.UserProcessorTime
                    });
                });
            };
        }

        private static void OnProcess(JobExecution execution, Action<Process> callback)
        {
            int pid = execution.Progress.GetPid();
            Process process = Process.GetProcesses().FirstOrDefault(x => x.Id == pid);

            if (process != null)
                callback(process);
        }
    }
}
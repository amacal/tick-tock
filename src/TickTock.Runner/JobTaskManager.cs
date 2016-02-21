using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TickTock.Runner
{
    public class JobTaskManager
    {
        public Task Start(JobTask task)
        {
            TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = task.Executable,
                Arguments = task.Arguments,
                WorkingDirectory = task.WorkingDirectory,
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
                File.WriteAllText(Path.Combine(task.Path, ".completed"), String.Empty);

                completion.SetResult(true);
                process.Dispose();
            };

            process.Start();

            string pid = Path.Combine(task.Path, $".pid.{process.Id}");
            File.WriteAllText(pid, String.Empty);

            return completion.Task;
        }

        public void Stop(JobTask task)
        {
        }

        public bool IsWorking(JobTask task)
        {
            return File.Exists(Path.Combine(task.Path, ".completed")) == false
                && Directory.GetFiles(task.Path, ".pid.*").Length == 1;
        }

        public JobRuntime GetRuntime(JobTask task)
        {
            return new JobRuntime
            {
                Task = task
            };
        }
    }
}
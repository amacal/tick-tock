namespace TickTock.Runner
{
    public class JobTask
    {
        public string Path { get; set; }

        public string Executable { get; set; }

        public string Arguments { get; set; }

        public string WorkingDirectory { get; set; }
    }
}
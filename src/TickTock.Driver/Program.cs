using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace TickTock.Driver
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string host = args[0];
            string command = args[1];

            switch (command)
            {
                case "blob":
                    UploadBlob(host, args[2]);
                    break;

                case "job":
                    CreateJob(host, args[2], args[3], args[4], args[5]);
                    break;
            }
        }

        private static void UploadBlob(string host, string path)
        {
            using (WebClient client = new WebClient())
            {
                client.BaseAddress = host;

                byte[] data = client.UploadData("blobs", "POST", File.ReadAllBytes(path));
                dynamic response = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data));

                Console.WriteLine(response.id);
            }
        }

        private static void CreateJob(string host, string name, string executable, string arguments, string blob)
        {
            using (WebClient client = new WebClient())
            {
                client.BaseAddress = host;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                object request = new
                {
                    name = name,
                    executable = executable,
                    arguments = arguments,
                    blob = blob
                };

                string body = JsonConvert.SerializeObject(request);
                string data = client.UploadString("jobs", "POST", body);
                dynamic response = JsonConvert.DeserializeObject(data);

                Console.WriteLine(response.id);
            }
        }
    }
}
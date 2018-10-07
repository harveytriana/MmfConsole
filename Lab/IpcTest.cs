using MML.Shared;
using System;
using System.Collections.Generic;
using System.Threading;

// Sample in production case, external Ipc proivider
// The model has to exist in a DLL
// new Lab.IpcTest().RunFromExternal();
//    

namespace Lab
{
    class IpcTest
    {
        const string mapFile = "SAMP-CO-F-0001";

        readonly IpcWriter w = new IpcWriter(mapFile);
        readonly IpcReader r = new IpcReader(mapFile);

        public void Run()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("C# Send Content Between Processes (Memory Mapped File)\n");
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("Write something...");

            WriteSomething();

            Console.WriteLine("Press any key to read it...");
            Console.ReadKey();

            Console.WriteLine("Read");

            ReadSomething();

            Console.ReadLine();
        }

        private void ReadSomething()
        {
            var data = r.Get<OnlineRecord>();

            Console.WriteLine(data);
        }

        private void WriteSomething()
        {
            w.Put(Sample());
        }

        private OnlineRecord Sample()
        {
            var r = new OnlineRecord {
                ID = "SAMP-CO-F-0001",
                Time = DateTime.Now,
                Depth = 1234.12F,
                Content = new List<Parameter>()
            };
            r.Content.Add(new Parameter { ID = "ROP", Value = 122.23F });
            r.Content.Add(new Parameter { ID = "WOB", Value = 2.423F });
            r.Content.Add(new Parameter { ID = "RPM", Value = 120.03F });
            return r;
        }

        // For this, imperative: The model has to be a DLL
        // in this case I use LabCore21.Models.dll
        public void RunFromExternal()
        {
            for (int i = 1; i <= 10; i++) {
                ReadSomething();
                Thread.Sleep(1000);
            }
        }
    }


}

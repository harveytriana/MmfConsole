// Samples
// Visonary S.A.S.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization.Formatters.Binary;

// Begin: http://bit.ly/2zWrtMf

namespace MmfConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Lab.IpcTest().RunFromExternal();
            //return;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("C# Send Data Between Processes (Memory Mapped File)\n");
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("Write something...");

            WriteSomething();

            Console.WriteLine("Press any key to read it...");
            Console.ReadLine();

            Console.WriteLine("Read");

            ReadSomething2();

            Console.ReadLine();
        }

        private static void WriteSomething()
        {
            const int MMF_MAX_SIZE = 1024;  // allocated memory for this memory mapped file (bytes)
            const int MMF_VIEW_SIZE = 1024; // how many bytes of the allocated memory can this process access

            // creates the memory mapped file which allows 'Reading' and 'Writing'
            var mmf = MemoryMappedFile.CreateOrOpen("mmf1", MMF_MAX_SIZE, MemoryMappedFileAccess.ReadWrite);

            // creates a stream for this process, which allows it to write data from offset 0 to 1024 (whole memory)
            var mmvStream = mmf.CreateViewStream(0, MMF_VIEW_SIZE);

            // this is what we want to write to the memory mapped file
            var m = new Message {
                Title = "test",
                Parameters = new List<Parameter>(),
                Time = DateTime.Now
            };
            m.Parameters.Add(new Parameter { Mnemonic = "ROP", Value = 123.456F });
            m.Parameters.Add(new Parameter { Mnemonic = "WOB", Value = 2.321F });
            m.Parameters.Add(new Parameter { Mnemonic = "RPM", Value = 187.321F });

            // serialize the variable 'message1' and write it to the memory mapped file
            var formatter = new BinaryFormatter();
            formatter.Serialize(mmvStream, m);
            mmvStream.Seek(0, SeekOrigin.Begin); // sets the current position back to the beginning of the stream
        }

        private static void ReadSomething()
        {
            const int MMF_VIEW_SIZE = 1024; // how many bytes of the allocated memory can this process access

            // creates the memory mapped file
            MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("mmf1");
            MemoryMappedViewStream mmvStream = mmf.CreateViewStream(0, MMF_VIEW_SIZE); // stream used to read data

            BinaryFormatter formatter = new BinaryFormatter();

            // needed for deserialization
            byte[] buffer = new byte[MMF_VIEW_SIZE];

            mmvStream.Read(buffer, 0, MMF_VIEW_SIZE);

            // deserializes the buffer & prints the message
            var m = (Message)formatter.Deserialize(new MemoryStream(buffer));
            Console.WriteLine(m);
        }

       
        // Without use MMF_MAX_SIZE
        private static void ReadSomething2()
        {
            using (var mmf = MemoryMappedFile.OpenExisting("mmf1",
                            MemoryMappedFileRights.Read,
                            HandleInheritability.Inheritable)) {

                using (var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read)) {
                    var n = accessor.Capacity;
                    var buffer = new byte[n];
                    for (int i = 0; i < n; i++) {
                        buffer[i] = accessor.ReadByte(i);
                    }
                    var formatter = new BinaryFormatter();
                    var m = (Message)formatter.Deserialize(new MemoryStream(buffer));

                    Console.WriteLine(m);
                }
            }
        }
    }

    [Serializable]  // mandatory
    class Message
    {
        public string Title { get; set; }
        public DateTime Time { get; set; }
        public List<Parameter> Parameters { get; set; }

        public override string ToString()
        {
            var s = $"{Time.ToString("HH:mm:ss")}: {Title}";
            Parameters.ForEach(x => s += $"\n{x.Mnemonic}: {x.Value}");
            return s;
        }
    }

    [Serializable]  // mandatory
    class Parameter
    {
        public string Mnemonic { get; set; }
        public float Value { get; set; }
    }
}

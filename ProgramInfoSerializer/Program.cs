using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkTrackingLib.Models;


namespace ProgramInfoSerializer
{
    class Program
    { 
        static void Main(string[] args)
        {
            var programInfo = new ProgramInfo();

            programInfo.ProgramName = Console.ReadLine();
            programInfo.Version = Convert.ToInt32(Console.ReadLine());
            programInfo.VersionReleaseDate = DateTime.Now;

            var tempString = JsonConvert.SerializeObject(programInfo);

            File.WriteAllText(@"C:\serverInfo.json", tempString);

            Console.ReadKey();
        }
    }
}

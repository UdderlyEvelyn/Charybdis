using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Charybdis.MonoGame;
using Charybdis.Neural;
using Space;
using Fortress;

namespace Charybdis
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            PInvoke.KERNEL32.SetThreadExecutionState(PInvoke.KERNEL32.ES_CONTINUOUS | PInvoke.KERNEL32.ES_SYSTEM_REQUIRED);
            string message = "";
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine(
                    "Welcome to the Charybdis Project.\n" +
                    "1. Evolution Simulator\n" +
                    "2. Fortress (Matter State Simulator)\n" +
                    "3. Space (4X)\n" +
                    "4. Raytracer\n" +
                    "5. Exit\n" +
                    "\n" +
                    message
                    );
                var key = Console.ReadKey();
                switch (key.KeyChar)
                {
                    case '1':
                        using (var k = new EvolutionSimulator.EvolutionSimulator()) k.Run();
                        message = "";
                        break;
                    case '2':
                        using (var k = new FortressKernel()) k.Run();
                        message = "";
                        break;
                    case '3':
                        using (var k = new SpaceKernel()) k.Run();
                        message = "";
                        break;
                    case '4':
                        new System.Windows.Application().Run(new RayTracer.MainWindow());
                        message = "";
                        break;
                    case '5':
                        exit = true;
                        message = "";
                        break;
                    default:
                        message = "Invalid option.";
                        break;
                }
            }
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace ExecuteProcesses
{
    class ExecuteProcesses
    {

        // Start the processes TransceiverSimulation. They can only be opened by passing them as arguments. 
        void OpenWithArguments()
        {

            // Start the processes TransceiverSimulation. They can only be opened by passing them as arguments.
            for (int i = 4004; i <= 4005; i++)
            {
            string port = i.ToString();

            Process.Start("C://Users/zelma/Documents/Visual Studio 2013/Projects/LASEC/Transmissor/Transmissor/bin/Debug/Transmissor.exe", port);
            }
        }

        // Start the processes TransceiverSimulation. They can only be opened by passing them as arguments.
        // Uses the ProcessStartInfo class to start new processes, 
        // both in a minimized mode. 
        void OpenWithStartInfo()
        {
            // The path that stores processes TransceiverSimulation 
            ProcessStartInfo startInfo = new ProcessStartInfo("C://Users/zelma/Documents/Visual Studio 2013/Projects/LASEC/TransceiverSimulation/TransceiverSimulation/bin/Debug/TransceiverSimulation.exe");
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;

            for (int i = 4001; i <= 4050; i++)
            {
                string port= i.ToString();
                startInfo.Arguments = port;
                Process.Start(startInfo);
            }
        }
        
        // Start the processes Transmissor. They can only be opened by passing them as arguments.
        void OpenWithStartInfo2()
        {
             // The path that stores processes Transmissor
            ProcessStartInfo startInfo = new ProcessStartInfo("C://Users/zelma/Documents/Visual Studio 2013/Projects/LASEC/Lectura y simulación de varios transceiver automáticamente/Transmissor/Transmissor/bin/Debug/Transmissor.exe");
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;

            startInfo.WorkingDirectory = @"C://Users/zelma/Documents/Visual Studio 2013/Projects/LASEC/Lectura y simulación de varios transceiver automáticamente/Transmissor/Transmissor/bin/Debug/";

            for (int i = 4001; i <= 4050; i++)
            {
                string port = i.ToString();
                startInfo.Arguments = port;
                Process.Start(startInfo);
            }

        }

        static void Main()
        {
            ExecuteProcesses myProcess = new ExecuteProcesses();
            myProcess.OpenWithStartInfo();

            Thread.Sleep(5000);

            ExecuteProcesses myProcess2 = new ExecuteProcesses();
            //myProcess2.OpenWithArguments();
            myProcess2.OpenWithStartInfo2();

        }
    }
}



















        
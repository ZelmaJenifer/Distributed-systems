using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TransceiverSimulation 
{
    /*
     *This class 
     */
    class Program
    {
       static void Main(string[] args)
        {
            int port_number = 0;
            port_number = Convert.ToInt32(args[0]);
            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
          
            TcpListener serverSocket = new TcpListener(ipAddress, port_number);
           

            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" >> Server Started");
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine(" >> Accept connection from client");


            while ((true))
            {
                try
                {

                    NetworkStream ns = clientSocket.GetStream();

                    string serverResponse = "20,0000111E12FA,0,13,14";
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    ns.Write(sendBytes, 0, sendBytes.Length);

                    Thread.Sleep(1000);

                    Console.WriteLine(" >> " + serverResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> exit");
            Console.ReadLine();
        }
    }
}


       
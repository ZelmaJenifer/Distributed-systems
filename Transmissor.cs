using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitConfiguration;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;


/*
    Description: Define a Transmissor class
*/
class Transmissor
{
   
    ConnectionFactory factory;

    ConfigurationReader c_reader;

    public Transmissor(string file_path)
    {
        
        this.Setup(file_path);

    }

    /*
        This method creates a configuration reader object and
        setups the connection factory
    */
    public void Setup(string file_path)
    {

        c_reader = new ConfigurationReader(file_path);

        this.factory = new ConnectionFactory();
        this.factory.HostName = c_reader.GetHost();
        this.factory.UserName = c_reader.GetUser();
        this.factory.Password = c_reader.GetPassword();

    }

    /*
        This method send messages to the RabbitMQ Server 

    */
    public void transmit(byte[] message)
    {

        this.factory.HostName = c_reader.GetServer();
        using (var connection_transmit = factory.CreateConnection())
        {
            using (var channel = connection_transmit.CreateModel())
            {
                channel.ExchangeDeclare("transmission", "fanout");
                channel.BasicPublish("transmission", "", null, message);


                }

             }
          

     }

    /*
        This method is listening the responses from the server
    */
    public byte[] listen_response()
    {

        using (var connection_listen = factory.CreateConnection())
        {
            using (var channel = connection_listen.CreateModel())
            {

                channel.ExchangeDeclare("response", "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, "response", "");

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, true, consumer);

                Console.WriteLine(" [*] Waiting for logs." +
                                  "To exit press CTRL+C");

                byte[] body_message;
                while (true)
                {
                    var event_args = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    var body = event_args.Body;
                    body_message = body;
                    var message = Encoding.UTF8.GetString(body);
                    
                    Console.WriteLine(" [x] {0}", message);
                    return body_message;
                }

                
            }
        }

    }

    /*
        This method prepares the message
    */
    private static void PrepareMessage(string message)
    {
        Transmissor transmissor = new Transmissor("configuration.txt");
        var body = Encoding.UTF8.GetBytes(message);
        transmissor.transmit(body);
    }


    /*
      This method is listening the signals from the transceiver
    */

    public void ListenDevice(TcpClient tcp_client)
    {
        //byte[] buffer = new byte[100];
        try
            {
                while (true)
                {
                        StreamReader leer = new StreamReader(tcp_client.GetStream());

                        string s_received  = leer.ReadLine();
                        
                        if (s_received != "\0")
                        {

                            Console.WriteLine(" >> " + s_received);

                            PrepareMessage(s_received);
                            

                        }
                     
                    
                  

                }
               
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: {0}", error.ToString());
            
            }

    }

    public void SendDatatoDevice(Socket socket, byte[] message)
    {

        while (true)
        {
            //Prender leds
            //string str = "SDAT 1 000011DB9BF7 0B 08125554025500035B0C01 2000\r\n";
            //Apagar leds
            //string str = "SDAT 1 000011DB9BF7 0B 08125554025500035B0C00 2000\r\n";
            
            //string str = "SBIV 00100";
            //string str = "GDAT";
            //string str = "$%&/\r\n";
            
            
            byte[] buffer = new byte[100];
            //buffer = Encoding.ASCII.GetBytes(message);
            buffer = message;
            try
            { // sends the text with timeout 10s
                socket.Send(buffer);
                var str = Encoding.UTF8.GetString(message);
                Console.WriteLine("Enviado..."+ str);
                Thread.Sleep(2000);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message.ToString()); }
        }

    }


    public void ConnectToDevice(string ip, int port)
    {
        TcpClient tcp_client;
        Socket socket;

        tcp_client = new TcpClient(ip, port);
        socket = tcp_client.Client;

     
        Thread.Sleep(2000);

        Thread t2 = new Thread(delegate()
        {
            ListenDevice(tcp_client);
        });
        t2.Start();


        Thread t1 = new Thread(delegate()
        {
            SendDatatoDevice(socket, listen_response());
        });
        t1.Start();

    }




    /*
        Main method
    */
    public static void Main(string[] args)
    {

        string ip_address = "";
       
        int port_number = 0;
        //port_number = Convert.ToInt32(args[0]);

        try
        {
            ip_address = args[0];

            port_number = Convert.ToInt32("4001");
        }
        catch (Exception error)
        {
            Console.WriteLine("Error: {0}", error.ToString());
        }



        //Create a transmissor
        Transmissor transmissor = new Transmissor("configuration.txt");

        //This line is listening signals from the transceiver
        transmissor.ConnectToDevice(ip_address, port_number);

        //This line is listening responses from the server
        //transmissor.listen_response();
        

    }


}
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
        file_path (string): the path of the file to read
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
        message (byte): the message to send to

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
                while (true)
                {
                    var event_args = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    var body = event_args.Body;
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine(" [x] {0}", message);
                }
            }
        }

    }

    /*
        This method prepares the message
        message (string): the message to be encoding
    */
    private static void PrepareMessage(string message)
    {
        Transmissor transmissor = new Transmissor("configuration.txt");
        var body = Encoding.UTF8.GetBytes(message);
        transmissor.transmit(body);
    }


    /*
      This method is listening the signals from the transceiver
      tcp_client (TcpClient):the client to receive the messages from
    */

    public void ListenDevice(TcpClient tcp_client)
    {
       byte[] buffer = new byte[100];
        try
            {
                while (true)
                {
                    
                    int received = 0;

                    while (received < buffer.Length)
                    {
                        //received = socket.Receive(buffer, 0 + received, 40, SocketFlags.None);
                        received = socket.Receive(buffer);

                        if (received != 0)
                        {
                            
                            string s_received = Encoding.ASCII.GetString(buffer);

                            Console.Write("DATOS RECIBIDOS:");
                            Console.WriteLine(" >> " + s_received);

                            PrepareMessage(buffer);
                            

                        }
                     
                    }
                  

                }
               
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: {0}", error.ToString());
            
            }


    }

 
    /*
     *This method create the connection with a client
     *ip (string): the ip of the client
     *port (int): the port through which the server will connect
     */
    public void ConnectToDevice(string ip, int port)
    {
       TcpClient tcp_client;
        Socket socket;

        tcp_client = new TcpClient(ip, port);
        socket = tcp_client.Client;

        ListenDevice(socket);

    }




    /*
        Main method
    */
    public static void Main(string[] args)
    {

        int port_number = 0;
        port_number = Convert.ToInt32(args[0]);


        //Create a transmissor
        Transmissor transmissor = new Transmissor("configuration.txt");

        //This line is listening signals from the transceiver
        transmissor.ConnectToDevice("localhost", port_number);

        //This line is listening responses from the server
        transmissor.listen_response();
        

    }


}
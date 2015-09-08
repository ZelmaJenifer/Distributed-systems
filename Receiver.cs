using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitConfiguration;
using System.Threading;
using System.Data.SqlClient;

/*
    Define a Receiver class
*/
class Receiver
{

    ConnectionFactory factory;
    ConfigurationReader c_reader;
    private Conexion cnn;

    /*
     *Construct and setup
     *file_path (string): the path of the file to read
     */
    public Receiver(string file_path)
    {
        this.Setup(file_path);
        this.SetDefaultConnectionString();
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
        This method send response from this server to the caller
    */
    public void send_response()
    {
        using (var connection = this.factory.CreateConnection())
        {

            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("response", "fanout");

                var message = MessageResponse();

                channel.BasicPublish("response", "", null, message);

            }
        }
    }

    /*
        This method is receiving request from the server
    */
    public void receive()
    {
        DateTime date_ini = DateTime.Now;
        this.factory.HostName = c_reader.GetServer();

        using (var connection = this.factory.CreateConnection())
        {

            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("transmission", "fanout");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, "transmission", "");

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queueName, true, consumer);

                Console.WriteLine(" [*] Waiting for logs." +
                                  " To exit press CTRL+C");
                while ((DateTime.Now - date_ini).TotalMinutes < 2)
                {
                    var event_args = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    var body = event_args.Body;
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine(" [x] {0}", message);
                    StoreData(message);
                    //Va al método StoreData (guardar en la base de datos)

                    //this.send_response(); Sí lo recibí
                }
            }
        }

    }


    /*
     *This method sends the parameters for a connection
     */
    private void SetDefaultConnectionString()
    {
            
         try
         {
             cnn = new Conexion(@"ZELMA-PC\LASEC",
                                "sa",
                                "pa$$w0rd",
                                "LASECDB");
            }
            catch (Exception ex)
            {
 
            }

     }

     /*
      *This method stores the retrieved data in to a database
      *data (string): the data to be stored
      */
    private void StoreData(string data)
    {
        
        string builder;
       
        string[] splitRawData;

        splitRawData = data.Split(new string[] { "\r\0\n" }, StringSplitOptions.None);

        foreach (string rawData in splitRawData)
        {
            builder = rawData.Replace("??_??_??\0??_??\0", "");

            builder = builder.Replace("������\0����\05", "");
            builder = builder.Replace("��_��_��\0��_��\0", "");
            builder = builder.Replace("??☺??♥?? ??☺??", "");
            builder = builder.Replace("??☺??♥?? ??☺?? ??,♣ ????,♣♀??", "");
            builder = builder.Replace("??????", "");
            builder = builder.Replace("*RRN:", "");
            builder = builder.Replace("RRN:", "");
            builder = builder.Replace("RN:", "");
            builder = builder.Replace("N:", "");
            builder = builder.Replace(":", "");

            string[] splitRecordToInsert = builder.Split(',');

            if (splitRecordToInsert.Length == 5)
            {
                if (splitRecordToInsert[0].Length==12)
                {
                  try
                  {
                    //if (item.Lector == splitRecordToInsert[0] && listLectores.FirstOrDefault(L => L.Lector == splitRecordToInsert[1]) == null)
                    //{
                        SqlParameter[] param =
                        {
                                                new SqlParameter("@LectorId", splitRecordToInsert[0]),
                                                new SqlParameter("@SwarmId", splitRecordToInsert[1]),
                                                new SqlParameter("@Error", splitRecordToInsert[2]),
                                                new SqlParameter("@Distancia", splitRecordToInsert[3]),
                                                new SqlParameter("@Dato1", splitRecordToInsert[4]),
                                                new SqlParameter("@Bateria", "0")
                        };

                        cnn.ExecNonProcedure("addSwarmRegistros", param);
                    //}
                   }


                catch (Exception) { }
                }
            }
        }






    }




    /*
        Send message to the client
    */
    private static byte[] MessageResponse()
    {

        //Prender leds del 9BF7
        //va message = "SDAT 1 000011DB9BF7 0B 08125554025500035B0C01 2000\r\n";
        //Apagar leds del 9BF7
        //var massage = "SDAT 1 000011DB9BF7 0B 08125554025500035B0C00 2000\r\n";
        //Prende leds del B7B0 *NO PRENDEN SUS LEDS (el 9BF7 no ve al B7B0)*
        //var message = "SDAT 1 0000112DB7B0 0B 08125554025500035B0C01 2000\r\n";

        var message = "SDAT 1 000011DB9BF7 0B 08125554025500035B0C00 2000\r\n";

        var body = Encoding.UTF8.GetBytes(message);

        return (body);
    }


    //Main method
    public static void Main(String[] args)
    {
        //Create receiver
        Receiver receiver = new Receiver("configuration.txt");   

        
        //Listen
        Thread t1 = new Thread(delegate()
        {
            receiver.receive();
        });
        t1.Start();

        //Thread.Sleep(8000);

        Thread t2 = new Thread(delegate()
        {
            receiver.send_response();
        });
        //t2.Start();




    }

}
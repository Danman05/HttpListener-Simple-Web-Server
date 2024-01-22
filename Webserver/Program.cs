namespace Webserver
{
    public class Program
    {
        static void Main(string[] args)
        {
           
            Server server = new Server();
 
            Thread serverThread = new Thread(server.Start);
            serverThread.Start();
        }
    }
}
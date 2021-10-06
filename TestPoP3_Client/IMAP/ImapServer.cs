
namespace TestPoP3_Client
{


    // @author Jonathan & Damien
    public class ImapServer
    {

        protected bool FOREVER = true;
        protected int m_port;
        protected System.Net.Sockets.TcpListener m_listener;
        

        public ImapServer(int port)
        {
            this.m_port = port;
        }


        // Create a new Server on the default port : 143
        public ImapServer()
            : this(143)
        { }

        

        public void Start()
        {
            System.Collections.Generic.List<ImapConnection> servers = new System.Collections.Generic.List<ImapConnection>();

            System.Net.IPEndPoint endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, this.m_port);
            this.m_listener = new System.Net.Sockets.TcpListener(endPoint);
            this.m_listener.Start();

            while (FOREVER)
            {
                try
                {
                    System.Net.Sockets.TcpClient client = this.m_listener.AcceptTcpClient();
                    ImapConnection connection = new ImapConnection(client, 0);
                    servers.Add(connection);
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(connection.HandleConnection));
                    thread.Start();
                }
                catch (System.Net.Sockets.SocketException se)
                {
                    if (FOREVER) // Not a planned server shutdown 
                    {
                        System.Console.WriteLine(System.Environment.NewLine);
                        System.Console.WriteLine(se.Message);
                        System.Console.WriteLine(System.Environment.NewLine);
                        System.Console.WriteLine(se.StackTrace);
                        System.Console.WriteLine(System.Environment.NewLine);
                    }
                    else
                        System.Console.WriteLine("Server stopped.");
                } // End Catch System.Net.Sockets.SocketException 
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(System.Environment.NewLine);
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(System.Environment.NewLine);
                    System.Console.WriteLine(ex.StackTrace);
                    System.Console.WriteLine(System.Environment.NewLine);
                } // End Catch System.Exception 

            } // Whend 

        } // End Sub StartServer 


        public void Stop()
        {
            this.FOREVER = false;
            this.m_listener.Stop();
        }


        public static void Test()
        {
            ImapServer s = new ImapServer();
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(s.Start));
            thread.Start();
        }


    }


}

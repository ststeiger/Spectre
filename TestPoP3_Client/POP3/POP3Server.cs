
namespace TestPoP3_Client
{


    // @author Jonathan & Damien
    public class POP3Server
    {

        protected bool FOREVER = true;
        protected int m_port;
        protected System.Net.Sockets.TcpListener m_listener;
        protected System.Collections.Generic.Dictionary<string, string> users;
        protected System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> emails;
        


        public POP3Server(int port)
        {
            this.m_port = port;

            // Initialisation of users credentials and mails
            users = new System.Collections.Generic.Dictionary<string, string>();
            emails = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>();

            // Fill the users and emails
            users["john"] = "doe";
            emails["john"] = new System.Collections.Generic.List<string>();

            try
            {
                using (System.IO.StreamReader inputStream = new System.IO.StreamReader("john.txt"))
                {

                    string line;
                    string msg = "";
                    while ((line = inputStream.ReadLine()) != null)
                    {
                        msg += line + "\r\n";
                        if (line.Equals("."))
                        {
                            System.Console.Out.WriteLine(msg);
                            emails["john"].Add(msg);
                            msg = "";
                        }
                    }

                }

                //while (bis.Read(buf) != -1)
                //{
                //    emails["john"].Add("From : god \r\nSubject : " 
                //        + i.ToString(System.Globalization.CultureInfo.InvariantCulture) 
                //        + "\r\n\r\nMessage number " 
                //        + i.ToString(System.Globalization.CultureInfo.InvariantCulture) 
                //        + "\r\n.\r\n");
                //}
                
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine(e.StackTrace);
            }
        }


        // Create a new Server on the default port : 110
        public POP3Server()
            : this(110)
        { }

        

        public void Start()
        {
            System.Collections.Generic.List<POP3Connection> servers = new System.Collections.Generic.List<POP3Connection>();

            System.Net.IPEndPoint endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, this.m_port);
            this.m_listener = new System.Net.Sockets.TcpListener(endPoint);
            this.m_listener.Start();

            while (FOREVER)
            {
                try
                {
                    System.Net.Sockets.TcpClient client = this.m_listener.AcceptTcpClient();
                    POP3Connection connection = new POP3Connection(client, users, emails);
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
            POP3Server s = new POP3Server();
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(s.Start));
            thread.Start();
        }


    }


}


namespace TestPoP3_Server
{


    // https://en.wikipedia.org/wiki/Hyper_Text_Coffee_Pot_Control_Protocol
    // https://www.simform.com/blog/angular-vs-react/
    public class Program
    {


        public static void StartPop3Server()
        {
            // IMAP: 143, 993=SSL/TLS
            // POP3: 110, 995=SSL/TLS
            // SMTP:  25, 587=SSL/TLS, Navigator’s email server also supports 2525, which is another alternative to port 25.
            Spectre.Pop3.Server srv = new Spectre.Pop3.Server("127.0.0.1", 110);
            srv.Start();

            // srv.Stop();

        }


        public static void PrintCapabilities()
        {
            using (MailKit.Net.Pop3.Pop3Client client = new MailKit.Net.Pop3.Pop3Client())
            {
                client.Connect("127.0.0.1", 110, MailKit.Security.SecureSocketOptions.None);

                if (client.Capabilities.HasFlag(MailKit.Net.Pop3.Pop3Capabilities.Sasl))
                {
                    string mechanisms = string.Join(", ", client.AuthenticationMechanisms);
                    System.Console.WriteLine("The POP3 server supports the following SASL mechanisms: {0}", mechanisms);
                }

                // client.Authenticate("username", "password");

                if (client.Capabilities.HasFlag(MailKit.Net.Pop3.Pop3Capabilities.Apop))
                    System.Console.WriteLine("The server supports APOP authentication.");

                if (client.Capabilities.HasFlag(MailKit.Net.Pop3.Pop3Capabilities.Expire))
                {
                    if (client.ExpirePolicy > 0)
                        System.Console.WriteLine("The POP3 server automatically expires messages after {0} days", client.ExpirePolicy);
                    else
                        System.Console.WriteLine("The POP3 server will never expire messages.");
                }

                if (client.Capabilities.HasFlag(MailKit.Net.Pop3.Pop3Capabilities.LoginDelay))
                    System.Console.WriteLine("The minimum number of seconds between login attempts is {0}.", client.LoginDelay);

                if (client.Capabilities.HasFlag(MailKit.Net.Pop3.Pop3Capabilities.Pipelining))
                    System.Console.WriteLine("The POP3 server can pipeline commands, so using client.GetMessages() will be faster.");

                if (client.Capabilities.HasFlag(MailKit.Net.Pop3.Pop3Capabilities.Top))
                    System.Console.WriteLine("The POP3 server supports the TOP command, so it's possible to download message headers.");

                if (client.Capabilities.HasFlag(MailKit.Net.Pop3.Pop3Capabilities.UIDL))
                    System.Console.WriteLine("The POP3 server supports the UIDL command which means we can track messages by UID.");

                client.Disconnect(true);
            }
        }


        // https://stackoverflow.com/questions/5083430/removing-region
        public static void StripRegions(string fileName, System.Text.RegularExpressions.Regex re)
        {
            string input = System.IO.File.ReadAllText(fileName, System.Text.Encoding.UTF8);
            string output = re.Replace(input, "");
            System.IO.File.WriteAllText(fileName, output, System.Text.Encoding.UTF8);
        }


        public static void StripRegions(string basePath)
        {
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"(^[ \t]*\#[ \t]*(region|endregion).*)(\r)?\n", System.Text.RegularExpressions.RegexOptions.Multiline);

            foreach (string file in System.IO.Directory.GetFiles(basePath, "*.cs", System.IO.SearchOption.AllDirectories))
            {
                StripRegions(file, re);
            }
        }


        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            // StripRegions(@"D:\username\Documents\Visual Studio 2017\Projects\CoreMail");
            // StripRegions(@"D:\username\Documents\Visual Studio 2017\Projects\Spectre\Spectre");

            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(StartPop3Server));
            thread.Start();
            await System.Threading.Tasks.Task.Delay(100);

            PrintCapabilities();


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


    } // End Class Program 


} // End Namespace TestPoP3_Server 

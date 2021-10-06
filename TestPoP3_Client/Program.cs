
namespace TestPoP3_Client
{



    // https://www.dynu.com/Resources/Tutorials/DynamicDNS/CreateMXRecord
    // https://github.com/rnwood/smtpserver/tree/master/Rnwood.SmtpServer
    // https://github.com/zabszk/CSharp-SMTP-Server/tree/master/CSharp-SMTP-Server
    // https://github.com/rodrigosor/Spectre
    // https://github.com/smiley22/S22.Imap

    class Program
    {


        static void Main(string[] args)
        {
            TestPOP3.PrintCapabilities();
            TestPOP3.PrintSslConnectionInfo("smtp.gmail.com", 587);

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }


    } // End Class Program 


} // End Namespace TestPoP3_Client

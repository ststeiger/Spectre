
namespace TestPoP3_Client
{



    // https://www.dynu.com/Resources/Tutorials/DynamicDNS/CreateMXRecord
    // https://github.com/rnwood/smtpserver/tree/master/Rnwood.SmtpServer
    // https://github.com/zabszk/CSharp-SMTP-Server/tree/master/CSharp-SMTP-Server
    // https://github.com/rodrigosor/Spectre
    // https://github.com/smiley22/S22.Imap

    class Program
    {


        static async System.Threading.Tasks.Task TestPop3()
        {
            POP3Server.Test();
            await System.Threading.Tasks.Task.Delay(100);
            Pop3Test.PrintCapabilities("localhost", 110, MailKit.Security.SecureSocketOptions.None);
        }



        static async System.Threading.Tasks.Task TestImap()
        {
            ImapServer.Test();
            await System.Threading.Tasks.Task.Delay(100);
            ImapTest.Capabilities("localhost", 143, MailKit.Security.SecureSocketOptions.None);
        }


        static async System.Threading.Tasks.Task Main(string[] args)
        {
            SmtpExamples.PrintCapabilities("localhost", 25, MailKit.Security.SecureSocketOptions.None);
            await SmtpTestClient.SendEmailAsync();
            // SmtpExamples.PrintSslConnectionInfo("smtp.gmail.com", 587);
            // await TestImap();
            

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }


    } // End Class Program 


} // End Namespace TestPoP3_Client

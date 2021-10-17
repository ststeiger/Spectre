
namespace TestPoP3_Client
{

    public class MailSettings
    {

        public string Author;
        public string Email;

        public string Password;


        public MailSettings()
        {
            this.Author = "FirstName LastName";
            this.Email = ReverseGraphemeClusters("hc.tnemeganam-roc}ta{ksedecivres".Replace("}ta{", "@"));
            this.Password = "TOP_SECRET";
        }



        public static string ReverseGraphemeClusters(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length == 1)
                return s;

            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();

            System.Globalization.TextElementEnumerator enumerator = System.Globalization.StringInfo.GetTextElementEnumerator(s);
            while (enumerator.MoveNext())
            {
                ls.Add((string)enumerator.Current);
            } // Whend 

            ls.Reverse();

            return string.Join("", ls.ToArray());
        } // End Sub Test 


    }



    public class SmtpTestClient 
    {


        public static async System.Threading.Tasks.Task SendEmailAsync()
        {
            string email = MailSettings.ReverseGraphemeClusters("hc.tnemeganam-roc]ta[regiets".Replace("]ta[", "@"));
            string subject = "This is a test"; 
            string message = "Test sent at " + System.DateTime.Now.ToString("dddd' 'dd'.'MM'.'yyyy' 'HH':'mm':'ss'.'fff");


            MailSettings _settings = new MailSettings();

            MimeKit.MimeMessage emailMessage = new MimeKit.MimeMessage();

            emailMessage.From.Add(new MimeKit.MailboxAddress(_settings.Author, _settings.Email));
            emailMessage.To.Add(new MimeKit.MailboxAddress("Undisclosed Recepient", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new MimeKit.TextPart("html") { Text = message };

            try
            {
                using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Timeout = int.MaxValue;
                    client.Timeout = 5000;

                    // client.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                    // client.Authenticate(_settings.Email, _settings.Password);
                    // client.Send(emailMessage);
                    // client.Disconnect(true);

                    // await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.Auto);
                    // await client.ConnectAsync("localhost", 25, MailKit.Security.SecureSocketOptions.Auto);
                    // await client.ConnectAsync("127.0.0.1", 25, MailKit.Security.SecureSocketOptions.None);

                    // await client.ConnectAsync("daniel-steiger.ch", 25, MailKit.Security.SecureSocketOptions.None);
                    // await client.ConnectAsync("daniel-steiger.ch", 465, MailKit.Security.SecureSocketOptions.SslOnConnect); // Only if SECURE 

                    // await client.ConnectAsync("daniel-steiger.ch", 587, MailKit.Security.SecureSocketOptions.None);
                    // await client.ConnectAsync("daniel-steiger.ch", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    // await client.ConnectAsync("daniel-steiger.ch", 587, MailKit.Security.SecureSocketOptions.SslOnConnect); // Only if SECURE - doesn't work on 587
                    await client.ConnectAsync("daniel-steiger.ch", 587, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);

                    // await client.AuthenticateAsync(_settings.Email, _settings.Password);

                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (System.Exception ex) //todo add another try to send email
            {
                throw;
            }

        } // End Task SendEmailAsync 


    } // End Class TestSMPT 


} // End Namespace TestPoP3_Client


namespace TestPoP3_Client
{


    // https://github.com/jstedfast/MailKit/blob/master/Documentation/Examples/Pop3Examples.cs
    class TestPOP3
    {


        public static void DownloadMessages()
        {
            using (MailKit.Net.Pop3.Pop3Client client = new MailKit.Net.Pop3.Pop3Client(new MailKit.ProtocolLogger("pop3.log")))
            {
                client.Connect("pop.gmail.com", 995, MailKit.Security.SecureSocketOptions.SslOnConnect);

                client.Authenticate("username", "password");

                for (int i = 0; i < client.Count; i++)
                {
                    var message = client.GetMessage(i);

                    // write the message to a file
                    message.WriteTo(string.Format("{0}.msg", i));

                    // mark the message for deletion
                    client.DeleteMessage(i);
                }

                client.Disconnect(true);
            }
        }


        public static void DownloadMessages2()
        {
            using (MailKit.Net.Pop3.Pop3Client client = new MailKit.Net.Pop3.Pop3Client())
            {
                client.Connect("pop.gmail.com", 995, MailKit.Security.SecureSocketOptions.SslOnConnect);

                client.Authenticate("username", "password");

                for (int i = 0; i < client.Count; i++)
                {
                    var message = client.GetMessage(i);

                    // write the message to a file
                    message.WriteTo(string.Format("{0}.msg", i));

                    // mark the message for deletion
                    client.DeleteMessage(i);
                }

                client.Disconnect(true);
            }
        }

        public static void DownloadNewMessages(System.Collections.Generic.HashSet<string> previouslyDownloadedUids)
        {
            using (MailKit.Net.Pop3.Pop3Client client = new MailKit.Net.Pop3.Pop3Client())
            {
                client.Connect("pop.gmail.com", 995, MailKit.Security.SecureSocketOptions.SslOnConnect);

                client.Authenticate("username", "password");

                if (!client.Capabilities.HasFlag(MailKit.Net.Pop3.Pop3Capabilities.UIDL))
                    throw new System.Exception("The POP3 server does not support UIDs!");

                var uids = client.GetMessageUids();

                for (int i = 0; i < client.Count; i++)
                {
                    // check that we haven't already downloaded this message
                    // in a previous session
                    if (previouslyDownloadedUids.Contains(uids[i]))
                        continue;

                    var message = client.GetMessage(i);

                    // write the message to a file
                    message.WriteTo(string.Format("{0}.msg", uids[i]));

                    // add the message uid to our list of downloaded uids
                    previouslyDownloadedUids.Add(uids[i]);
                }

                client.Disconnect(true);
            }
        }


        public static void DownloadNewMessages2(System.Collections.Generic.HashSet<string> previouslyDownloadedUids)
        {
            using (var client = new MailKit.Net.Pop3.Pop3Client())
            {
                System.Collections.Generic.IList<string> uids = null;

                try
                {
                    client.Connect("pop.gmail.com", 995, MailKit.Security.SecureSocketOptions.SslOnConnect);
                }
                catch (MailKit.Net.Pop3.Pop3CommandException ex)
                {
                    System.Console.WriteLine("Error trying to connect: {0}", ex.Message);
                    System.Console.WriteLine("\tStatusText: {0}", ex.StatusText);
                    return;
                }
                catch (MailKit.Net.Pop3.Pop3ProtocolException ex)
                {
                    System.Console.WriteLine("Protocol error while trying to connect: {0}", ex.Message);
                    return;
                }

                try
                {
                    client.Authenticate("username", "password");
                }
                catch (MailKit.Security.AuthenticationException ex)
                {
                    System.Console.WriteLine("Invalid user name or password.");
                    return;
                }
                catch (MailKit.Net.Pop3.Pop3CommandException ex)
                {
                    System.Console.WriteLine("Error trying to authenticate: {0}", ex.Message);
                    System.Console.WriteLine("\tStatusText: {0}", ex.StatusText);
                    return;
                }
                catch (MailKit.Net.Pop3.Pop3ProtocolException ex)
                {
                    System.Console.WriteLine("Protocol error while trying to authenticate: {0}", ex.Message);
                    return;
                }

                // for the sake of this example, let's assume GMail supports the UIDL extension
                if (client.Capabilities.HasFlag(MailKit.Net.Pop3.Pop3Capabilities.UIDL))
                {
                    try
                    {
                        uids = client.GetMessageUids();
                    }
                    catch (MailKit.Net.Pop3.Pop3CommandException ex)
                    {
                        System.Console.WriteLine("Error trying to get the list of uids: {0}", ex.Message);
                        System.Console.WriteLine("\tStatusText: {0}", ex.StatusText);

                        // we'll continue on leaving uids set to null...
                    }
                    catch (MailKit.Net.Pop3.Pop3ProtocolException ex)
                    {
                        System.Console.WriteLine("Protocol error while trying to authenticate: {0}", ex.Message);

                        // Pop3ProtocolExceptions often cause the connection to drop
                        if (!client.IsConnected)
                            return;
                    }
                }

                for (int i = 0; i < client.Count; i++)
                {
                    if (uids != null && previouslyDownloadedUids.Contains(uids[i]))
                    {
                        // we must have downloaded this message in a previous session
                        continue;
                    }

                    try
                    {
                        // download the message at the specified index
                        var message = client.GetMessage(i);

                        // write the message to a file
                        if (uids != null)
                        {
                            message.WriteTo(string.Format("{0}.msg", uids[i]));

                            // keep track of our downloaded message uids so we can skip downloading them next time
                            previouslyDownloadedUids.Add(uids[i]);
                        }
                        else
                        {
                            message.WriteTo(string.Format("{0}.msg", i));
                        }
                    }
                    catch (MailKit.Net.Pop3.Pop3CommandException ex)
                    {
                        System.Console.WriteLine("Error downloading message {0}: {1}", i, ex.Message);
                        System.Console.WriteLine("\tStatusText: {0}", ex.StatusText);
                        continue;
                    }
                    catch (MailKit.Net.Pop3.Pop3ProtocolException ex)
                    {
                        System.Console.WriteLine("Protocol error while sending message {0}: {1}", i, ex.Message);
                        // most likely the connection has been dropped
                        if (!client.IsConnected)
                            break;
                    }
                }

                if (client.IsConnected)
                {
                    // if we do not disconnect cleanly, then the messages won't actually get deleted
                    client.Disconnect(true);
                }
            }
        }

        public static void PrintCapabilities()
        {
            using (MailKit.Net.Pop3.Pop3Client client = new MailKit.Net.Pop3.Pop3Client())
            {
                client.Connect("pop.gmail.com", 995, MailKit.Security.SecureSocketOptions.SslOnConnect);

                if (client.Capabilities.HasFlag(MailKit.Net.Pop3.Pop3Capabilities.Sasl))
                {
                    var mechanisms = string.Join(", ", client.AuthenticationMechanisms);
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

        public static void PrintSslConnectionInfo(string host, int port)
        {
            using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(host, port, MailKit.Security.SecureSocketOptions.Auto);

                System.Console.WriteLine($"Negotiated the following SSL options with {host}:");
                System.Console.WriteLine($"        Protocol Version: {client.SslProtocol}");
                System.Console.WriteLine($"        Cipher Algorithm: {client.SslCipherAlgorithm}");
                System.Console.WriteLine($"         Cipher Strength: {client.SslCipherStrength}");
                System.Console.WriteLine($"          Hash Algorithm: {client.SslHashAlgorithm}");
                System.Console.WriteLine($"           Hash Strength: {client.SslHashStrength}");
                System.Console.WriteLine($"  Key-Exchange Algorithm: {client.SslKeyExchangeAlgorithm}");
                System.Console.WriteLine($"   Key-Exchange Strength: {client.SslKeyExchangeStrength}");

                // Example Log:
                //
                // Negotiated the following SSL options with pop.gmail.com:
                //         Protocol Version: Tls12
                //         Cipher Algorithm: Aes128
                //          Cipher Strength: 128
                //           Hash Algorithm: Sha256
                //            Hash Strength: 0
                //   Key-Exchange Algorithm: 44550
                //    Key-Exchange Strength: 255

                client.Disconnect(true);
            }
        }


    }
}

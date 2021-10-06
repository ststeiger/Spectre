
// https://github.com/jon-artefact/POP3S-server/blob/master/src/Connexion.java
// https://github.com/jon-artefact/POP3S-server/blob/master/Warehouse/john
namespace TestPoP3_Client
{


    // @author Jonathan & Damien
    public class POP3Connection 
    {

        // protected System.Text.Encoding m_enc = System.Text.Encoding.UTF8;
        protected System.Text.Encoding m_enc = new System.Text.ASCIIEncoding();

        protected System.Net.Sockets.TcpClient m_client;
        protected System.Collections.Generic.Dictionary<string, string> m_users;
        protected System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> m_emails;
        protected System.Collections.Generic.List<bool> m_deletes;

        protected State m_state;

        protected enum State
        {
            AUTHORIZATION,
            WAITING_FOR_PASSWORD,
            TRANSACTION
        }

        public void Start()
        { }


        public POP3Connection(
              System.Net.Sockets.TcpClient client
            , System.Collections.Generic.Dictionary<string, string> users
            , System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> emails)
        {
            this.m_client = client;

            try
            {
                // socket.setSoTimeout(0);
                this.m_client.SendTimeout = 0;
                this.m_client.ReceiveTimeout = 0;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                System.Console.WriteLine("Error socket time-out : " + ex.Message);
            }

            this.m_users = users;
            this.m_emails = emails;
        }


        private bool SendMail(string msg)
        {
            try
            {
                byte[] bytes = m_enc.GetBytes(msg);

                System.Net.Sockets.NetworkStream clientStream = this.m_client.GetStream();
                clientStream.Write(bytes, 0, bytes.Length);
                clientStream.Flush();

                System.Console.WriteLine("> " + msg);
                return true;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("IOException : " + e.Message);
                return false;
            }
        }


        private bool Send(string msg)
        {
            msg += "\r\n";
            return SendMail(msg);
        }


        private bool Ok()
        {
            return Send("+OK");
        }


        private bool Ok(string msg)
        {
            return Send("+OK " + msg);
        }


        private bool Err()
        {
            return Send("-ERR Invalid");
        }


        private bool Err(string msg)
        {
            return Send("-ERR " + msg);
        }


        private string ReadLine() 
        {
            string line = "";
            int reading;

            System.Net.Sockets.NetworkStream s = this.m_client.GetStream();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(s, m_enc, false, 4096, true))
            {
                do
                {

                    reading = sr.Read();

                    if (reading == -1)
                    {
                        return null;
                    }

                    line += (char)reading;
                } while (!line.Contains("\r\n"));
            }

            return line.Replace("\r\n", "");
        }


        private void Success(string user)
        {
            this.m_deletes = new System.Collections.Generic.List<bool>();
            int total = 0;
            for (int i = 0; i < this.m_emails[user].Count; i++)
            {
                total += this.m_emails[user][i].Length;
                this.m_deletes.Add(false);
            }

            Ok(this.m_emails[user].Count + " " + total + " octets");
            this.m_state = State.TRANSACTION;
        }


        public void HandleConnection()
        {
            try
            {
                this.m_state = State.AUTHORIZATION;

                // outData = new System.IO.StreamWriter(socket.getOutputStream());

                System.Random rand = new System.Random();
                System.Guid uuid = System.Guid.NewGuid();
                string timestamp = uuid.ToString().Substring(0, rand.Next(25 - 15 + 1) + 15);
                timestamp = "<" + timestamp + ">";
                Ok("Server POP3 ready " + timestamp);

                string line;
                string user = "";
                while (true)
                {
                    line = ReadLine();
                    System.Console.WriteLine("< " + line);
                    if (line != null)
                    {
                        if (string.Equals(line, "QUIT"))
                        {
                            try
                            {
                                for (int i = this.m_emails[user].Count - 1; i >= 0; --i)
                                    if (this.m_deletes[i])
                                        this.m_emails[user].RemoveAt(i);

                                try
                                {
                                    using (System.IO.StreamWriter dos = new System.IO.StreamWriter("johnny.txt"))
                                    {
                                        foreach (string msg in this.m_emails["john"])
                                        {
                                            byte[] ba = m_enc.GetBytes(msg);
                                            // dos.BaseStream.Write(ba, 0, ba.Length);
                                            dos.Write(msg);
                                        }

                                        dos.Flush();
                                    }


                                    // Delete success
                                    Ok();
                                }
                                catch (System.Exception ignored) 
                                { }

                                System.Console.WriteLine(this.m_emails[user].Count);
                            }
                            catch (System.Exception e)
                            {
                                // Delete failed
                                Err("Delete failed");
                            }
                            break;
                        }

                        string[] lines = line.Split(' ');

                        switch (this.m_state)
                        {
                            case State.AUTHORIZATION:
                                switch (lines[0])
                                {
                                    case "APOP":
                                        if (lines.Length < 3)
                                        {
                                            Err("Bad APOP");
                                            break;
                                        }
                                        string password = this.m_users[lines[1]];

                                        try
                                        {
                                            password = timestamp + password;

                                            // MessageDigest m = MessageDigest.getInstance("MD5");
                                            // m.update(password.getBytes(), 0, password.length());
                                            // password = new BigInteger(1, m.digest()).tostring(16);
                                            using (System.Security.Cryptography.MD5 m = System.Security.Cryptography.MD5.Create())
                                            {
                                                byte[] pwBytes = m_enc.GetBytes(password);
                                                byte[] hashBytes = m.ComputeHash(pwBytes);
                                                password = HexUtilities.ByteArrayToHex(hashBytes, Capitalization.LOWERCASE);
                                            }
                                        }
                                        catch (System.Exception e)
                                        {
                                            Err("Error");
                                            break;
                                        }

                                        if (password != null && (string.Equals(lines[2], password)))
                                        {
                                            // APOP success
                                            Success(user = lines[1]);
                                        }
                                        else
                                        {
                                            Err("Bad APOP");
                                        }
                                        break;
                                    case "USER":
                                        if (lines.Length > 1 && this.m_users.ContainsKey(lines[1]))
                                        {
                                            Ok();
                                            this.m_state = State.WAITING_FOR_PASSWORD;
                                            user = lines[1];
                                        }
                                        else
                                        {
                                            Err("Bad USER");
                                        }
                                        break;
                                    default:
                                        Err();
                                        break;
                                }
                                break;
                            case State.TRANSACTION:
                                if (lines.Length == 2)
                                {
                                    switch (lines[0])
                                    {
                                        case "RETR":
                                            try
                                            {
                                                int i = int.Parse(lines[1], System.Globalization.CultureInfo.InvariantCulture) - 1;
                                                if (!this.m_deletes[i])
                                                {
                                                    SendMail("+OK " + this.m_emails[user][i].Length + " octets \r\n" + this.m_emails[user][int.Parse(lines[1]) - 1]);
                                                }
                                            }
                                            catch (System.Exception e)
                                            {
                                                Err();
                                            }
                                            break;
                                        case "DELE":
                                            try
                                            {
                                                this.m_deletes[int.Parse(lines[1], System.Globalization.CultureInfo.InvariantCulture)] = true;
                                                Ok();
                                            }
                                            catch (System.Exception e)
                                            {
                                                Err();
                                            }
                                            break;
                                        default:
                                            Err();
                                            break;
                                    }
                                }
                                else
                                {
                                    Err();
                                }
                                break;
                            case State.WAITING_FOR_PASSWORD:
                                if (lines.Length > 1 && string.Equals(this.m_users[user], lines[1]))
                                {
                                    Success(user);
                                }
                                else
                                {
                                    Err("Bad PASS");
                                }
                                break;
                        }
                    }
                    else
                    {
                        Err();
                        break;
                    }
                }

            }
            catch (System.Net.Sockets.SocketException e)
            {
                System.Console.WriteLine("Time out : " + e.Message);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error : " + ex.Message);
            }
            finally
            {
                Close(this.m_client);
            }
        }


        // Close a stream @param stream stream need to be closed
        protected void Close(object stream)
        {
            if (stream == null)
                return;

            try
            {
                if (stream is System.IO.StreamReader)
                {
                    ((System.IO.StreamReader)stream).Close();
                }
                else if (stream is System.IO.StreamWriter)
                {
                    ((System.IO.StreamWriter)stream).Close();
                }
                else if (stream is System.IO.Stream)
                {
                    ((System.IO.Stream)stream).Close();
                }
                else if (stream is System.Net.Sockets.TcpClient)
                {
                    ((System.Net.Sockets.TcpClient)stream).Close();
                }
                else
                {
                    System.Console.Error.WriteLine("Unable to close object: " + stream);
                }
            }
            catch (System.Exception e)
            {
                System.Console.Error.WriteLine("Error closing stream: " + e);
            }

        } // End Sub Close 


    } // End Class POP3Connection 


} // End Namespace 

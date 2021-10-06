
// https://github.com/jon-artefact/POP3S-server/blob/master/src/Connexion.java
// https://github.com/jon-artefact/POP3S-server/blob/master/Warehouse/john
namespace TestPoP3_Client.POP3
{


    // @author Jonathan & Damien
    class Connexion // extends Thread
    {

        protected System.Text.Encoding m_enc = System.Text.Encoding.UTF8;

        private System.Collections.Generic.Dictionary<string, string> users;
        private System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> emails;
        private System.Collections.Generic.List<bool> deletes;

        private System.Net.Sockets.Socket socket;
        private System.IO.StreamWriter outData;

        private State state;

        private enum State
        {
            AUTHORIZATION,
            WAITING_FOR_PASSWORD,
            TRANSACTION
        }

        Connexion(
              System.Net.Sockets.Socket _socket
            , System.Collections.Generic.Dictionary<string, string> _users
            , System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> _emails)
        {
            socket = _socket;
            try
            {
                // socket.setSoTimeout(0);
                socket.SendTimeout = 0;
                socket.ReceiveTimeout = 0;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                System.Console.WriteLine("Error socket time-out : " + ex.Message);
            }
            users = _users;
            emails = _emails;
        }

        private bool ok()
        {
            return send("+OK");
        }

        private bool ok(string msg)
        {
            return send("+OK " + msg);
        }

        private bool err()
        {
            return send("-ERR Invalid");
        }

        private bool err(string msg)
        {
            return send("-ERR " + msg);
        }

        private bool sendMail(string msg)
        {
            try
            {


                byte[] bytes = m_enc.GetBytes(msg);
                outData.BaseStream.Write(bytes, 0, bytes.Length);

                outData.Flush();
                System.Console.WriteLine("> " + msg);
                return true;

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("IOException : " + e.Message);
                return false;
            }
        }

        private bool send(string msg)
        {
            msg += "\r\n";
            return sendMail(msg);
        }

        private string readLine() // throws IOException
        {
            // InputStream input = socket.getInputStream();
            System.IO.StreamReader input = null;

            string line = "";
            int reading;

            do
            {

                reading = input.Read();

                if (reading == -1)
                {
                    return null;
                }

                line += (char)reading;
            } while (!line.Contains("\r\n"));
            return line.Replace("\r\n", "");
        }

        private void success(string user)
        {
            deletes = new System.Collections.Generic.List<bool>();
            int total = 0;
            for (int i = 0; i < emails[user].Count; i++)
            {
                total += emails[user][i].Length;
                deletes.Add(false);
            }

            ok(emails[user].Count + " " + total + " octets");
            state = State.TRANSACTION;
        }

        public void run()
        {
            try
            {
                state = State.AUTHORIZATION;

                // outData = new System.IO.StreamWriter(socket.getOutputStream());

                System.Random rand = new System.Random();
                System.Guid uuid = System.Guid.NewGuid();
                string timestamp = uuid.ToString().Substring(0, rand.Next(25 - 15 + 1) + 15);
                timestamp = "<" + timestamp + ">";
                ok("Server POP3 ready " + timestamp);

                string line;
                string user = "";
                while (true)
                {
                    line = readLine();
                    System.Console.WriteLine("< " + line);
                    if (line != null)
                    {
                        if (string.Equals(line, "QUIT"))
                        {
                            try
                            {
                                for (int i = emails[user].Count - 1; i >= 0; --i)
                                    if (deletes[i])
                                        emails[user].RemoveAt(i);

                                try
                                {
                                    // DataOutputStream dos = new DataOutputStream(new System.IO.StreamWriter(new FileOutputStream(new File("Warehouse/john"))));
                                    using (var dos = new System.IO.StreamWriter(""))
                                    {
                                        foreach (string msg in emails["john"])
                                        {
                                            byte[] ba = m_enc.GetBytes(msg);
                                            // dos.BaseStream.Write(ba, 0, ba.Length);
                                            dos.Write(msg);
                                        }

                                    }


                                    // Delete success
                                    ok();
                                }
                                catch (System.Exception ignored) { }

                                System.Console.WriteLine(emails[user].Count);
                            }
                            catch (System.Exception e)
                            {
                                // Delete failed
                                err("Delete failed");
                            }
                            break;
                        }

                        string[] lines = line.Split(' ');

                        switch (state)
                        {
                            case State.AUTHORIZATION:
                                switch (lines[0])
                                {
                                    case "APOP":
                                        if (lines.Length < 3)
                                        {
                                            err("Bad APOP");
                                            break;
                                        }
                                        string password = users[lines[1]];

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
                                            err("Error");
                                            break;
                                        }

                                        if (password != null && (string.Equals(lines[2], password)))
                                        {
                                            // APOP success
                                            success(user = lines[1]);
                                        }
                                        else
                                        {
                                            err("Bad APOP");
                                        }
                                        break;
                                    case "USER":
                                        if (lines.Length > 1 && users.ContainsKey(lines[1]))
                                        {
                                            ok();
                                            state = State.WAITING_FOR_PASSWORD;
                                            user = lines[1];
                                        }
                                        else
                                        {
                                            err("Bad USER");
                                        }
                                        break;
                                    default:
                                        err();
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
                                                if (!deletes[i])
                                                {
                                                    sendMail("+OK " + emails[user][i].Length + " octets \r\n" + emails[user][int.Parse(lines[1]) - 1]);
                                                }
                                            }
                                            catch (System.Exception e)
                                            {
                                                err();
                                            }
                                            break;
                                        case "DELE":
                                            try
                                            {
                                                deletes[int.Parse(lines[1], System.Globalization.CultureInfo.InvariantCulture)] = true;
                                                ok();
                                            }
                                            catch (System.Exception e)
                                            {
                                                err();
                                            }
                                            break;
                                        default:
                                            err();
                                            break;
                                    }
                                }
                                else
                                {
                                    err();
                                }
                                break;
                            case State.WAITING_FOR_PASSWORD:
                                if (lines.Length > 1 && string.Equals(users[user], lines[1]))
                                {
                                    success(user);
                                }
                                else
                                {
                                    err("Bad PASS");
                                }
                                break;
                        }
                    }
                    else
                    {
                        err();
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
                close(outData);
                close(socket);
            }
        }

        /**
         * Close a stream
         *
         * @param stream stream need to be closed
         */
        private void close(object stream)
        {
            if (stream == null)
            {
                return;
            }
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
                else if (stream is System.Net.Sockets.Socket)
                {
                    ((System.Net.Sockets.Socket)stream).Close();
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
        }
    }


}

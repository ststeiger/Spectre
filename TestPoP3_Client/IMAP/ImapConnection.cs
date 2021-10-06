
namespace TestPoP3_Client
{


    // https://github.com/bkrunic/A-simple-IMAP-server/blob/master/imapServer/src/imapServer/ServerThread.java
    public class ImapConnection
    {
        // protected System.Text.Encoding m_enc = System.Text.Encoding.UTF8;
        protected System.Text.Encoding m_enc = new System.Text.ASCIIEncoding();

        protected System.Net.Sockets.TcpClient m_client;
        protected int m_cmdId;



        public ImapConnection(System.Net.Sockets.TcpClient client, int cmdId)
        {
            this.m_client = client;
            this.m_cmdId = cmdId;
        }


        public void HandleConnection()
        {
            try
            {
                // System.IO.StreamReader inputstream = null; // new System.IO.StreamReader(new InputStreamReader(socket.getInputStream()));
                // System.IO.StreamWriter outputstream = null; // new PrintWriter(new OutputStreamWriter(socket.getOutputStream()), true);

                System.Net.Sockets.NetworkStream s = this.m_client.GetStream();

                using (System.IO.StreamWriter outputstream = new System.IO.StreamWriter(s, m_enc, 4096, true))
                {

                    // https://github.com/greysoft/mailismus/blob/master/server/src/main/java/com/grey/mailismus/imap/server/IMAP4Server.java
                    // case E_CONNECTED: transmit(shared.imap4rsp_greet, true); break;
                    const string STATUS_UNTAGGED = "* "; //prefixes untagged response
                    const string STATUS_OK = "OK";
                    const string EOL = "\r\n";

                    // static final String RSPMSG_GREET = "Mailismus IMAP Ready";
                    // String greetmsg = cfg.getValue("greet", true, Defs.RSPMSG_GREET).replace(Defs.TOKEN_HOSTNAME, task.getAppConfig().getAnnounceHost());
                    // string greetmsg = IMAP4Protocol.STATUS_UNTAGGED + IMAP4Protocol.STATUS_OK + " " + greetmsg + IMAP4Protocol.EOL;
                    // imap4rsp_greet = com.grey.mailismus.Task.constBuffer(greetmsg);
                    string greetmsg = "Mailismus IMAP Ready";
                    greetmsg = STATUS_UNTAGGED + STATUS_OK + " " + greetmsg + EOL;
                    outputstream.Write(greetmsg);
                    outputstream.Flush();

                    using (System.IO.StreamReader inputstream = new System.IO.StreamReader(s, m_enc, false, 4096, true))
                    {
                        string message = "";
                        string file = "mail.txt";
                        bool log = false; // FLAG KOJI PROVERAVA DA LI SMO ULOGAVNI

                        while (!message.StartsWith("logout"))
                        {
                            message = inputstream.ReadLine();
                            if (message.StartsWith("login"))
                            {
                                outputstream.WriteLine("cmdId"
                                    + this.m_cmdId.ToString(System.Globalization.CultureInfo.InvariantCulture)
                                    + " OK LOGIN completed"
                                );

                                log = true;
                                this.m_cmdId++; // SVAKA KOMANDA IMA SVOJ ID
                            }
                            if (message.StartsWith("select") && log)
                            {
                                outputstream.WriteLine("*" + EmailPrefix(file) + " new messages in inbox");
                                outputstream.WriteLine("cmdId"
                                    + this.m_cmdId.ToString(System.Globalization.CultureInfo.InvariantCulture)
                                    + " OK SELECT completed"
                                );

                                this.m_cmdId++;

                            }
                            if (message.StartsWith("fetch") && log)
                            { // UCITAVANJE MEJLOVA IZ FAJLA
                                string[] token = message.Split(' ');
                                int kolicina = int.Parse(token[1], System.Globalization.CultureInfo.InvariantCulture);

                                outputstream.WriteLine("* " + kolicina.ToString(System.Globalization.CultureInfo.InvariantCulture) + "fetch");
                                string poruka = LoadFromFile(file, kolicina);
                                outputstream.WriteLine(poruka);
                                outputstream.WriteLine("cmdId"
                                    + this.m_cmdId.ToString(System.Globalization.CultureInfo.InvariantCulture)
                                    + " OK FETCH completed"
                                );

                                this.m_cmdId++;

                            }
                            if (message.StartsWith("delete") && log)
                            {
                                string[] token = message.Split(' ');
                                int kolicina = int.Parse(token[1], System.Globalization.CultureInfo.InvariantCulture);
                                RemoveLineFromFile(file, kolicina);
                                outputstream.WriteLine("* " + kolicina + "deleted");
                                outputstream.WriteLine("cmdId"
                                    + this.m_cmdId.ToString(System.Globalization.CultureInfo.InvariantCulture)
                                    + " OK DELETE completed"
                                );

                                this.m_cmdId++;

                            }

                            if (!log)
                            {
                                outputstream.WriteLine("You have to login first");
                            }
                            else
                            {
                                outputstream.WriteLine("cmdId"
                                    + this.m_cmdId.ToString(System.Globalization.CultureInfo.InvariantCulture)
                                    + " BAD command (unrecognized command or command syntax error)"
                                );

                            }
                        }
                        outputstream.WriteLine("* BYE IMAP server terminating connection");
                        outputstream.WriteLine("cmdId" +
                            this.m_cmdId.ToString(System.Globalization.CultureInfo.InvariantCulture)
                            + " OK LOGOUT completed"
                        );

                    } // End Using inputStream 

                } // End Using outputstream 

                this.m_client.Close();
            }
            catch (System.Exception e)
            {
                // TODO Auto-generated catch block
                System.Console.WriteLine(e.StackTrace);
            }

        } // End Sub Run 


        // učitaj iz fajla = LoadFromFile
        private string LoadFromFile(string file, int kolicina)
        {
            string buffer = "";
            using (System.IO.StreamReader br = new System.IO.StreamReader(file))
            {
                string line;
                while ((line = br.ReadLine()) != null && kolicina > 0)
                {
                    kolicina--;
                    buffer += "\n" + line;
                }
            }
            /*
            catch (FileNotFoundException e)
            {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
            catch (IOException e)
            {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
            */
            System.Console.WriteLine(buffer);
            return buffer;

        } // End Function LoadFromFile 


        // predbroj Mailove = E-Mail-Präfix
        private int EmailPrefix(string file)
        {
            int linije = 0;
            using (System.IO.StreamReader br = new System.IO.StreamReader(file))
            {

                string line;
                while ((line = br.ReadLine()) != null)
                {

                    linije++;
                }
            }
            /*
            catch (FileNotFoundException e)
            {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
            catch (IOException e)
            {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
            */
            return linije;
        } // End Function EmailPrefix 


        public void RemoveLineFromFile(string inFile, int brisanje)
        {
            int linija = 0;
            string line = null;

            // Construct the new file that will later be renamed to the original filename.
            string tempFile = "infile.tmp";//new File(inFile.getAbsolutePath() + ".tmp");

            using (System.IO.StreamReader br = new System.IO.StreamReader(inFile))
            {
                /*
				catch (FileNotFoundException e3)
				{
					// TODO Auto-generated catch block
					e3.printStackTrace();
				}
				*/

                using (System.IO.StreamWriter pw = new System.IO.StreamWriter(tempFile))
                {
                    //PrintWriter pw = null;
                    //try
                    //{
                    //	pw = new PrintWriter(new FileWriter(tempFile));
                    //}
                    //catch (IOException e2)
                    //{
                    //	// TODO Auto-generated catch block
                    //	e2.printStackTrace();
                    //}


                    while ((line = br.ReadLine()) != null)
                    {
                        linija++;
                        // Read from the original file and write to the new
                        // unless content matches data to be removed.
                        if (brisanje < linija)
                        {
                            pw.WriteLine(line);
                            pw.Flush();
                        }
                    }

                }
            }



            // Delete the original file
            if (System.IO.File.Exists(inFile))
                System.IO.File.Delete(inFile);
            else
            {
                System.Console.WriteLine("Could not delete file");
                return;
            }

            // Rename the new file to the filename the original file had.
            if (System.IO.File.Exists(tempFile))
                System.IO.File.Move(tempFile, inFile);
            else
                System.Console.WriteLine("Could not rename file");
        }


    }


}


using System.Net.Sockets;



//package imapServer;

//import java.io.System.IO.StreamReader;
//import java.io.File;
//import java.io.FileNotFoundException;
//import java.io.FileReader;
//import java.io.FileWriter;
//import java.io.IOException;
//import java.io.InputStreamReader;
//import java.io.OutputStreamWriter;
//import java.io.PrintWriter;
//import java.net.Socket;


namespace TestPoP3_Client
{


    // https://github.com/bkrunic/A-simple-IMAP-server/blob/master/imapServer/src/imapServer/ServerThread.java
    public class JavaImapServer_ServerThread
    // implements Runnable
    {
        private Socket socket;
        private int cmdId;

        public JavaImapServer_ServerThread(Socket socket, int cmdId)
        {
            // TODO Auto-generated constructor stub
            this.socket = socket;
            this.cmdId = cmdId;
        }


        public void run()
        {
            // TODO Auto-generated method stub
            try
            {



                System.IO.StreamReader inputstream = null; // new System.IO.StreamReader(new InputStreamReader(socket.getInputStream()));
                System.IO.StreamWriter outputstream = null; // new PrintWriter(new OutputStreamWriter(socket.getOutputStream()), true);

                string message = "";
                string file = "mail.txt";
                bool log = false; // FLAG KOJI PROVERAVA DA LI SMO ULOGAVNI

                while (!message.StartsWith("logout"))
                {

                    message = inputstream.ReadLine();
                    if (message.StartsWith("login"))
                    {
                        outputstream.WriteLine("cmdId" + cmdId + " OK LOGIN completed");
                        log = true;
                        cmdId++; // SVAKA KOMANDA IMA SVOJ ID
                    }
                    if (message.StartsWith("select") && log)
                    {
                        outputstream.WriteLine("*" + EmailPrefix(file) + " new messages in inbox");
                        outputstream.WriteLine("cmdId" + cmdId + " OK SELECT completed");
                        cmdId++;

                    }
                    if (message.StartsWith("fetch") && log)
                    { // UCITAVANJE MEJLOVA IZ FAJLA
                        string[] token = message.Split(' ');
                        int kolicina = int.Parse(token[1], System.Globalization.CultureInfo.InvariantCulture);

                        outputstream.WriteLine("* " + kolicina.ToString(System.Globalization.CultureInfo.InvariantCulture) + "fetch");
                        string poruka = LoadFromFile(file, kolicina);
                        outputstream.WriteLine(poruka);
                        outputstream.WriteLine("cmdId" + cmdId + " OK FETCH completed");
                        cmdId++;

                    }
                    if (message.StartsWith("delete") && log)
                    {
                        string[] token = message.Split(' ');
                        int kolicina = int.Parse(token[1], System.Globalization.CultureInfo.InvariantCulture);
                        RemoveLineFromFile(file, kolicina);
                        outputstream.WriteLine("* " + kolicina + "deleted");
                        outputstream.WriteLine("cmdId" + cmdId + " OK DELETE completed");
                        cmdId++;

                    }

                    if (!log)
                    {
                        outputstream.WriteLine("You have to login first");
                    }
                    else
                    {
                        outputstream.WriteLine("cmdId" + cmdId + " BAD command (unrecognized command or command syntax error)");

                    }
                }
                outputstream.WriteLine("* BYE IMAP server terminating connection");
                outputstream.WriteLine("cmdId" + cmdId + " OK LOGOUT completed");
                socket.Close();

            }
            catch (System.Exception e)
            {
                // TODO Auto-generated catch block
                System.Console.WriteLine(e.StackTrace);
            }

        }



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

        }


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

        }

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
            System.IO.File.Delete(inFile);
            {
                System.Console.WriteLine("Could not delete file");
                return;
            }

            // Rename the new file to the filename the original file had.
            System.IO.File.Move(tempFile, inFile);
            // System.Console.WriteLine("Could not rename file");

        }

    }


}

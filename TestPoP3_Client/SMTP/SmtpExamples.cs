//
// SmtpExamples.cs
//
// Author: Jeffrey Stedfast <jestedfa@microsoft.com>
//
// Copyright (c) 2013-2021 .NET Foundation and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

namespace TestPoP3_Client
{


	// https://github.com/jstedfast/MailKit/tree/master/Documentation/Examples
	public static class SmtpExamples
	{


		public static void SaveToPickupDirectory(MimeKit.MimeMessage message, string pickupDirectory)
		{
			do
			{
                // Generate a random file name to save the message to.
                string path = System.IO.Path.Combine(pickupDirectory, System.Guid.NewGuid().ToString() + ".eml");
				System.IO.Stream stream;

				try
				{
					// Attempt to create the new file.
					stream = System.IO.File.Open(path, System.IO.FileMode.CreateNew);
				}
				catch (System.IO.IOException)
				{
					// If the file already exists, try again with a new Guid.
					if (System.IO.File.Exists(path))
						continue;

					// Otherwise, fail immediately since it probably means that there is
					// no graceful way to recover from this error.
					throw;
				}

				try
				{
					using (stream)
					{
						// IIS pickup directories expect the message to be "byte-stuffed"
						// which means that lines beginning with "." need to be escaped
						// by adding an extra "." to the beginning of the line.
						//
						// Use an SmtpDataFilter "byte-stuff" the message as it is written
						// to the file stream. This is the same process that an SmtpClient
						// would use when sending the message in a `DATA` command.
						using (MimeKit.IO.FilteredStream filtered = new MimeKit.IO.FilteredStream(stream))
						{
							filtered.Add(new MailKit.Net.Smtp.SmtpDataFilter());

                            // Make sure to write the message in DOS (<CR><LF>) format.
                            MimeKit.FormatOptions options = MimeKit.FormatOptions.Default.Clone();
							options.NewLineFormat = MimeKit.NewLineFormat.Dos;

							message.WriteTo(options, filtered);
							filtered.Flush();
							return;
						}
					}
				}
				catch
				{
					// An exception here probably means that the disk is full.
					//
					// Delete the file that was created above so that incomplete files are not
					// left behind for IIS to send accidentally.
					System.IO.File.Delete(path);
					throw;
				}
			} while (true);
		}


	
		public static void SendMessage(MimeKit.MimeMessage message)
		{
			using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient(
				new MailKit.ProtocolLogger("smtp.log")
				))
			{
				client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

				client.Authenticate("username", "password");

				client.Send(message);

				client.Disconnect(true);
			}

			// Example log:
			//
			// Connected to smtps://smtp.gmail.com:465/
			// S: 220 smtp.gmail.com ESMTP w81sm22057166qkg.43 - gsmtp
			// C: EHLO [192.168.1.220]
			// S: 250-smtp.gmail.com at your service, [192.168.1.220]
			// S: 250-SIZE 35882577
			// S: 250-8BITMIME
			// S: 250-AUTH LOGIN PLAIN XOAUTH2 PLAIN-CLIENTTOKEN OAUTHBEARER XOAUTH
			// S: 250-ENHANCEDSTATUSCODES
			// S: 250-PIPELINING
			// S: 250-CHUNKING
			// S: 250 SMTPUTF8
			// C: AUTH PLAIN AHVzZXJuYW1lAHBhc3N3b3Jk
			// S: 235 2.7.0 Accepted
			// C: MAIL FROM:<from.addr@gmail.com>
			// C: RCPT TO:<to.addr@gmail.com>
			// S: 250 2.1.0 OK w81sm22057166qkg.43 - gsmtp
			// S: 250 2.1.5 OK w81sm22057166qkg.43 - gsmtp
			// C: DATA
			// S: 354  Go ahead w81sm22057166qkg.43 - gsmtp
			// C: From: "LastName, FirstName" <from.addr@gmail.com>
			// C: Date: Thu, 27 Dec 2018 10:55:18 -0500
			// C: Subject: This is a test message
			// C: Message-Id: <C7GVXWE3C6U4.7ZQ0K9OUHTDP1@MADUNLA-SP4.northamerica.corp.microsoft.com>
			// C: To: "LastName, FirstName" <to.addr@gmail.com>
			// C: MIME-Version: 1.0
			// C: Content-Type: multipart/alternative; boundary="=-CToJI+AD2gS6z+fFlzDvhg=="
			// C: 
			// C: --=-CToJI+AD2gS6z+fFlzDvhg==
			// C: Content-Type: text/plain; charset=utf-8
			// C: Content-Transfer-Encoding: quoted-printable
			// C: 
			// C: This is the text/plain message body.
			// C: --=-CToJI+AD2gS6z+fFlzDvhg==
			// C: Content-Type: text/html; charset=utf-8
			// C: Content-Transfer-Encoding: quoted-printable
			// C: 
			// C: <html><body><center>This is the <b>text/html</b> message body.</center></body></html>
			// C: --=-CToJI+AD2gS6z+fFlzDvhg==--
			// C: 
			// C: .
			// S: 250 2.0.0 OK 1545926120 w81sm22057166qkg.43 - gsmtp
			// C: QUIT
			// S: 221 2.0.0 closing connection w81sm22057166qkg.43 - gsmtp
		}


		public static void PrintCapabilities()
		{
			using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
			{
				client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

				if (client.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.Authentication))
				{
                    string mechanisms = string.Join(", ", client.AuthenticationMechanisms);
					System.Console.WriteLine("The SMTP server supports the following SASL mechanisms: {0}", mechanisms);
					client.Authenticate("username", "password");
				}

				if (client.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.Size))
					System.Console.WriteLine("The SMTP server has a size restriction on messages: {0}.", client.MaxSize);

				if (client.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.Dsn))
					System.Console.WriteLine("The SMTP server supports delivery-status notifications.");

				if (client.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.EightBitMime))
					System.Console.WriteLine("The SMTP server supports Content-Transfer-Encoding: 8bit");

				if (client.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.BinaryMime))
					System.Console.WriteLine("The SMTP server supports Content-Transfer-Encoding: binary");

				if (client.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.UTF8))
					System.Console.WriteLine("The SMTP server supports UTF-8 in message headers.");

				client.Disconnect(true);
			}
		}


		public static void SendMessage1(MimeKit.MimeMessage message)
		{
			using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
			{
				try
				{
					client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
				}
				catch (MailKit.Net.Smtp.SmtpCommandException ex)
				{
					System.Console.WriteLine("Error trying to connect: {0}", ex.Message);
					System.Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);
					return;
				}
				catch (MailKit.Net.Smtp.SmtpProtocolException ex)
				{
					System.Console.WriteLine("Protocol error while trying to connect: {0}", ex.Message);
					return;
				}

				// Note: Not all SMTP servers support authentication, but GMail does.
				if (client.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.Authentication))
				{
					try
					{
						client.Authenticate("username", "password");
					}
					catch (MailKit.Security.AuthenticationException ex)
					{
						System.Console.WriteLine("Invalid user name or password.");
						return;
					}
					catch (MailKit.Net.Smtp.SmtpCommandException ex)
					{
						System.Console.WriteLine("Error trying to authenticate: {0}", ex.Message);
						System.Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);
						return;
					}
					catch (MailKit.Net.Smtp.SmtpProtocolException ex)
					{
						System.Console.WriteLine("Protocol error while trying to authenticate: {0}", ex.Message);
						return;
					}
				}

				try
				{
					client.Send(message);
				}
				catch (MailKit.Net.Smtp.SmtpCommandException ex)
				{
					System.Console.WriteLine("Error sending message: {0}", ex.Message);
					System.Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);

					switch (ex.ErrorCode)
					{
						case MailKit.Net.Smtp.SmtpErrorCode.RecipientNotAccepted:
							System.Console.WriteLine("\tRecipient not accepted: {0}", ex.Mailbox);
							break;
						case MailKit.Net.Smtp.SmtpErrorCode.SenderNotAccepted:
							System.Console.WriteLine("\tSender not accepted: {0}", ex.Mailbox);
							break;
						case MailKit.Net.Smtp.SmtpErrorCode.MessageNotAccepted:
							System.Console.WriteLine("\tMessage not accepted.");
							break;
					}
				}
				catch (MailKit.Net.Smtp.SmtpProtocolException ex)
				{
					System.Console.WriteLine("Protocol error while sending message: {0}", ex.Message);
				}

				client.Disconnect(true);
			}
		}


		public static void SendMessage2(MimeKit.MimeMessage message)
		{
			using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
			{
				client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

				client.Authenticate("username", "password");

				client.Send(message);

				client.Disconnect(true);
			}
		}


		public static void SendMessage3(MimeKit.MimeMessage message)
		{
			using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
			{
				// Note: since GMail requires SSL at connection time, use the "smtps"
				// protocol instead of "smtp".
				System.Uri uri = new System.Uri("smtps://smtp.gmail.com:465");

				client.Connect(uri);

				client.Authenticate("username", "password");

				client.Send(message);

				client.Disconnect(true);
			}
		}


		public static void SendMessageWithOptions(MimeKit.MimeMessage message)
		{
			using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
			{
				client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

				client.Authenticate("username", "password");

				MimeKit.FormatOptions options = MimeKit.FormatOptions.Default.Clone();

				if (client.Capabilities.HasFlag(MailKit.Net.Smtp.SmtpCapabilities.UTF8))
					options.International = true;

				client.Send(options, message);

				client.Disconnect(true);
			}
		}


		public static void SendMessages(System.Collections.Generic.IList<MimeKit.MimeMessage> messages)
		{
			using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
			{
				client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

				client.Authenticate("username", "password");

				foreach (MimeKit.MimeMessage message in messages)
				{
					client.Send(message);
				}

				client.Disconnect(true);
			}
		}


		public class DSNSmtpClient 
			: MailKit.Net.Smtp.SmtpClient
		{
			public DSNSmtpClient()
			{
			}

			/// <summary>
			/// Get the envelope identifier to be used with delivery status notifications.
			/// </summary>
			/// <remarks>
			/// <para>The envelope identifier, if non-empty, is useful in determining which message
			/// a delivery status notification was issued for.</para>
			/// <para>The envelope identifier should be unique and may be up to 100 characters in
			/// length, but must consist only of printable ASCII characters and no white space.</para>
			/// <para>For more information, see rfc3461, section 4.4.</para>
			/// </remarks>
			/// <returns>The envelope identifier.</returns>
			/// <param name="message">The message.</param>
			protected override string GetEnvelopeId(MimeKit.MimeMessage message)
			{
				// Since you will want to be able to map whatever identifier you return here to the
				// message, the obvious identifier to use is probably the Message-Id value.
				return message.MessageId;
			}

			/// <summary>
			/// Get the types of delivery status notification desired for the specified recipient mailbox.
			/// </summary>
			/// <remarks>
			/// Gets the types of delivery status notification desired for the specified recipient mailbox.
			/// </remarks>
			/// <returns>The desired delivery status notification type.</returns>
			/// <param name="message">The message being sent.</param>
			/// <param name="mailbox">The mailbox.</param>
			protected override MailKit.DeliveryStatusNotification? GetDeliveryStatusNotifications(
				MimeKit.MimeMessage message, 
				MimeKit.MailboxAddress mailbox)
			{
				// In this example, we only want to be notified of failures to deliver to a mailbox.
				// If you also want to be notified of delays or successful deliveries, simply bitwise-or
				// whatever combination of flags you want to be notified about.
				return MailKit.DeliveryStatusNotification.Failure;
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
				// Negotiated the following SSL options with smtp.gmail.com:
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
//
// ImapExamples.cs
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


	public static class ImapTest
	{


		public static void DownloadMessages()
		{
			using (MailKit.Net.Imap.ImapClient client = new MailKit.Net.Imap.ImapClient(
				new MailKit.ProtocolLogger("imap.log")
				))
			{
				client.Connect("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);

				client.Authenticate("username", "password");

				client.Inbox.Open(MailKit.FolderAccess.ReadOnly);

                System.Collections.Generic.IList<MailKit.UniqueId> uids = client.Inbox.Search(MailKit.Search.SearchQuery.All);

				foreach (MailKit.UniqueId uid in uids)
				{
					MimeKit.MimeMessage message = client.Inbox.GetMessage(uid);

					// write the message to a file
					message.WriteTo(string.Format("{0}.eml", uid));
				}

				client.Disconnect(true);
			}
		}


		public static void Capabilities()
		{ 
			string imapServer = "imap.gmail.com";
			int port = 993;
			Capabilities(imapServer, port, MailKit.Security.SecureSocketOptions.SslOnConnect);
		}


		public static void Capabilities(string server, int port, MailKit.Security.SecureSocketOptions options)
		{
			using (MailKit.Net.Imap.ImapClient client = new MailKit.Net.Imap.ImapClient())
			{
				client.Connect(server, port, options);

                string mechanisms = string.Join(", ", client.AuthenticationMechanisms);
				System.Console.WriteLine("The IMAP server supports the following SASL authentication mechanisms: {0}", mechanisms);

				client.Authenticate("username", "password");

				if (client.Capabilities.HasFlag(MailKit.Net.Imap.ImapCapabilities.Id))
				{
					MailKit.Net.Imap.ImapImplementation clientImplementation = new MailKit.Net.Imap.ImapImplementation { Name = "MailKit", Version = "1.0" };
					MailKit.Net.Imap.ImapImplementation serverImplementation = client.Identify(clientImplementation);

					System.Console.WriteLine("Server implementation details:");
					foreach (System.Collections.Generic.KeyValuePair<string, string> property in serverImplementation.Properties)
						System.Console.WriteLine("  {0} = {1}", property.Key, property.Value);
				}

				if (client.Capabilities.HasFlag(MailKit.Net.Imap.ImapCapabilities.Acl))
				{
					System.Console.WriteLine("The IMAP server supports Access Control Lists.");

					System.Console.WriteLine("The IMAP server supports the following access rights: {0}", client.Rights);

					System.Console.WriteLine("The Inbox has the following access controls:");
					MailKit.AccessControlList acl = client.Inbox.GetAccessControlList();
					foreach (MailKit.AccessControl ac in acl)
						System.Console.WriteLine("  {0} = {1}", ac.Name, ac.Rights);

					MailKit.AccessRights myRights = client.Inbox.GetMyAccessRights();
					System.Console.WriteLine("Your current rights for the Inbox folder are: {0}", myRights);
				}

				if (client.Capabilities.HasFlag(MailKit.Net.Imap.ImapCapabilities.Quota))
				{
					System.Console.WriteLine("The IMAP server supports quotas.");

					System.Console.WriteLine("The current quota for the Inbox is:");
					MailKit.FolderQuota quota = client.Inbox.GetQuota();

					if (quota.StorageLimit.HasValue && quota.StorageLimit.Value > 0)
						System.Console.WriteLine("  Limited by storage space. Using {0} out of {1} bytes.", quota.CurrentStorageSize.Value, quota.StorageLimit.Value);

					if (quota.MessageLimit.HasValue && quota.MessageLimit.Value > 0)
						System.Console.WriteLine("  Limited by the number of messages. Using {0} out of {1} bytes.", quota.CurrentMessageCount.Value, quota.MessageLimit.Value);

					System.Console.WriteLine("The quota root is: {0}", quota.QuotaRoot);
				}

				if (client.Capabilities.HasFlag(MailKit.Net.Imap.ImapCapabilities.Thread))
				{
					if (client.ThreadingAlgorithms.Contains(MailKit.ThreadingAlgorithm.OrderedSubject))
						System.Console.WriteLine("The IMAP server supports threading by subject.");
					if (client.ThreadingAlgorithms.Contains(MailKit.ThreadingAlgorithm.References))
						System.Console.WriteLine("The IMAP server supports threading by references.");
				}

				client.Disconnect(true);
			}
		}


		public static void DownloadMessages2()
		{
			using (MailKit.Net.Imap.ImapClient client = new MailKit.Net.Imap.ImapClient())
			{
				client.Connect("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);

				client.Authenticate("username", "password");

				client.Inbox.Open(MailKit.FolderAccess.ReadOnly);

                System.Collections.Generic.IList<MailKit.UniqueId> uids = client.Inbox.Search(MailKit.Search.SearchQuery.All);

				foreach (MailKit.UniqueId uid in uids)
				{
					MimeKit.MimeMessage message = client.Inbox.GetMessage(uid);

					// write the message to a file
					message.WriteTo(string.Format("{0}.eml", uid));
				}

				client.Disconnect(true);
			}
		}


		public static void DownloadBodyParts(string baseDirectory)
		{
			using (MailKit.Net.Imap.ImapClient client = new MailKit.Net.Imap.ImapClient())
			{
				client.Connect("imap.gmail.com", 993, MailKit.Security.SecureSocketOptions.SslOnConnect);

				client.Authenticate("username", "password");

				client.Inbox.Open(MailKit.FolderAccess.ReadOnly);

				// search for messages where the Subject header contains either "MimeKit" or "MailKit"
				MailKit.Search.BinarySearchQuery query = MailKit.Search.SearchQuery.SubjectContains("MimeKit")
					.Or(MailKit.Search.SearchQuery.SubjectContains("MailKit"));

                System.Collections.Generic.IList<MailKit.UniqueId> uids = client.Inbox.Search(query);

                // fetch summary information for the search results (we will want the UID and the BODYSTRUCTURE
                // of each message so that we can extract the text body and the attachments)
                System.Collections.Generic.IList<MailKit.IMessageSummary> items = 
					client.Inbox.Fetch(uids, MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.BodyStructure);

				foreach (MailKit.IMessageSummary item in items)
				{
                    // determine a directory to save stuff in
                    string directory = System.IO.Path.Combine(baseDirectory, item.UniqueId.ToString());

					// create the directory
					System.IO.Directory.CreateDirectory(directory);

					// IMessageSummary.TextBody is a convenience property that finds the 'text/plain' body part for us
					MailKit.BodyPartText bodyPart = item.TextBody;

					// download the 'text/plain' body part
					MimeKit.TextPart body = (MimeKit.TextPart)client.Inbox.GetBodyPart(item.UniqueId, bodyPart);

                    // TextPart.Text is a convenience property that decodes the content and converts the result to
                    // a string for us
                    string text = body.Text;

					System.IO.File.WriteAllText(System.IO.Path.Combine(directory, "body.txt"), text);

					// now iterate over all of the attachments and save them to disk
					foreach (MailKit.BodyPartBasic attachment in item.Attachments)
					{
						// download the attachment just like we did with the body
						MimeKit.MimeEntity entity = client.Inbox.GetBodyPart(item.UniqueId, attachment);

						// attachments can be either message/rfc822 parts or regular MIME parts
						if (entity is MimeKit.MessagePart)
						{
							MimeKit.MessagePart rfc822 = (MimeKit.MessagePart)entity;

                            string path = System.IO.Path.Combine(directory, attachment.PartSpecifier + ".eml");

							rfc822.Message.WriteTo(path);
						}
						else
						{
							MimeKit.MimePart part = (MimeKit.MimePart)entity;

                            // note: it's possible for this to be null, but most will specify a filename
                            string fileName = part.FileName;

                            string path = System.IO.Path.Combine(directory, fileName);

							// decode and save the content to a file
							using (System.IO.FileStream stream = System.IO.File.Create(path))
								part.Content.DecodeTo(stream);
						}
					}
				}

				client.Disconnect(true);
			}
		}


		public static void PrintSslConnectionInfo(string host, int port)
		{
			using (MailKit.Net.Imap.ImapClient client = new MailKit.Net.Imap.ImapClient())
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
				// Negotiated the following SSL options with imap.gmail.com:
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
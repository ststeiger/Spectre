
namespace com.grey.mailismus.imap
{
	// Copyright 2013 Yusef Badri - All rights reserved.
	// Mailismus is distributed under the terms of the GNU Affero General Public License, Version 3 (AGPLv3).

	using com.grey.@base.utils;


	// IMAP2: RFC-1064 (Jul 1988) => RFC-1176 (Aug 1990)
	// IMAP3: RFC-1203 (Feb 1991) - never adopted, withdrawn within 2 years
	// IMAP4 is RFC-1730 (Dec 1994) => RFC-2060 (Dec 1996) => RFC-3501 (Rev1, Mar 2003)
	// IMAP4 descends from IMAP2 and was initially known as IMAP2bis.
	// RFC-1731 (Dec 1994) defines IMAP SASL profile and hasn't been updated, but 3501 doesn't refer to it.
	// Useful reference: http://www.networksorcery.com/enp/protocol/imap.htm
	public class IMAP4Protocol
	{
		public static readonly int TCP_PORT = 143;
		public static readonly int TCP_SSLPORT = 993;
		public static readonly string VERSION = "IMAP4rev1";
		public static readonly string STATUS_OK = "OK";
		public static readonly string STATUS_REJ = "NO";
		public static readonly string STATUS_ERR = "BAD";
		public static readonly string STATUS_BYE = "BYE";
		public static readonly string STATUS_PREAUTH = "PREAUTH";
		public static readonly string STATUS_UNTAGGED = "* "; // prefixes untagged response
		public static readonly string STATUS_CONTD = "+ "; // prefixes continuation response
		public static readonly char AUTH_EMPTY = '='; // denotes zero-length initial SASL response
		public static readonly string EOL = "\r\n";
		public static readonly string ENDRSP = "." + EOL;
		public static readonly char WILDCARD_PARTIAL = '%';
		public static readonly char WILDCARD_FULL = '*';
		public static readonly char LITERALPLUS = '+'; //see RFC-2088

		public enum AUTHTYPE { LOGIN, SASL_PLAIN, SASL_CRAM_MD5, SASL_EXTERNAL }

		// mandatory IMAP commands (RFC-3501)
		public static readonly ByteChars CMDREQ_LOGIN = new ByteChars("LOGIN");
		public static readonly ByteChars CMDREQ_AUTH = new ByteChars("AUTHENTICATE");
		public static readonly ByteChars CMDREQ_STLS = new ByteChars("STARTTLS");
		public static readonly ByteChars CMDREQ_QUIT = new ByteChars("LOGOUT");
		public static readonly ByteChars CMDREQ_CAPA = new ByteChars("CAPABILITY");
		public static readonly ByteChars CMDREQ_NOOP = new ByteChars("NOOP");
		public static readonly ByteChars CMDREQ_LIST = new ByteChars("LIST");
		public static readonly ByteChars CMDREQ_LSUB = new ByteChars("LSUB");
		public static readonly ByteChars CMDREQ_CREATE = new ByteChars("CREATE");
		public static readonly ByteChars CMDREQ_DELETE = new ByteChars("DELETE");
		public static readonly ByteChars CMDREQ_RENAME = new ByteChars("RENAME");
		public static readonly ByteChars CMDREQ_SELECT = new ByteChars("SELECT");
		public static readonly ByteChars CMDREQ_EXAMINE = new ByteChars("EXAMINE");
		public static readonly ByteChars CMDREQ_STATUS = new ByteChars("STATUS");
		public static readonly ByteChars CMDREQ_APPEND = new ByteChars("APPEND");
		public static readonly ByteChars CMDREQ_CLOSE = new ByteChars("CLOSE");
		public static readonly ByteChars CMDREQ_EXPUNGE = new ByteChars("EXPUNGE");
		public static readonly ByteChars CMDREQ_UID = new ByteChars("UID");
		public static readonly ByteChars CMDREQ_STORE = new ByteChars("STORE");
		public static readonly ByteChars CMDREQ_FETCH = new ByteChars("FETCH");
		public static readonly ByteChars CMDREQ_SRCH = new ByteChars("SEARCH");
		public static readonly ByteChars CMDREQ_COPY = new ByteChars("COPY");
		public static readonly ByteChars CMDREQ_CHECK = new ByteChars("CHECK");
		public static readonly ByteChars CMDREQ_SUBSCRIBE = new ByteChars("SUBSCRIBE");
		public static readonly ByteChars CMDREQ_UNSUBSCRIBE = new ByteChars("UNSUBSCRIBE");
		// optional IMAP commands (extensions)
		public static readonly ByteChars CMDREQ_IDLE = new ByteChars("IDLE"); // RFC-2177 (Jun 1997)
		public static readonly ByteChars CMDREQ_NAMSPC = new ByteChars("NAMESPACE"); // RFC-2342 (May 1998)
		public static readonly ByteChars CMDREQ_UNSELECT = new ByteChars("UNSELECT"); // RFC-3691 (Feb 2004)

		//mailbox flags
		public static readonly string BOXFLAG_NOSELECT = "\\Noselect";
		public static readonly string BOXFLAG_MARKED = "\\Marked";
		public static readonly string BOXFLAG_UNMARKED = "\\Unmarked";
		public static readonly string BOXFLAG_NEVERCHILD = "\\Noinferiors";
		public static readonly string BOXFLAG_CHILD = "\\HasChildren"; //RFC-3348 extension
		public static readonly string BOXFLAG_NOCHILD = "\\HasNoChildren";

		//message flags
		public static readonly string MSGFLAG_DRAFT = "\\Draft"; // Maildir flag: D
		public static readonly string MSGFLAG_FLAGGED = "\\Flagged"; // Maildir flag: F
		public static readonly string MSGFLAG_ANSWERED = "\\Answered";  // Maildir flag: R
		public static readonly string MSGFLAG_SEEN = "\\Seen"; // Maildir flag: S
		public static readonly string MSGFLAG_DEL = "\\Deleted"; // Maildir flag: T
		public static readonly string MSGFLAG_RECENT = "\\Recent";

		public static readonly ByteChars CAPA_TAG_CHILDREN = new ByteChars("CHILDREN"); // RFC-3348 extension
		public static readonly ByteChars CAPA_TAG_LITERALPLUS = new ByteChars("LITERAL+"); // RFC-2088 extension

		public static readonly string MBXNAME_INBOX = "INBOX";

		public static readonly string IDLE_DONE = "DONE";
	}
}

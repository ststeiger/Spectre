
namespace com.grey.mailismus.pop3
{


	// Copyright 2012-2013 Yusef Badri - All rights reserved.
	// Mailismus is distributed under the terms of the GNU Affero General Public License, Version 3 (AGPLv3).

	// import com.grey.base.utils.ByteChars;
	// import com.grey.base.sasl.SaslEntity;
	using com.grey.@base.utils;
	using com.grey.@base.sasl;


	// POP3 is RFC-1081 (Nov 1988) => 1939 (May 1996)
	// POP3-AUTH is RFC-1734 (Dec 1994) => RFC-2449 (Nov 1998) => RFC-5034 (Jul 2007) - defines SASL profile for POP3.
	// POP3 STLS extension is defined in RFC-2595 (Jun 1999)
	// Note that for the purposes of the initial client response, the 255-octet limit on the length of a single command
	// still applies, as defined in Section 4 of RFC-2449.
	public class POP3Protocol
	{
		public static readonly int TCP_PORT = 110;
		public static readonly int TCP_SSLPORT = 995;
		public static readonly string STATUS_OK = "+OK";
		public static readonly string STATUS_ERR = "-ERR";
		public static readonly string STATUS_AUTH = "+ "; //prefixes server challenge - see RFC-5034 section 4
		public static readonly char AUTH_EMPTY = '='; //denotes zero-length initial response - see RFC-5034 section 4
		public static readonly string EOL = "\r\n";
		public static readonly string ENDRSP = "." + EOL;

		public enum AUTHTYPE { USERPASS, APOP, SASL_PLAIN, SASL_CRAM_MD5, SASL_EXTERNAL }

		// mandatory POP3 commands (RFC-1939)

		public static readonly ByteChars CMDREQ_QUIT = new ByteChars("QUIT");
		public static readonly ByteChars CMDREQ_NOOP = new ByteChars("NOOP");
		public static readonly ByteChars CMDREQ_RSET = new ByteChars("RSET");
		public static readonly ByteChars CMDREQ_STAT = new ByteChars("STAT");
		public static readonly ByteChars CMDREQ_LIST = new ByteChars("LIST");
		public static readonly ByteChars CMDREQ_RETR = new ByteChars("RETR");
		public static readonly ByteChars CMDREQ_DELE = new ByteChars("DELE");
		// optional core POP3 commands (RFC-1939)
		public static readonly ByteChars CMDREQ_USER = new ByteChars("USER");
		public static readonly ByteChars CMDREQ_PASS = new ByteChars("PASS");
		public static readonly ByteChars CMDREQ_APOP = new ByteChars("APOP");
		public static readonly ByteChars CMDREQ_TOP = new ByteChars("TOP");
		public static readonly ByteChars CMDREQ_UIDL = new ByteChars("UIDL");
		// optional commands from RFC-5034 and RFC-2595
		public static readonly ByteChars CMDREQ_CAPA = new ByteChars("CAPA");
		private static readonly ByteChars CMDREQ_AUTH = new ByteChars("AUTH ");
		public static readonly ByteChars CMDREQ_SASL_PLAIN = new ByteChars(CMDREQ_AUTH).append(SaslEntity.MECHNAME_PLAIN);
		public static readonly ByteChars CMDREQ_SASL_CMD5 = new ByteChars(CMDREQ_AUTH).append(SaslEntity.MECHNAME_CMD5);
		public static readonly ByteChars CMDREQ_SASL_EXTERNAL = new ByteChars(CMDREQ_AUTH).append(SaslEntity.MECHNAME_EXTERNAL);
		public static readonly ByteChars CMDREQ_STLS = new ByteChars("STLS");

		// CAPA tags from RFC-2449 (other than the tags that correspond to POP commands)
		public static readonly ByteChars CAPA_TAG_PIPE = new ByteChars("PIPELINING");
		public static readonly ByteChars CAPA_TAG_EXPIRE = new ByteChars("EXPIRE");
		public static readonly ByteChars CAPA_TAG_IMPL = new ByteChars("IMPLEMENTATION");
	}


}
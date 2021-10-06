
namespace com.grey.mailismus.imap.server
{

	// Copyright 2013 Yusef Badri - All rights reserved.
	// Mailismus is distributed under the terms of the GNU Affero General Public License, Version 3 (AGPLv3).

	// https://github.com/greysoft/mailismus/blob/master/server/src/main/java/com/grey/mailismus/imap/server/Defs.java
	public static class Defs
	{
		static readonly string TOKEN_HOSTNAME = "%H%";
		static readonly string RSPMSG_GREET = "Mailismus IMAP Ready";

		static readonly string DATA_NIL = "NIL";
		static readonly char CHAR_QUOTE = '"';
		// static readonly string PARENTH_QUOTE = string.valueOf((char) CHAR_QUOTE)+string.valueOf((char) CHAR_QUOTE);
		static readonly string PARENTH_QUOTE = CHAR_QUOTE.ToString(System.Globalization.CultureInfo.InvariantCulture) + CHAR_QUOTE.ToString(System.Globalization.CultureInfo.InvariantCulture);


		// PROTO_STATE maps to the RFC-3501 protocol states as follows:
		// - S_AUTH: The Not-Authenticated state (we are in the process of authenticating)
		// - S_SELECT: The Authenticated state (we are in the process of selecting a mailbox)
		// - S_MAILBOX: The Selected state (we have selected a mailbox)
		// - S_DISCON: The Logout state
		// The other PROTO_STATE values can be seen as sub-states of the RFC ones, eg. S_STLS and S_SASL
		// occur during authentication.
		enum PROTO_STATE { S_DISCON, S_AUTH, S_SASL, S_STLS, S_SELECT, S_MAILBOX, S_APPEND, S_IDLE }

		enum PROTO_EVENT
		{
			E_CONNECTED, E_DISCONNECT, E_DISCONNECTED, E_QUIT, E_STLS, E_CAPA,
			E_LOGIN, E_AUTHSASL, E_SASLRSP,
			E_LIST, E_NAMSPC, E_SELECT, E_EXAMINE, E_STATUS, E_CREATE, E_DELETE, E_RENAME, E_APPEND,
			E_CLOSE, E_EXPUNGE, E_STORE, E_FETCH, E_SRCH, E_COPY, E_UID, E_CHECK,
			E_LSUB, E_SUBSCRIBE, E_UNSUBSCRIBE, E_UNSELECT,
			E_NOOP, E_REJCMD, E_BADCMD, E_LOCALERROR, E_IDLE
		}

		static bool isFlagSet(int f, int t) 
		{ 
			return ((f & t) != 0); 
		}


		class EnvelopeHeader
		{
			public static readonly int F_ADDR = 1 << 0; //this field is an email address
			public static readonly int F_ESC = 1 << 1; //value might need to be escaped
			public static readonly int F_ANGBRACE = 1 << 2; //take angle-bracketed part only

			public readonly string hdrname;
			public readonly string dflt; //another header name, to whose value this one defaults
			private readonly int flags;

			public EnvelopeHeader(string n, string d, int f)
			{
				hdrname = n; dflt = d; flags = f;
			}

			public EnvelopeHeader(string n) 
				: this(n, null, 0) 
			{  }
			public EnvelopeHeader(string n, string d)
				: this(n, d, 0)
			{  }
			public EnvelopeHeader(string n, int f) 
				: this(n, null, f)
			{  }

			public bool trait(int f) 
			{ 
				return Defs.isFlagSet(flags, f); 
			}
		}


		class FetchOpDef
		{
			public enum OPCODE { DUMMY, UID, SIZE, TIMESTAMP, FLAGS, ENVELOPE, BODYSTRUCTURE, BODY, HEADERS, MIME, TEXT, ALL }
			public static readonly int F_RDWR = 1 << 0;
			public static readonly int F_HASFLDS = 1 << 1;
			public static readonly int F_HASMIME = 1 << 2;
			public static readonly int F_MDTYMIME = 1 << 3;
			public static readonly int F_EXCL = 1 << 4; //the specified values are exclusive

			public readonly OPCODE code;
			public readonly int flags;
			public readonly int batchsize;
			public readonly string parenth; //if non-null, char-0 is opening parenthesis, and char-0 is the closing

			public bool isSet(int f) { return isFlagSet(flags, f); }

			public FetchOpDef(OPCODE opcode, int opflags, int bs, string parentheses)
			{
				if ((opflags & F_MDTYMIME) != 0) opflags |= F_HASMIME;
				code = opcode;
				flags = opflags;
				batchsize = bs;
				parenth = parentheses;
			}
		}

		class FetchOp
		{
			public FetchOpDef def;
			public readonly string replytag;
			public readonly bool peek;
			public string[] hdrs;
			public com.grey.@base.collections.NumberList mime_coords;
			public int partial_off;
			public int partial_len;

			public FetchOp(FetchOpDef opdef, string rtag, bool rdonly)
			{
				def = opdef; replytag = rtag.ToUpperInvariant(); peek = (rdonly || !def.isSet(FetchOpDef.F_RDWR));
			}
		}


		 class SearchKey
		{
			public enum ARGTYPE { ANY, KWORD, HEADER, NUMBER, DATE }
			public readonly string token;
			public readonly bool ignored;
			public readonly int argcnt;
			public readonly char msflag;
			public readonly bool excl_flag;
			public readonly ARGTYPE argtype;
			public readonly string alias;
			
			
			public SearchKey(string t, bool i, int c, char f, bool e, ARGTYPE at, string a)
			{
				token = t; ignored = i; argcnt = c; msflag = f; excl_flag = e; argtype = at; alias = a;
			}
			
			public SearchKey(string t, bool i, int c) 
				: this(t, i, c, (char)0, false, ARGTYPE.ANY, null)
			{  }
			
			public SearchKey(string t, char f, bool e) 
				: this(t, false, 0, f, e, ARGTYPE.ANY, null)
			{  }  //for MS flags
			
			public SearchKey(string t, ARGTYPE a, int c) 
				: this(t, false, c, (char)0, false, a, null)  
			{  }

			public SearchKey(string t, ARGTYPE a) 
				: this(t, a, 1)
			{  }
			public SearchKey(string t, string a)
				: this(t, false, 0, (char)0, false, ARGTYPE.ANY, a)
			{  }
		}


	}

}

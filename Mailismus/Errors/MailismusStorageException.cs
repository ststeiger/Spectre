
namespace com.grey.mailismus.errors
{
 
		// Copyright 2018 Yusef Badri - All rights reserved.
		// Mailismus is distributed under the terms of the GNU Affero General Public License, Version 3 (AGPLv3).
	

	public class MailismusStorageException 
		: MailismusException
	{
		private static readonly long serialVersionUID = 1L;

		public MailismusStorageException(string message)
				:base(message)
		{
		
		}

		public MailismusStorageException(System.Exception cause)
				:base(cause)
		{ }

		public MailismusStorageException(string message, System.Exception cause)
			: base(message, cause)
		{ }


	}

}

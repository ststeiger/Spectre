
namespace com.grey.mailismus.errors
{

	 // Copyright 2018 Yusef Badri - All rights reserved.
	 // Mailismus is distributed under the terms of the GNU Affero General Public License, Version 3 (AGPLv3).
	

	// import com.grey.naf.errors.NAFException;

	public class MailismusException 
		: System.Exception 
	{
		private static readonly long serialVersionUID = 1L;

		public MailismusException(string message)
			:base(message)
		{ }

		public MailismusException(System.Exception cause)
			: base(cause.Message, cause)
		{ }


		public MailismusException(string message, System.Exception cause)
			:base(message, cause)
		{
			
		}

		public static bool isError(System.Exception ex)
		{
			if (ex is MailismusException) 
					return false;

			return false; // NAFException.isError(ex);
		}
	}


}

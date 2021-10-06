
// greysoft-java-master\greybase\src\main\java\com\grey\base\sasl\SaslEntity.java
namespace com.grey.@base.sasl 
{


	public abstract class SaslEntity
	{
		public enum MECH { PLAIN, CRAM_MD5, EXTERNAL }
		public static readonly string MECHNAME_PLAIN = MECH.PLAIN.ToString();
		public static readonly string MECHNAME_CMD5 = MECH.CRAM_MD5.ToString().Replace('_', '-');
		public static readonly string MECHNAME_EXTERNAL = MECH.EXTERNAL.ToString();

		public readonly MECH mechanism;
		
	}
}

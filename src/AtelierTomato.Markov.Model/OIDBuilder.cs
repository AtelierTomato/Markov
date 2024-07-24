using System.Security.Cryptography;
using System.Text;

namespace AtelierTomato.Markov.Model;

public class OIDBuilder
{
	private readonly StringBuilder stringBuilder;

	public OIDBuilder(ServiceType serviceType)
	{
		stringBuilder = new StringBuilder(serviceType.ToString());
	}

	public OIDBuilder Append(string field)
	{
		stringBuilder.Append(":").Append(OIDEscapement.Escape(field));
		return this;
	}

	public string Build() => stringBuilder.ToString();
}
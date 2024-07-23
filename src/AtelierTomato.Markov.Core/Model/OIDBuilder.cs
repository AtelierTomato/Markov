using System.Security.Cryptography;
using System.Text;

namespace AtelierTomato.Markov.Core.Model;

public class OIDBuilder
{
	private readonly StringBuilder stringBuilder;

	public OIDBuilder(ServiceType serviceType)
	{
		this.stringBuilder = new StringBuilder(serviceType.ToString());
	}

	public OIDBuilder Append(string field)
	{
		this.stringBuilder.Append(":").Append(OIDEscapement.Escape(field));
		return this;
	}

	public string Build() => this.stringBuilder.ToString();
}
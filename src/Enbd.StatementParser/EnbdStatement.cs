using System.Text.Json.Serialization;

namespace Enbd.StatementParser;

/// <summary />
[JsonDerivedType( typeof( CreditCardStatement ), typeDiscriminator: "cc" )]
[JsonDerivedType( typeof( CurrentAccountStatement ), typeDiscriminator: "ca" )]
public class EnbdStatement
{
}
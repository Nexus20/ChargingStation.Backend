namespace ChargingStation.Common.Messages_OCPP20;
#pragma warning disable // Disable all warnings

/// <summary>Used algorithms for the hashes provided.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public enum HashAlgorithmEnumType
{
    [System.Runtime.Serialization.EnumMember(Value = @"SHA256")]
    SHA256 = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"SHA384")]
    SHA384 = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"SHA512")]
    SHA512 = 2,

}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class OCSPRequestDataType
{
    [Newtonsoft.Json.JsonProperty("customData", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CustomDataType CustomData { get; set; }

    [Newtonsoft.Json.JsonProperty("hashAlgorithm", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public HashAlgorithmEnumType HashAlgorithm { get; set; }

    /// <summary>Hashed value of the Issuer DN (Distinguished Name).
    /// 
    /// </summary>
    [Newtonsoft.Json.JsonProperty("issuerNameHash", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [System.ComponentModel.DataAnnotations.StringLength(128)]
    public string IssuerNameHash { get; set; }

    /// <summary>Hashed value of the issuers public key
    /// </summary>
    [Newtonsoft.Json.JsonProperty("issuerKeyHash", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [System.ComponentModel.DataAnnotations.StringLength(128)]
    public string IssuerKeyHash { get; set; }

    /// <summary>The serial number of the certificate.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("serialNumber", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [System.ComponentModel.DataAnnotations.StringLength(40)]
    public string SerialNumber { get; set; }

    /// <summary>This contains the responder URL (Case insensitive). 
    /// 
    /// </summary>
    [Newtonsoft.Json.JsonProperty("responderURL", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    [System.ComponentModel.DataAnnotations.StringLength(512)]
    public string ResponderURL { get; set; }


}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
public partial class AuthorizeRequest
{
    [Newtonsoft.Json.JsonProperty("customData", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CustomDataType CustomData { get; set; }

    [Newtonsoft.Json.JsonProperty("idToken", Required = Newtonsoft.Json.Required.Always)]
    [System.ComponentModel.DataAnnotations.Required]
    public IdTokenType IdToken { get; set; } = new IdTokenType();

    /// <summary>The X.509 certificated presented by EV and encoded in PEM format.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("certificate", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [System.ComponentModel.DataAnnotations.StringLength(5500)]
    public string Certificate { get; set; }

    [Newtonsoft.Json.JsonProperty("iso15118CertificateHashData", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [System.ComponentModel.DataAnnotations.MinLength(1)]
    [System.ComponentModel.DataAnnotations.MaxLength(4)]
    public System.Collections.Generic.ICollection<OCSPRequestDataType> Iso15118CertificateHashData { get; set; }


}
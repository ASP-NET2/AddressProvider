using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AddressLibrary.Data.Entities;

public class AddressEntity
{

    [Key]
    public int AddressId { get; set; }
    public string? AddressTitle { get; set; }
    public string? AddressLine_1 { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }

    [ForeignKey("AccountUser")]
    public int AccountId { get; set; }

    [JsonIgnore]
    public AccountUserEntity? AccountUser { get; set; }

}

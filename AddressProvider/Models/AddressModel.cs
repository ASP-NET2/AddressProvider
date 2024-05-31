using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace AddressProvider.Models;

public class AddressModel
{
    public int AddressId { get; set; }
    public string? AddressTitle { get; set; }
    public string? AddressLine_1 { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public int AccountId { get; set; }
}

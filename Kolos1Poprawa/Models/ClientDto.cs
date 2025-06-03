using System.ComponentModel.DataAnnotations;

namespace Kolos1Poprawa.Models;

public class RentalDto {
    public string Vin { get; set; }
    public string Color { get; set; }
    public string Model { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalPrice { get; set; }
}
public class ClientDto {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public List<RentalDto> Rentals { get; set; } = new List<RentalDto>();
}
public class CreateClientDto {
    [Required]
    public ClientInfo Client { get; set; }
    [Required]
    public int CarId { get; set; }
    [Required]
    public DateTime DateFrom { get; set; }
    [Required]
    public DateTime DateTo { get; set; }
}

public class ClientInfo {
    [Required, MaxLength(50)]
    public string FirstName { get; set; }
    [Required, MaxLength(100)]
    public string LastName { get; set; }
    [Required, MaxLength(100)]
    public string Address { get; set; }
}
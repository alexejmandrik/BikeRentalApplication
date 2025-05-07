using BikeRentalApplication.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BikeBooking
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }  

    [Required]
    public int BikeId { get; set; }

    [ForeignKey("BikeId")]
    public Bike Bike { get; set; }

    [Required]
    public DateTime StartDateTime { get; set; }

    [Required]
    public DateTime EndDateTime { get; set; }

    public string? Comment { get; set; }

    [Required]
    public string BookingStatus { get; set; }

    [Required]
    public decimal Price { get; set; }
}

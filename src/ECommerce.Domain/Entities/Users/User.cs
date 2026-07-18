using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ECommerce.Domain.Enums.User;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Domain.Entities.Users;

public class User : IdentityUser
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 50 characters")]
    public string FirstName { get; set; }  = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 50 characters")]
    public string LastName { get; set; } = string.Empty;
    
    [DataType(DataType.Date)]
    [Column(TypeName = "date")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(2)] 
    public string? CountryCode { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;
    public string? ProfilePictureUrl { get; set; }
    public Gender? Gender { get; set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime UpdatedAt { get; set; }
    
    //Navigation Properties
    public  ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    public  ICollection<Address> Addresses { get; set; } = new List<Address>();
    public  ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
    public  ICollection<Order> Orders { get; set; } = new List<Order>();
    public  ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public  ICollection<Cart> Carts { get; set; } = new List<Carts.Cart>();
    public  ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
    public  ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();
    public  ICollection<InventoryHistory> InventoryHistories { get; set; } = new List<InventoryHistory>();
    public  ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
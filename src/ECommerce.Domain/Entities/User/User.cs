using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ECommerce.Domain.Enums.User;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Domain.Entities.User;

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
    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Review.ProductReview> ProductReviews { get; set; } = new List<Review.ProductReview>();
    public virtual ICollection<Order.Order> Orders { get; set; } = new List<Order.Order>();
    public virtual ICollection<Wishlist.Wishlist> Wishlists { get; set; } = new List<Wishlist.Wishlist>();
    public virtual ICollection<Cart.Cart> Carts { get; set; } = new List<Cart.Cart>();
    public virtual ICollection<Coupon.CouponUsage> CouponUsages { get; set; } = new List<Coupon.CouponUsage>();
    public virtual ICollection<Return.ReturnRequest> ReturnRequests { get; set; } = new List<Return.ReturnRequest>();
    public virtual ICollection<Inventory.InventoryHistory> InventoryHistories { get; set; } = new List<Inventory.InventoryHistory>();
    public virtual ICollection<Payment.Payment> Payments { get; set; } = new List<Payment.Payment>();
}
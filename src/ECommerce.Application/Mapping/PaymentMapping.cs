using ECommerce.Application.DTO.Payment;
using ECommerce.Domain.Entities.Payments;

namespace ECommerce.Application.Mapping;

public class PaymentMapping : Profile
{
    public PaymentMapping()
    {
        CreateMap<Payment, GetPaymentDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Method, o => o.MapFrom(s => s.Method.ToString()));
    }
}

using ECommerce.Application.DTO.Payment;
using ECommerce.Domain.Entities.Payments;

namespace ECommerce.Application.Mapping;

public class PaymentMapping : Profile
{
    public PaymentMapping()
    {
        CreateMap<Payment, GetPaymentDTO>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Method, o => o.MapFrom(s => s.Method.ToString()))
            .ForMember(d => d.StatusHistory, o => o.MapFrom(s => s.StatusHistories.OrderBy(h => h.CreatedAt)));

        CreateMap<PaymentStatusHistory, GetPaymentStatusHistoryDTO>()
            .ForMember(d => d.PaymentStatus, o => o.MapFrom(s => s.PaymentStatus.ToString()))
            .ForMember(d => d.ChangedBy, o => o.MapFrom(s => s.CreatedBy));
    }
}

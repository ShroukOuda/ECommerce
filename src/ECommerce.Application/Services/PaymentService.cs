using ECommerce.Application.DTO.Payment;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Payments;
using ECommerce.Domain.Enums.Payment;


namespace ECommerce.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePaymentDTO> _createValidator;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreatePaymentDTO> createValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<IEnumerable<GetPaymentDTO>> GetPaymentsByOrderIdAsync(Guid orderId)
    {
        var spec = new PaymentsByOrderSpecification(orderId);
        var payments = await _unitOfWork.GetRepository<Payment, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetPaymentDTO>>(payments);
    }

    public async Task<GetPaymentDTO> GetPaymentByIdAsync(Guid id)
    {
        var payment = await _unitOfWork.GetRepository<Payment, Guid>().GetByIdAsync(id);
        if (payment is null) throw new KeyNotFoundException($"Payment with ID {id} not found.");
        return _mapper.Map<GetPaymentDTO>(payment);
    }

    public async Task<GetPaymentDTO> CreatePaymentAsync(CreatePaymentDTO dto)
    {
        var result = await _createValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var payment = new Payment
        {
            OrderId = dto.OrderId,
            UserId = dto.UserId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            TransactionId = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Method = Enum.Parse<PaymentMethod>(dto.Method),
            Status = PaymentStatus.Pending,
            PaidAt = DateTime.UtcNow
        };

        await _unitOfWork.GetRepository<Payment, Guid>().AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetPaymentDTO>(payment);
    }
}

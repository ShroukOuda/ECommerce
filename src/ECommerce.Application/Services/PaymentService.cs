using ECommerce.Application.DTO.Payment;
using ECommerce.Domain.Entities.Payment;
using ECommerce.Domain.Interfaces.Repositories;

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

    public async Task<IEnumerable<GetPaymentDTO>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        var payments = await _unitOfWork.PaymentRepository.GetPaymentsByOrderIdAsync(orderId, ct);
        return _mapper.Map<IEnumerable<GetPaymentDTO>>(payments);
    }

    public async Task<GetPaymentDTO> GetPaymentByIdAsync(Guid id, CancellationToken ct = default)
    {
        var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id, ct);
        if (payment is null) throw new KeyNotFoundException($"Payment with ID {id} not found.");
        return _mapper.Map<GetPaymentDTO>(payment);
    }

    public async Task<GetPaymentDTO> CreatePaymentAsync(CreatePaymentDTO dto, CancellationToken ct = default)
    {
        var result = await _createValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var payment = new Payment
        {
            OrderId = dto.OrderId,
            UserId = dto.UserId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            TransactionId = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Method = Enum.Parse<ECommerce.Domain.Enums.Payment.PaymentMethod>(dto.Method),
            Status = ECommerce.Domain.Enums.Payment.PaymentStatus.Pending,
            PaidAt = DateTime.UtcNow
        };

        await _unitOfWork.PaymentRepository.AddAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetPaymentDTO>(payment);
    }
}

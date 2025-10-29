using AutoMapper;
using E_Commerece.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerece.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IMapper _mapper;
    public BaseController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
}
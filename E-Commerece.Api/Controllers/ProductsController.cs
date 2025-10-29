using AutoMapper;
using E_Commerece.Api.Helper;
using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;
using E_Commerece.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerece.Api.Controllers;


public class ProductsController : BaseController
{
    public ProductsController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(p=>p.Category, p=>p.Photos);
            if (products is null)
                return NotFound(new ResponseAPI(404));
            
            var productsDTO = _mapper.Map<IEnumerable<GetProductDTO>>(products);
            return Ok(productsDTO);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id, p=>p.Category, p=>p.Photos);
            if (product is null)
                return NotFound(new ResponseAPI(404));
            
            var productDTO = _mapper.Map<GetProductDTO>(product);
            return Ok(productDTO);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(ProductDTO productDTO)
    {
        try
        {
            var product = _mapper.Map<Product>(productDTO);
            await _unitOfWork.ProductRepository.AddAsync(product);
            return Ok(new ResponseAPI(200, "Product added successfully"));

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateProductDTO productDTO)
    {
        try
        {
            var product = _mapper.Map<Product>(productDTO);
            await _unitOfWork.ProductRepository.UpdateAsync(product);
            return Ok(new ResponseAPI(200, "Product updated successfully"));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _unitOfWork.ProductRepository.DeleteAsync(id);
            return Ok(new ResponseAPI(200, "Product deleted successfully"));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
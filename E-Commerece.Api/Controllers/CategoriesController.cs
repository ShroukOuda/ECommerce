using AutoMapper;
using E_Commerece.Api.Helper;
using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;
using E_Commerece.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerece.Api.Controllers;

public class CategoriesController : BaseController
{
    public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
    {
    }
    
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            if (categories is null)
                return NotFound(new ResponseAPI(404));
            return Ok(categories);
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
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return NotFound(new ResponseAPI(404, $"Id {id} doesn't exist"));
            return Ok(category);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(CategoryDTO categoryDto)
    {
        try
        {
            var category = _mapper.Map<CategoryDTO, Category>(categoryDto);
            await _unitOfWork.CategoryRepository.AddAsync(category);
            return Ok(new ResponseAPI(200, "Category added successfully"));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateCategoryDTO categoryDto)
    {
        try
        {
            var category = _mapper.Map<UpdateCategoryDTO, Category>(categoryDto);
            await  _unitOfWork.CategoryRepository.UpdateAsync(category);
            return Ok(new ResponseAPI(200, "Category updated successfully"));
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
            await _unitOfWork.CategoryRepository.DeleteAsync(id);
            return Ok(new ResponseAPI(200, "Category deleted successfully"));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
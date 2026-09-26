using Microsoft.AspNetCore.Mvc;
using ServiceRepoApi.Models;
using ServiceRepoApi.Services;

namespace ServiceRepoApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound(new { error = "Product not found." });
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        var result = await _productService.CreateAsync(product);

        if (!result.Success)
        {
            return ToErrorResult(result);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest(new
            {
                error = "Route id and product id do not match."
            });
        }

        var result = await _productService.UpdateAsync(product);

        if (!result.Success)
        {
            return ToErrorResult(result);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);

        if (!result.Success)
        {
            return ToErrorResult(result);
        }

        return NoContent();
    }

    private ActionResult ToErrorResult(ServiceResult result)
    {
        return result.Status switch
        {
            ServiceResultStatus.NotFound =>
                NotFound(new { error = result.Error }),

            ServiceResultStatus.Conflict =>
                Conflict(new { error = result.Error }),

            _ =>
                BadRequest(new { error = result.Error })
        };
    }
}
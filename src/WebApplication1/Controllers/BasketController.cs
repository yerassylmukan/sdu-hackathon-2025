using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpGet]
    public async Task<ActionResult<BasketModel>> GetUserBasket()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized("User ID is missing");

        var result = await _basketService.GetBasketAsync(userId);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<BasketModel>> AddItemToBasket([FromBody] BasketItemRequestModel basketItemRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized("User ID is missing");

        var result = await _basketService.AddItemToBasketAsync(userId, basketItemRequest);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetItemCount()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized("User ID is missing");

        var result = await _basketService.GetItemCountAsync(userId);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("{basketItemId}")]
    public async Task<ActionResult<BasketModel>> IncreaseItemQuantity(Guid basketItemId)
    {
        var result = await _basketService.IncreaseItemQuantityAsync(basketItemId);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("{basketItemId}")]
    public async Task<ActionResult<BasketModel>> DecreaseItemQuantity(Guid basketItemId)
    {
        var result = await _basketService.DecreaseItemQuantityAsync(basketItemId);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{basketItemId}")]
    public async Task<IActionResult> RemoveItemFromBasket(Guid basketItemId)
    {
        var result = await _basketService.RemoveItemFromBasketAsync(basketItemId);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearUserBasket()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return Unauthorized("User ID is missing");

        var result = await _basketService.ClearBasketAsync(userId);
        if (result.IsFailure) return BadRequest(result.Error);

        return Ok();
    }
}
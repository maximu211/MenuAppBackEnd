﻿using MenuApp.BLL.DTO.ReceiptsDTOs;
using MenuApp.BLL.Services.ReceiptService;
using MenuApp.BLL.Services.SubscriptionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReceipt(ReceiptsDTO receipt)
        {
            var result = await _receiptService.AddReceipt(receipt);
            if (result.Success == true)
                return Ok(result);
            else
                return BadRequest(result.Message);
        }
    }
}
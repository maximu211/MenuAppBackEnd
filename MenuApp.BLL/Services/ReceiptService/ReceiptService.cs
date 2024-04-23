using AutoMapper;
using MenuApp.BLL.DTO.ReceiptsDTOs;
using MenuApp.BLL.Services.MenuApp.BLL.Services;
using MenuApp.BLL.Utils.Authorization;
using MenuApp.DAL.Models.EntityModels;
using MenuApp.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace MenuApp.BLL.Services.ReceiptService
{
    public interface IReceiptService
    {
        Task<ServiceResult> GetReceiptsById(ObjectId userId);
        Task<ServiceResult> GetReceiptsBySubscriptions(ObjectId userId);
        Task<ServiceResult> DeleteReceipt(ObjectId receiptId);
        Task<ServiceResult> UpdateReceipt(Receipts receipt);
        Task<ServiceResult> AddReceipt(ReceiptsDTO receipt);
    }

    public class ReceiptService : IReceiptService
    {
        private readonly ILogger<ReceiptService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IReceiptsRepository _receiptsRepository;
        private readonly IGenerateJwtToken _jwtGenerator;
        private readonly IMapper _mapper;

        public ReceiptService(
            ILogger<ReceiptService> logger,
            IHttpContextAccessor httpContextAccessor,
            IReceiptsRepository receiptsRepository,
            IGenerateJwtToken generateJwtToken,
            IMapper mapper
        )
        {
            _logger = logger;
            _httpContextAccessor =
                httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _receiptsRepository = receiptsRepository;
            _jwtGenerator = generateJwtToken;
            _mapper = mapper;
        }

        public async Task<ServiceResult> AddReceipt(ReceiptsDTO receipt)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
                    c.Type == "userId"
                );
                if (userIdClaim == null)
                {
                    throw new Exception("userId claim is missing in the token");
                }

                receipt.UserId = userIdClaim.Value;

                await _receiptsRepository.AddReceipt(_mapper.Map<Receipts>(receipt));

                _logger.LogInformation($"Receipt successfuly created by user: {userIdClaim.Value}");
                return new ServiceResult(true, "Receipt successfuly created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating receipt: {ex}");
                return new ServiceResult(true, "An error occurred while creating receipt");
            }
        }

        public Task<ServiceResult> DeleteReceipt(ObjectId receiptId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> GetReceiptsById(ObjectId userId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> GetReceiptsBySubscriptions(ObjectId userId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> UpdateReceipt(Receipts receipt)
        {
            throw new NotImplementedException();
        }
    }
}

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
        Task<ServiceResult> GetReceiptsByUserId(GetReceiptByUserIdDTO userId);
        Task<ServiceResult> GetReceiptsBySubscriptions(ObjectId userId);
        Task<ServiceResult> DeleteReceipt(DeleteReceiptDTO receiptId);
        Task<ServiceResult> UpdateReceipt(Receipes receipt);
        Task<ServiceResult> AddReceipt(ReceiptsDTO receipt);
    }

    public class ReceiptService : IReceiptService
    {
        private readonly ILogger<ReceiptService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IReceipesRepository _receiptsRepository;
        private readonly IGenerateJwtToken _jwtGenerator;
        private readonly IMapper _mapper;

        public ReceiptService(
            ILogger<ReceiptService> logger,
            IHttpContextAccessor httpContextAccessor,
            IReceipesRepository receiptsRepository,
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

                await _receiptsRepository.AddReceipt(_mapper.Map<Receipes>(receipt));

                _logger.LogInformation($"Receipt successfuly created by user: {userIdClaim.Value}");
                return new ServiceResult(true, "Receipt successfuly created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating receipt: {ex}");
                return new ServiceResult(true, "An error occurred while creating receipt");
            }
        }

        public async Task<ServiceResult> DeleteReceipt(DeleteReceiptDTO receiptId)
        {
            try
            {
                await _receiptsRepository.DeleteReceipt(ObjectId.Parse(receiptId.ReceiptId));
                _logger.LogInformation($"Receipt {receiptId} successfuly deleted");
                return new ServiceResult(true, "Receipt successfuly deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting receipt: {ex}");
                return new ServiceResult(false, "An error occurred while deleting receipt");
            }
        }

        public async Task<ServiceResult> GetReceiptsByUserId(
            GetReceiptByUserIdDTO getReceiptByUserId
        )
        {
            try
            {
                var receiptList = await _receiptsRepository.GetReceiptsByUserId(
                    ObjectId.Parse(getReceiptByUserId.UserId)
                );

                _logger.LogInformation(
                    $"Data successfuly sended by userid: {getReceiptByUserId.UserId}"
                );
                return new ServiceResult(true, "Receipt List succesfuly sended", receiptList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while sending receipt list by userId {ex}");
                return new ServiceResult(false, "An error occurred while sending receipt list");
            }
        }

        public Task<ServiceResult> GetReceiptsBySubscriptions(ObjectId userId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> UpdateReceipt(Receipes receipt)
        {
            throw new NotImplementedException();
        }
    }
}

using ShoppingService.DTOs;
using ShoppingService.Models;
using ShoppingService.Repository;

namespace ShoppingService.Services
{
    public class CartDetailService : ICartDetailService
    {
        private readonly ICartDetailRepository _cartDetailRepository;
        private readonly HttpClient _httpClient;
        private readonly ShoppingContext _context;

        public CartDetailService(
            ICartDetailRepository cartDetailRepository,
            IHttpClientFactory httpClientFactory,
            ShoppingContext context)
        {
            _cartDetailRepository = cartDetailRepository;
            _httpClient = httpClientFactory.CreateClient(nameof(CartDetailService));
            _context = context;
        }


        public async Task<IEnumerable<CartDetailDTO>> GetCartDetailsByCartIdCartDetailService(int cartId)
        {
            return await _cartDetailRepository.GetCartDetailsByCartIdCartDetailRepository(cartId);
        }


        public async Task<CartDetail> GetCartDetailByCartIdAndRecordIdCartDetailService(int cartId, int recordId)
        {
            return await _cartDetailRepository.GetCartDetailByCartIdAndRecordIdCartDetailRepository(cartId, recordId);
        }


        public async Task AddCartDetailCartDetailService(CartDetail cartDetail)
        {
            var record = await _httpClient.GetFromJsonAsync<RecordDTO>($"/api/records/{cartDetail.RecordId}"); // Ajusta la ruta según tu API

            if (record == null)
            {
                throw new InvalidOperationException("Record not found");
            }

            if (record.Stock < cartDetail.Amount)
            {
                throw new InvalidOperationException($"Not enough stock for record {record.IdRecord}. Available: {record.Stock}, Requested: {cartDetail.Amount}");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.CartDetails.Add(cartDetail);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        
        }


        public async Task UpdateCartDetailCartDetailService(CartDetail cartDetail)
        {
            await _cartDetailRepository.UpdateCartDetailCartDetailRepository(cartDetail);
        }


        public async Task RemoveFromCartDetailCartDetailService(CartDetail cartDetail)
        {
            _cartDetailRepository.RemoveFromCartDetailCartDetailRepository(cartDetail);
            await _cartDetailRepository.SaveCartDetailRepository();
        }

    }
}

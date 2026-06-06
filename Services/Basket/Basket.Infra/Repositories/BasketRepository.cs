using Basket.Core.Repo;
using Basket.Core.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.Infra.Repositories
{
    public class BasketRepository : IBasketRepo
    {
        private readonly IDistributedCache _cache;
        public BasketRepository(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<ShoppingCart> GetBasket(string userName)
        {
            userName = userName?.Trim().ToLower();
            var basket = await _cache.GetStringAsync(userName);
            if (string.IsNullOrEmpty(basket))
                return null;
            return JsonConvert.DeserializeObject<ShoppingCart>(basket); 
        }
        public async Task<ShoppingCart> UpdateBasket(ShoppingCart shoppingCart)
        {
            shoppingCart.UserName = shoppingCart.UserName?.Trim().ToLower();
            await _cache.SetStringAsync(shoppingCart.UserName, JsonConvert.SerializeObject(shoppingCart));
            return await GetBasket(shoppingCart.UserName);
        }
        public async Task DeleteBasket(string userName)
        {
            userName = userName?.Trim().ToLower();
            await _cache.RemoveAsync(userName);
        }
    }
}
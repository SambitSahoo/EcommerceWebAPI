using Basket.Core.Entities;

namespace Basket.Core.Repo
{
    public interface IBasketRepo
    {
        Task<ShoppingCart> GetBasket(string userName);
        Task<ShoppingCart> UpdateBasket(ShoppingCart shoppingCart);
        Task DeleteBasket(string userName);
    }
}
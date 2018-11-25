﻿using ECOMCupCake.Data;
using ECOMCupCake.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECOMCupCake.Models.Services
{
    public class BasketService : IBasket
    {
        private StoreDbContext _storeDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public BasketService(StoreDbContext context)
        {
            _storeDbContext = context;
        }

        /// <summary>
        /// Adds the product asynchronous.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="ProductId">The product identifier.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public async Task<Basket> AddProductAsync(string UserId, int ProductId, int quantity = 1)
        {
            Basket basket = await GetProductInBasket(UserId, ProductId);
            if (basket != null)
            {
                basket.Quantity += 1;
            }
            else
            {

                 basket = new Basket()
                {
                    ProductID = ProductId,
                    UserID = UserId,
                    Quantity = quantity
                };

                _storeDbContext.Add(basket);
            }

            try
            {
                await _storeDbContext.SaveChangesAsync();
                return basket;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deletes the product.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="productInBasket">The product in basket.</param>
        /// <returns></returns>
        public async Task DeleteProduct(string userId, int productId, int? orderId = null)
        {
            Basket basket = await GetProductInBasket(userId, productId, orderId);
            _storeDbContext.Baskets.Remove(basket);
            try
            {
                await _storeDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all in basket.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Basket>> GetAllInBasket(string UserId)
        {
            return await _storeDbContext.Baskets.Include(p => p.Product).Where(b => b.UserID == UserId && b.OrderID == null).ToListAsync();
        }

        /// <summary>
        /// Baskets the count.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <returns></returns>
        public async Task<int> BasketCount(string UserId)
        {
            return await _storeDbContext.Baskets.Where(b => b.UserID == UserId && b.OrderID == null).SumAsync(x => x.Quantity);
        }

        /// <summary>
        /// Gets the product in basket.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="ProductId">The product identifier.</param>
        /// <returns></returns>
        public async Task<Basket> GetProductInBasket(string userId, int productId, int? orderId = null)
        {
            return await _storeDbContext.Baskets.FirstOrDefaultAsync(b => b.UserID == userId && b.ProductID == productId && b.OrderID == orderId);
        }

        /// <summary>
        /// Updates the specified user identifier.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="ProductInBasket">The product in basket.</param>
        /// <returns></returns>
        public async Task Update(Basket ProductInBasket)
        {
            _storeDbContext.Baskets.Update(ProductInBasket);
            try
            {
                await _storeDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Products the exists in basket.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="ProductId">The product identifier.</param>
        /// <returns></returns>
        public bool ProductExistsInBasket(string UserId, int ProductId, int ? orderId = null)
        {
            return _storeDbContext.Baskets.Any(b => b.UserID == UserId && b.ProductID == ProductId && b.OrderID == orderId);
        }
    }
}

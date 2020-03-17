﻿using Microsoft.EntityFrameworkCore;
using OkkaKalipWeb.DataAccess.Abstract;
using OkkaKalipWeb.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OkkaKalipWeb.DataAccess.Concrete.EfCore
{
    public class EfCoreProductDal : EfCoreRepository<Product, OkkaKalipContext>, IProductDal
    {
        public Product GetByIdWithCategories(int id)
        {
            using (var context = new OkkaKalipContext())
            {
                return context.Products
                    .Where(x => x.Id == id)
                    .Include(x => x.ProductCategories)
                    .ThenInclude(x => x.Category)
                    .FirstOrDefault();
            }
        }

        public Product GetProductDetails(int id)
        {
            using (var context = new OkkaKalipContext())
            {
                return context.Products
                    .Where(x => x.Id == id)
                    .Include(x => x.ProductCategories)
                    .ThenInclude(x => x.Category)
                    .FirstOrDefault();
            }
        }

        public List<Product> GetProductsByCategory(string category, int page, int pageSize)
        {
            using (var context = new OkkaKalipContext())
            {
                var products = context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(category))
                    products = products
                        .Include(x => x.ProductCategories)
                        .ThenInclude(x => x.Category)
                        .Where(x => x.ProductCategories.Any(c => c.Category.Name.ToLower() == category.ToLower()));

                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public int GetProductsByCategory(string category)
        {
            using (var context = new OkkaKalipContext())
            {
                var products = context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(category))
                    products = products
                        .Include(x => x.ProductCategories)
                        .ThenInclude(x => x.Category)
                        .Where(x => x.ProductCategories.Any(c => c.Category.Name.ToLower() == category.ToLower()));

                return products.Count();
            }
        }

        public void Update(Product entity, int[] categoryIds)
        {
            using (var context = new OkkaKalipContext())
            {
                var product = context.Products
                    .Include(x => x.ProductCategories)
                    .FirstOrDefault(x => x.Id == entity.Id);

                if (product != null)
                {
                    product.Name = entity.Name;
                    product.Description = entity.Description;
                    product.ImageUrl = entity.ImageUrl;
                    product.Price = entity.Price;

                    product.ProductCategories = categoryIds.Select(x => new ProductCategory()
                    {
                        CategoryId = x,
                        ProductId = entity.Id
                    }).ToList();

                    context.SaveChanges();
                }
            }
        }
    }
}
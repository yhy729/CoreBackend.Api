﻿using CoreBackend.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBackend.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MyContext _myContext;

        public ProductRepository(MyContext myContext)
        {
            _myContext = myContext;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _myContext.Products.OrderBy(x => x.Name).ToList();
        }

        public Product GetProduct(int productId, bool includeMaterials)
        {
            if (includeMaterials)
            {
                return _myContext.Products
                    .Include(x => x.Materials)
                    .FirstOrDefault(x => x.Id == productId);
            }
            return _myContext.Products.Find(productId);
        }

        public IEnumerable<Material> GetMaterialsForProduct(int productId)
        {
            return _myContext.Materials.Where(x => x.ProductId == productId).ToList();
        }

        public Material GetMaterialForProduct(int productId, int materialId)
        {
            return _myContext.Materials.FirstOrDefault(x => x.ProductId == productId && x.Id == materialId);
        }

        public bool ProductExist(int productId)
        {
            return _myContext.Products.Any(x => x.Id == productId);
        }

        public void AddProduct(Product product)
        {
            _myContext.Products.Add(product);
        }

        public void DeleteProduct(Product product)
        {
            _myContext.Products.Remove(product);
        }

        public bool Save()
        {
            return _myContext.SaveChanges() > 0;
        }
    }
}

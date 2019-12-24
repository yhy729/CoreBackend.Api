using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBackend.Api.Entities
{
    public static class MyContextExtensions
    {
        public static void EnsureSeedDataForContext(this MyContext context)
        {
            if (context.Products.Any())
            {
                return;
            }

           var products = new List<Product>
            {
                new Product
                {
                    Name = "牛奶",
                    Price = new decimal(2.5),
                    Description = "这是牛奶啊",
                    Materials = new List<Material>
                    {
                        new Material
                        {
                            Name = "水"
                        },
                        new Material
                        {
                            Name = "奶粉"
                        }
                    }
                },
                new Product
                {
                    Name = "面包",
                    Price = new decimal(4.5),
                    Description = "这是面包啊",
                    Materials = new List<Material>
                    {
                        new Material
                        {
                            Name = "面粉"
                        },
                        new Material
                        {
                            Name = "糖"
                        }
                    }
                },
                new Product
                {
                    Name = "啤酒",
                    Price = new decimal(7.5),
                    Description = "这是啤酒啊",
                    Materials = new List<Material>
                    {
                        new Material
                        {
                            Name = "麦芽"
                        },
                        new Material
                        {
                            Name = "地下水"
                        }
                    }
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}

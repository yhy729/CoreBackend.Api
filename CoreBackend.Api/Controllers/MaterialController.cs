using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreBackend.Api.Services;
using CoreBackend.Api.Repositories;
using CoreBackend.Api.Dto;
using AutoMapper;

namespace CoreBackend.Api.Controllers
{
    [Route("api/product")]
    public class MaterialController : Controller
    {
        private readonly IProductRepository _productRepository;

        public MaterialController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("{productId}/materials")]
        public IActionResult GetMaterials(int productId)
        {
            var producte = _productRepository.ProductExist(productId);
            if (!producte)
            {
                return NotFound();
            }

            var materials = _productRepository.GetMaterialsForProduct(productId);
            var result = Mapper.Map<IEnumerable<MaterialDto>>(materials);
            return Ok(result);
        }

        [HttpGet("{productId}/materials/{id}")]
        public IActionResult GetMaterial(int productId,int id)
        {
            var product = _productRepository.ProductExist(productId);
            if (!product)
            {
                return NotFound();
            }

            var material = _productRepository.GetMaterialForProduct(productId, id);
            if (material == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<MaterialDto>(material);

            return Ok(result);
        }
    }
}

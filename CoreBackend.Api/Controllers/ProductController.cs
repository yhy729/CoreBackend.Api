using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreBackend.Api.Dto;
using CoreBackend.Api.Entities;
using CoreBackend.Api.Repositories;
using CoreBackend.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreBackend.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        private readonly IMailService _mailService;

        private readonly IProductRepository _productRepository;

        public ProductController(ILogger<ProductController> logger, IMailService mailService, IProductRepository productRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _productRepository = productRepository;
        }

        [HttpGet("all")]
        public IActionResult GetProductes()
        {
            var products = _productRepository.GetProducts();
            var results = Mapper.Map<IEnumerable<ProductWithoutMaterialDto>>(products);
            return Ok(results);
        }

        [Route("{id}", Name = "GetProduct")]
        //[HttpGet("getone")]
        public IActionResult GetProduct(int id, bool includeMaterial = false)
        {
            var product = _productRepository.GetProduct(id, includeMaterial);
            if (product == null)
            {
                _logger.LogInformation($"Id为{id}的产品没有找到.");
                return NotFound();
            }

            if (includeMaterial)
            {
                var productWithMaterialResult = Mapper.Map<ProductDto>(product);
                return Ok(productWithMaterialResult);
            }

            var onlyProductResult = Mapper.Map<ProductWithoutMaterialDto>(product);
            return Ok(onlyProductResult);
        }

        [HttpPost("create")]
        public IActionResult Post([FromBody] ProductCreation product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (product.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newProduct = Mapper.Map<Product>(product);
            _productRepository.AddProduct(newProduct);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "保存产品的时候出错");
            }

            var dto = Mapper.Map<ProductWithoutMaterialDto>(newProduct);

            return CreatedAtRoute("GetProduct", new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProductModification product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (product.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = _productRepository.GetProduct(id, false);
            if (model == null)
            {
                return NotFound();
            }

            Mapper.Map(product, model);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "保存产品时候出错");
            }
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Path(int id, [FromBody] JsonPatchDocument<ProductModification> patcDoc)
        {
            if (patcDoc == null)
            {
                return BadRequest();
            }

            var model = _productRepository.GetProduct(id, true);
            if (model == null)
            {
                return NotFound();
            }

            var toPatch = Mapper.Map<ProductModification>(model);

            patcDoc.ApplyTo(toPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (toPatch.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }

            TryValidateModel(toPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(toPatch, model);

            if (!_productRepository.Save())
            {
                return StatusCode(500, "更新时出错");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = _productRepository.GetProduct(id, false);
            if (model == null)
            {
                return NotFound();
            }

            _productRepository.DeleteProduct(model);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "删除的时候出错");
            }

            _mailService.Send("Product Delete", $"Id为{id}的产品被删除了");
            return NoContent();
        }
    }
}

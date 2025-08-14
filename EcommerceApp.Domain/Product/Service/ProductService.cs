using EcommerceApp.Core.DTOs;
using EcommerceApp.Domain.Product.DTOs.Request;
using EcommerceApp.Domain.Product.DTOs.Response;
using EcommerceApp.Domain.Product.Interfaces;
using EcommerceApp.Domain.Product.Mappings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Domain.Product.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;

        public ProductService(
            IProductRepository productRepository,
            
            IStorageService storageService,
            IConfiguration configuration)
        {
            _productRepository = productRepository;
           
            _storageService = storageService;
            _configuration = configuration;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            var productData = await _productRepository.GetAllAsync();
            return productData.ToDto();
        }

        public async Task<ProductResponse?> GetByIdAsync(int id)
        {
            var productData = await _productRepository.GetByIdAsync(id);
            if (productData == null) return null;

            return productData.ToDto();
        }

        public async Task<IEnumerable<ProductResponse>> GetByCategoryAsync(int categoryId)
        {
            var productData = await _productRepository.GetByCategoryAsync(categoryId);
            // Remove .ToEntity() since productData is already IEnumerable<ProductModel>
            return productData.ToDto();
        }

        public async Task<PagedResult<ProductResponse>> GetPagedAsync(PaginationParameters parameters, ProductFilter? filter = null)
        {
            var pagedData = await _productRepository.GetPagedAsync(parameters, filter);
            var productDtos = pagedData.Data.ToDto().ToList();

            return new PagedResult<ProductResponse>(productDtos, pagedData.TotalRecords, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<ProductResponse> CreateAsync(CreateProduct createDto)
        {
            // Validate
            //var validationResult = await _createValidator.ValidateAsync(createDto);
            //if (!validationResult.IsValid)
            //{
            //    throw new ValidationException(validationResult.Errors);
            //}

            var product = createDto.ToEntity();

            // Upload image if provided
            if (createDto.Image is not null)
            {
                var imageUrl = await UploadProductImageAsync(createDto.Image);
                product.Image = imageUrl;
            }
            var id = await _productRepository.CreateAsync(product);
            product.Id = id;

            return product.ToDto();
        }

        public async Task<bool> UpdateAsync(int id, UpdateProduct updateDto)
        {
            // Basic validation
            //var validationResult = await _updateValidator.ValidateAsync(updateDto);
            //if (!validationResult.IsValid)
            //{
            //    throw new ValidationException(validationResult.Errors);
            //}

            //// Additional validation for update
            //var isValidForUpdate = await _updateValidator.ValidateForUpdateAsync(id, updateDto);
            //if (!isValidForUpdate)
            //{
            //    throw new ValidationException("Product validation failed for update.");
            //}

            var existingData = await _productRepository.GetByIdAsync(id);
            if (existingData == null) return false;

            
            var existingProduct = existingData;
            existingProduct.UpdateFromDto(updateDto);

            // Upload new image if provided
            if (updateDto.Image != null)
            {
                var imageUrl = await UploadProductImageAsync(updateDto.Image, id);
                existingProduct.Image = imageUrl;
            }

           
            return await _productRepository.UpdateAsync(existingProduct);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        public async Task<string> UploadImageAsync(UploadProductImage uploadDto)
        {
            var imageUrl = await UploadProductImageAsync(uploadDto.Image, uploadDto.ProductId);

            // Update product image
            await _productRepository.UpdateImageAsync(uploadDto.ProductId, imageUrl);

            return imageUrl;
        }

        private async Task<string?> UploadProductImageAsync(IFormFile? image, int? productId = null)
        {
            using var stream = new MemoryStream();
            await image.CopyToAsync(stream);

            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            var fileName = productId.HasValue
                ? $"product_{productId}_{Guid.NewGuid()}{fileExtension}"
                : $"product_{Guid.NewGuid()}{fileExtension}";

            var containerName = _configuration["AzureStorage:ContainerName"];
            return await _storageService.UploadAsync(stream.ToArray(), fileName, containerName);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IProductImageService
    {
        void Add(ProductImage productImage);

        void Delete(int id);

        List<ProductImage> GetAll(int productId);

        void Save();
    }

    public class ProductImageService : IProductImageService
    {
        private IUnitOfWork _unitOfWork;
        private IProductImageRepository _productImageRepository;

        public ProductImageService(IUnitOfWork unitOfWork, IProductImageRepository productImageRepository)
        {
            _unitOfWork = unitOfWork;
            _productImageRepository = productImageRepository;
        }

        public void Add(ProductImage productImage)
        {
            _productImageRepository.Add(productImage);
        }

        public void Delete(int id)
        {
            _productImageRepository.Delete(id);
        }

        public List<ProductImage> GetAll(int productId)
        {
            return _productImageRepository.GetMulti(x => x.ProductId == productId).ToList();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
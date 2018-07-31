using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMDT.Common.ViewModels;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IProductQuantityService
    {
        void Add(ProductQuantity productQuantity);
        void AddColor(Color color);
        void AddSize(Size size);
        void Delete(int productId, int colorId, int sizeId);
        void DeleteColor(int id);
        void DeleteSize(int id);
        List<ProductQuantity> GetAll(int productId, int? sizeId, int? colorId);

        bool CheckExist(int productId, int sizeId, int colorId);

        IEnumerable<Size> GetListSize(string filter);

        IEnumerable<Color> GetListColor(string filter);
        List<ListViewModel> GetListByProduct(int productId);
        bool SellProduct(int productId,int sizeId,int colorId ,int quantity);
        void Save();
    }

    public class ProductQuantityService : IProductQuantityService
    {
        private IUnitOfWork _unitOfWork;
        private IColorRepository _colorRepository;
        private ISizeRepository _sizeRepository;
        private IProductQuantityRepository _productQuantityRepository;

        public ProductQuantityService(IProductQuantityRepository productQuantityRepository, IColorRepository colorRepository, ISizeRepository sizeRepository, IUnitOfWork unitOfWork)
        {
            _productQuantityRepository = productQuantityRepository;
            _sizeRepository = sizeRepository;
            _colorRepository = colorRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(ProductQuantity productQuantity)
        {
            _productQuantityRepository.Add(productQuantity);
        }

        public bool CheckExist(int productId, int sizeId, int colorId)
        {
            return _productQuantityRepository.CheckContains(x => x.ProductId == productId && x.SizeId == sizeId && x.ColorId == colorId);
        }

        public void Delete(int productId, int colorId, int sizeId)
        {
            var product = _productQuantityRepository.GetSingleByCondition(x => x.ColorId == colorId && x.ProductId == productId && x.SizeId == sizeId);
            _productQuantityRepository.Delete(product);
        }

        public List<ProductQuantity> GetAll(int productId, int? sizeId, int? colorId)
        {
            var query = _productQuantityRepository.GetMulti(x => x.ProductId == productId, new string[] { "Color", "Size" });
            if (sizeId.HasValue)
                query = query.Where(x => x.SizeId == sizeId);
            if (colorId.HasValue)
                query = query.Where(x => x.ColorId == colorId);
            return query.ToList();
        }

        public IEnumerable<Color> GetListColor(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                return _colorRepository.GetMulti(x => x.Name.Contains(filter));
            }
            return _colorRepository.GetAll();
        }

        public IEnumerable<Size> GetListSize(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                return _sizeRepository.GetMulti(x => x.Name.Contains(filter));
            }
            return _sizeRepository.GetAll();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void AddColor(Color color)
        {
            _colorRepository.Add(color);
        }

        public void AddSize(Size size)
        {
            _sizeRepository.Add(size);
        }

        public void DeleteColor(int id)
        {
            var model = _colorRepository.GetSingleById(id);
            _colorRepository.Delete(model);
        }

        public void DeleteSize(int id)
        {
            var model = _sizeRepository.GetSingleById(id);
            _sizeRepository.Delete(model);
        }

        public List<ListViewModel> GetListByProduct(int productId)
        {
            var model = _productQuantityRepository.GetMulti(x => x.ProductId == productId);
            var list = new List<ListViewModel>();
            foreach(var item in model)
            {
                var size = _sizeRepository.GetSingleById(item.SizeId);
                var color = _colorRepository.GetSingleById(item.ColorId);
                list.Add(new ListViewModel() {
                    Name = color.Name + " " + size.Name,
                    Quantity = item.Quantity,
                    ColorId = color.ID,
                    SizeId = size.ID,
                    Status = false
                });
            }
            return list.ToList();
        }
        public bool SellProduct(int productId, int sizeId, int colorId, int quantity)
        {
            var query = _productQuantityRepository.GetMulti(x=>x.ColorId==colorId&&x.ProductId==productId&&x.SizeId==sizeId);
            foreach(var item in query)
            {
                if (item.Quantity >= quantity)
                    return true;
            }
            return false;
        }
    }
}
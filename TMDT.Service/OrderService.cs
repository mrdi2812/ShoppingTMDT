using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IOrderService
    {
        Order Add(Order order);

        List<Order> GetListByDate(string startDate, string endDate, string customerName, string paymentStatus,
            int pageIndex, int pageSize, out int totalRow);

        Order GetDetail(int orderId);

        OrderDetail CreateDetail(OrderDetail order);

        void DeleteDetail(int productId, int orderId, int colorId, int sizeId);
        void Delete(int orderId);
        void UpdateStatus(int orderId);

        List<OrderDetail> GetOrderDetails(int orderId);

        void Save();
    }

    public class OrderService : IOrderService
    {
        private IUnitOfWork _unitOfWork;
        private IOrderRepository _orderRepository;
        private IOrderDetailRepository _orderDetailRepository;

        public OrderService(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public Order Add(Order order)
        {
            try
            {
                _orderRepository.Add(order);
                _unitOfWork.Commit();
                return order;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Order> GetListByDate(string startDate, string endDate, string customerName, string paymentStatus, int pageIndex, int pageSize, out int totalRow)
        {
            var query = _orderRepository.GetAll();
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime start = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate >= start);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime end = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate <= end);
            }
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                query = query.Where(x => x.PaymentStatus == paymentStatus);
            }
            totalRow = query.Count();
            return query.OrderByDescending(x => x.CreatedDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }

        public Order GetDetail(int orderId)
        {
            return _orderRepository.GetSingleByCondition(x => x.ID == orderId, new string[] { "OrderDetails" });
        }

        public OrderDetail CreateDetail(OrderDetail order)
        {
            return _orderDetailRepository.Add(order);
        }

        public void DeleteDetail(int productId, int orderId, int colorId, int sizeId)
        {
            var query = _orderDetailRepository.GetSingleByCondition(x => x.OrderID == orderId && x.ProductID == productId && x.ColorId == colorId && x.SizeId == sizeId);
            _orderDetailRepository.Delete(query);         
        }

        public void UpdateStatus(int orderId)
        {
            var query = _orderRepository.GetSingleByCondition(x => x.ID == orderId);
            query.Status = true;
            _orderRepository.Update(query);
        }

        public List<OrderDetail> GetOrderDetails(int orderId)
        {
            return _orderDetailRepository.GetMulti(x => x.OrderID == orderId, new string[] { "Order", "Color", "Size", "Product" }).ToList();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Delete(int orderId)
        {
            var order = _orderRepository.GetSingleById(orderId);
            _orderRepository.Delete(order);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using TMDT.Common.ViewModels;
using TMDT.Data.Infrastructure;
using TMDT.Model.Models;

namespace TMDT.Data.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<RevenueStatisticViewModel> GetRevenueStatistic(string fromDate, string toDate);
    }

    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<RevenueStatisticViewModel> GetRevenueStatistic(string fromDate, string toDate)
        {
            var query = from o in DbContext.Orders
                        join od in DbContext.OrderDetails
                        on o.ID equals od.OrderID
                        join p in DbContext.Products
                        on od.ProductID equals p.ID
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime start = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));

                query = query.Where(x => x.CreatedDate >= start);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
 
                query = query.Where(x => x.CreatedDate <= endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate ?? DateTime.Now))
                .Select(r => new
                {
                    Date = r.Key.Value,
                    TotalBuy = r.Sum(x => x.OriginalPrice * x.Quantity),
                    TotalSell = r.Sum(x => x.Price * x.Quantity),
                }).Select(x => new RevenueStatisticViewModel()
                {
                    Date = x.Date,
                    Benefit = x.TotalSell - x.TotalBuy,
                    Revenues = x.TotalSell
                });
            return result.ToList();
        }
    }
}
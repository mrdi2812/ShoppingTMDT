using TMDT.Data.Infrastructure;
using TMDT.Model.Models;

namespace TMDT.Data.Repositories
{
    public interface IFooterRepository : IRepository<Footer>
    { }

    public class FooterRepository : RepositoryBase<Footer>, IFooterRepository
    {
        public FooterRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
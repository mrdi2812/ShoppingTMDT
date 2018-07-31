using TMDT.Data.Infrastructure;
using TMDT.Model.Models;

namespace TMDT.Data.Repositories
{
    public interface IAppRoleRepository : IRepository<AppRole>
    {
    }

    public class AppRoleRepository : RepositoryBase<AppRole>, IAppRoleRepository
    {
        public AppRoleRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
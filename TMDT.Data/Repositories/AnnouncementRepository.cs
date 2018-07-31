using TMDT.Data.Infrastructure;
using TMDT.Model.Models;
using System.Linq;
using System.Data.Entity;

namespace TMDT.Data.Repositories
{
    public interface IAnnouncementRepository :IRepository<Announcement>
    {
        IQueryable<Announcement> GetAllUnread(string userId);
    }
    public class AnnouncementRepository : RepositoryBase<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IQueryable<Announcement> GetAllUnread(string userId)
        {
            var query = (from x in DbContext.Announcements.Include("AppUser")
                         join y in DbContext.AnnouncementUsers.DefaultIfEmpty()
                         on x.ID equals y.AnnouncementId                        
                         where (y.HasRead == false)
                         && (y.UserId == null || y.UserId == userId)
                         select x).Include(x => x.AppUser);
            return query;
        }
    }
}

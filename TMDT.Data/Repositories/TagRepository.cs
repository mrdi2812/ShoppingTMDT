using System;
using System.Collections.Generic;
using System.Linq;
using TMDT.Data.Infrastructure;
using TMDT.Model.Models;

namespace TMDT.Data.Repositories
{
    public interface ITagRepository : IRepository<Tag>
    {
      IEnumerable<Tag> GetListTagByProductID(int productId);
      IEnumerable<Tag> GetListTagByPostID(int postId);
    }

    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<Tag> GetListTagByPostID(int postId)
        {
            var query = (from t in DbContext.Tags
                         join pt in DbContext.PostTags
                         on t.ID equals pt.TagID
                         where pt.PostID == postId
                         select t);
            return query;
        }

        public IEnumerable<Tag> GetListTagByProductID(int productId)
        {
            var query = (from t in DbContext.Tags
                         join pt in DbContext.ProductTags
                         on t.ID equals pt.TagID
                         where pt.ProductID==productId
                         select t);
            return query;
        }
    }
}
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IPageService
    {
        Page Add(Page page);

        void Update(Page page);

        void Delete(int id);

        Page GetByAlias(string alias);
    }

    public class PageService : IPageService
    {
        private IUnitOfWork _unitOfWork;
        private IPageRepository _pageRepository;

        public PageService(IUnitOfWork unitOfWork, IPageRepository pageRepository)
        {
            _unitOfWork = unitOfWork;
            _pageRepository = pageRepository;
        }

        public Page Add(Page page)
        {
            return _pageRepository.Add(page);
        }

        public void Update(Page page)
        {
            _pageRepository.Update(page);
        }

        public void Delete(int id)
        {
            _pageRepository.Delete(id);
        }

        public Page GetByAlias(string alias)
        {
            return _pageRepository.GetSingleByCondition(x => x.Alias == alias);
        }
    }
}
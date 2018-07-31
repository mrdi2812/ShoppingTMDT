using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IMenuService
    {
        Menu Add(Menu menu);
        void Update(Menu menu);
        void Delete(string id);
        IEnumerable<Menu> GetAll();
        void Save();
        Menu GetById(string id);
    }
    public class MenuService : IMenuService
    {
        private IUnitOfWork _unitOfWork;
        private IMenuRepository _menuRepository;
        public MenuService(IUnitOfWork unitOfWork, IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
            _unitOfWork = unitOfWork;
        }
        public Menu Add(Menu menu)
        {
            return _menuRepository.Add(menu);
        }

        public void Delete(string id)
        {
            var model = _menuRepository.GetSingleByCondition(x => x.ID == id);
            _menuRepository.Delete(model);
        }

        public IEnumerable<Menu> GetAll()
        {
            var model =  _menuRepository.GetAll();
            return model.OrderBy(x => x.ParentId);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(Menu menu)
        {
            _menuRepository.Update(menu);
        }

        public Menu GetById(string id)
        {
            return _menuRepository.GetSingleByCondition(x=>x.ID ==id);
        }
    }
}

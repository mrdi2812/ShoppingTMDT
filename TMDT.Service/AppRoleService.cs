using TMDT.Common.Exceptions;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IAppRoleService
    {
        AppRole Add(AppRole appRole);

        void Delete(string id);

        void Update(AppRole appRole);

        AppRole GetByName(string name);

        AppRole GetDetail(string id);

        void Save();
    }

    public class AppRoleService : IAppRoleService
    {
        private IAppRoleRepository _appRoleRepository;
        private IUnitOfWork _unitOfWork;

        public AppRoleService(IAppRoleRepository appRoleRepository, IUnitOfWork unitOfWork)
        {
            _appRoleRepository = appRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public AppRole Add(AppRole appRole)
        {
            if (_appRoleRepository.CheckContains(x => x.Description == appRole.Description || x.Name == appRole.Name))
            {
                throw new NameDuplicatedException("Tên không được trùng");
            }
            return _appRoleRepository.Add(appRole);
        }

        public void Delete(string id)
        {
            _appRoleRepository.DeleteMulti(x => x.Id == id);
        }

        public AppRole GetByName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return _appRoleRepository.GetSingleByCondition(x => x.Name == name || x.Description == name);
            }
            return null;
        }

        public AppRole GetDetail(string id)
        {
            return _appRoleRepository.GetSingleByCondition(x => x.Id == id);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(AppRole appRole)
        {
            if (_appRoleRepository.CheckContains(x => x.Description == appRole.Description && x.Id != appRole.Id))
                throw new NameDuplicatedException("Tên không được trùng");
            _appRoleRepository.Update(appRole);
        }
    }
}
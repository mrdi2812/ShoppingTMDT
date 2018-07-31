using System;
using System.Collections.Generic;
using System.Linq;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IPermissionService
    {
        ICollection<Permission> GetByFunctionId(string functionId);

        ICollection<Permission> GetByUserId(string userId);

        void Add(Permission permission);

        void DeleteAll(string functionId);
        void Delete(string roleId);

        void SaveChange();

        bool CheckContaint(string functionId);
        ICollection<Permission> GetByRoleId(string roleId);
    }

    public class PermissionService : IPermissionService
    {
        private IPermissionRepository _permissionRepository;
        private IUnitOfWork _unitOfWork;

        public PermissionService(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork)
        {
            this._permissionRepository = permissionRepository;
            this._unitOfWork = unitOfWork;
        }

        public void Add(Permission permission)
        {
            _permissionRepository.Add(permission);
        }

        public bool CheckContaint(string functionId)
        {
            return _permissionRepository.CheckContains(x => x.FunctionId == functionId);
        }

        public void DeleteAll(string functionId)
        {
            _permissionRepository.DeleteMulti(x => x.FunctionId == functionId);
        }
        public void Delete(string roleId)
        {
            _permissionRepository.DeleteMulti(x => x.RoleId == roleId);
        }

        public ICollection<Permission> GetByFunctionId(string functionId)
        {
            return _permissionRepository
                .GetMulti(x => x.FunctionId == functionId, new string[] { "AppRole", "AppRole" }).ToList();
        }

        public ICollection<Permission> GetByRoleId(string roleId)
        {
            return _permissionRepository.GetMulti(x => x.RoleId == roleId).ToList();
        }

        public ICollection<Permission> GetByUserId(string userId)
        {
            return _permissionRepository.GetByUserId(userId);
        }

        public void SaveChange()
        {
            _unitOfWork.Commit();
        }
    }
}
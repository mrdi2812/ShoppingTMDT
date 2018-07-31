using System.Collections.Generic;
using System.Linq;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IFunctionService
    {
        Function Create(Function function);

        IEnumerable<Function> GetAll(string filter);

        IEnumerable<Function> GetAllWithPermission(string userId);

        IEnumerable<Function> GetAllWithParentID(string parentId);

        Function GetById(string id);

        void Update(Function function);

        void Delete(string id);

        void Save();

        bool CheckExistedId(string id);
    }

    public class FunctionService : IFunctionService
    {
        private IFunctionsRepository _functionsRepository;
        private IUnitOfWork _unitOfWork;

        public FunctionService(IFunctionsRepository functionsRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _functionsRepository = functionsRepository;
        }

        public bool CheckExistedId(string id)
        {
            return _functionsRepository.CheckContains(x => x.ID == id);
        }

        public Function Create(Function function)
        {
            return _functionsRepository.Add(function);
        }

        public void Delete(string id)
        {
            var model = _functionsRepository.GetSingleByCondition(x => x.ID == id);
            _functionsRepository.Delete(model);
        }

        public Function GetById(string id)
        {
            return _functionsRepository.GetSingleByCondition(x => x.ID == id);
        }

        public IEnumerable<Function> GetAll(string filter)
        {
            var query = _functionsRepository.GetMulti(x => x.Status);
            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => x.Name.Contains(filter));
            return query.OrderBy(x => x.ParentId);
        }

        public IEnumerable<Function> GetAllWithParentID(string parentId)
        {
            return _functionsRepository.GetMulti(x => x.ParentId.Contains(parentId));
        }

        public IEnumerable<Function> GetAllWithPermission(string userId)
        {
            var query = _functionsRepository.GetListFunctionWithPermission(userId);
            return query.OrderBy(x => x.ParentId);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(Function function)
        {
            _functionsRepository.Update(function);
        }
    }
}
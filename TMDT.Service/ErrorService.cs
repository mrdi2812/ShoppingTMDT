using System;
using System.Collections.Generic;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IErrorService
    {
        Error Create(Error error);
        IEnumerable<Error> GetAll();
        void Save();
    }

    public class ErrorService : IErrorService
    {
        private IErrorRepository _errorRepository;
        private IUnitOfWork _unitOfWork;

        public ErrorService(IErrorRepository errorRepository, IUnitOfWork unitOfWork)
        {
            this._errorRepository = errorRepository;
            this._unitOfWork = unitOfWork;
        }

        public Error Create(Error error)
        {
            return _errorRepository.Add(error);
        }

        public IEnumerable<Error> GetAll()
        {
            return _errorRepository.GetAll();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
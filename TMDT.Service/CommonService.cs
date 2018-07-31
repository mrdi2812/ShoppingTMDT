using System;
using System.Collections.Generic;
using TMDT.Common;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface ICommonService
    {
        Footer GetFooter();
        Slide GetById(int id);
        Slide Add(Slide slide);
        void UpdateSlide(Slide slide);
        void DeleteSlide(int id);
        Footer AddFooter(Footer footer);
        void UpdateFooter(Footer footer);
        void DeleteFooter(int id);
        IEnumerable<Slide> GetSlides();
        IEnumerable<Slide> GetAllSlide();
        SystemConfig GetSystemConfig(string code);
        void Save();
    }

    public class CommonService : ICommonService
    {
        private IFooterRepository _footerRepository;
        private ISystemConfigRepository _systemConfigRepository;
        private IUnitOfWork _unitOfWork;
        private ISlideRepository _slideRepository;

        public CommonService(IFooterRepository footerRepository, ISystemConfigRepository systemConfigRepository, IUnitOfWork unitOfWork, ISlideRepository slideRepository)
        {
            _footerRepository = footerRepository;
            _systemConfigRepository = systemConfigRepository;
            _unitOfWork = unitOfWork;
            _slideRepository = slideRepository;
        }

        public Footer GetFooter()
        {
            return _footerRepository.GetSingleByCondition(x => x.ID == CommonConstants.DefaultFooterId);
        }

        public IEnumerable<Slide> GetSlides()
        {
            return _slideRepository.GetMulti(x => x.Status);
        }

        public SystemConfig GetSystemConfig(string code)
        {
            return _systemConfigRepository.GetSingleByCondition(x => x.Code == code);
        }

        public Slide Add(Slide slide)
        {
            return _slideRepository.Add(slide);
        }

        public void UpdateSlide(Slide slide)
        {
            _slideRepository.Update(slide);
        }

        public void DeleteSlide(int id)
        {
            _slideRepository.Delete(id);
        }

        public Footer AddFooter(Footer footer)
        {
            return _footerRepository.Add(footer);
        }

        public void UpdateFooter(Footer footer)
        {
            _footerRepository.Update(footer);
        }

        public void DeleteFooter(int id)
        {
            _footerRepository.Delete(id);
        }

        public IEnumerable<Slide> GetAllSlide()
        {
            return _slideRepository.GetAll();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public Slide GetById(int id)
        {
            return _slideRepository.GetSingleById(id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IAnnouncementService
    {
        Announcement Create(Announcement announcement);

        List<Announcement> GetListByUserId(string userId, int pageIndex, int pageSize, out int totalRow);

        List<Announcement> GetListByUserId(string userId, int top);
        int CountByUserId(string userId);
        void Delete(int notificationId);

        void MarkAsRead(string userId, int notificationId);

        Announcement GetDetail(int id);

        List<Announcement> GetListAll(int pageIndex, int pageSize, out int totalRow);

        List<Announcement> ListAllUnread(string userId, int pageIndex, int pageSize, out int totalRow);

        List<AnnouncementUser> ListByNotificationId(int notificationId);
        void DeleteAllUser(AnnouncementUser model);
        void Save();
    }

    public class AnnouncementService : IAnnouncementService
    {
        private IUnitOfWork _unitOfWork;
        private IAnnouncementRepository _announcementRepository;
        private IAnnouncementUserRepository _announcementUserRepository;

        public AnnouncementService(IUnitOfWork unitOfWork, IAnnouncementRepository announcementRepository, IAnnouncementUserRepository announcementUserRepository)
        {
            _unitOfWork = unitOfWork;
            _announcementRepository = announcementRepository;
            _announcementUserRepository = announcementUserRepository;
        }

        public Announcement Create(Announcement announcement)
        {
            return _announcementRepository.Add(announcement);
        }

        public void Delete(int notificationId)
        {
            _announcementRepository.Delete(notificationId);
        }

        public Announcement GetDetail(int id)
        {
            return _announcementRepository.GetSingleById(id);
        }

        public List<Announcement> GetListAll(int pageIndex, int pageSize, out int totalRow)
        {
            var query = _announcementRepository.GetAll(new string[] { "AppUser" });
            totalRow = query.Count();
            return query.OrderByDescending(x => x.CreatedDate)
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize).ToList();
        }

        public List<Announcement> GetListByUserId(string userId, int top)
        {
            var list = _announcementUserRepository.GetMulti(x=>x.UserId==userId);
            List<Announcement> model = new List<Announcement>();
            foreach(var item in list)
            {
                var detail = _announcementRepository.GetSingleById(item.AnnouncementId);
                model.Add(detail);
            }
            model = model.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
            return model;
        }

        public List<Announcement> GetListByUserId(string userId, int pageIndex, int pageSize, out int totalRow)
        {
            var query = _announcementRepository.GetMulti(x => x.UserId == userId);
            totalRow = query.Count();
            return query.OrderByDescending(x => x.CreatedDate)
                .Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }

        public List<Announcement> ListAllUnread(string userId, int pageIndex, int pageSize, out int totalRow)
        {
            var query = _announcementRepository.GetAllUnread(userId);
            totalRow = query.Count();
            return query.OrderByDescending(x => x.CreatedDate).Skip(pageSize * (pageSize - 1)).Take(pageSize).ToList();
        }

        public void MarkAsRead(string userId, int notificationId)
        {
            var announ = _announcementUserRepository.GetSingleByCondition(x => x.AnnouncementId == notificationId && x.UserId == userId);
            if (announ == null)
            {
                _announcementUserRepository.Add(new AnnouncementUser()
                {
                    AnnouncementId = notificationId,
                    UserId = userId,
                    HasRead = true
                });
            }
            else
            {
                announ.HasRead = true;
            }
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public List<AnnouncementUser> ListByNotificationId(int notificationId)
        {
            return _announcementUserRepository.GetMulti(x => x.AnnouncementId == notificationId).ToList();
        }

        public void DeleteAllUser(AnnouncementUser model)
        {
            _announcementUserRepository.Delete(model);
        }
        public int CountByUserId(string userId)
        {
            return _announcementUserRepository.GetMulti(x => x.UserId == userId && x.HasRead == false).Count();
        }
    }
}
using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IContactDetailService
    {
        ContactDetail Add(ContactDetail contactDetail);

        void Update(ContactDetail contactDetail);

        ContactDetail Delete(int id);

        ContactDetail GetDefaultContact();

        void Save();
    }

    public class ContactDetailService : IContactDetailService
    {
        private IUnitOfWork _unitOfWork;
        private IContactDetailRepository _contactDetailRepository;

        public ContactDetailService(IUnitOfWork unitOfWork, IContactDetailRepository contactDetailRepository)
        {
            _unitOfWork = unitOfWork;
            _contactDetailRepository = contactDetailRepository;
        }

        public ContactDetail Add(ContactDetail contactDetail)
        {
            return _contactDetailRepository.Add(contactDetail);
        }

        public void Update(ContactDetail contactDetail)
        {
            _contactDetailRepository.Update(contactDetail);
        }

        public ContactDetail Delete(int id)
        {
            return _contactDetailRepository.Delete(id);
        }

        public ContactDetail GetDefaultContact()
        {
            return _contactDetailRepository.GetSingleByCondition(x => x.Status);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
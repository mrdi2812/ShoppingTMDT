using TMDT.Data.Infrastructure;
using TMDT.Data.Repositories;
using TMDT.Model.Models;

namespace TMDT.Service
{
    public interface IFeedbackService
    {
        Feedback Add(Feedback feedBack);

        void Save();
    }

    public class FeedbackService : IFeedbackService
    {
        private IUnitOfWork _unitOfWork;
        private IFeedbackRepository _feedbackRepository;

        public FeedbackService(IUnitOfWork unitOfWork, IFeedbackRepository feedbackRepository)
        {
            _unitOfWork = unitOfWork;
            _feedbackRepository = feedbackRepository;
        }

        public Feedback Add(Feedback feedBack)
        {
            return _feedbackRepository.Add(feedBack);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
using AutoMapper;
using TMDT.Model.Models;
using TMDT.Service;
using System.Web.Mvc;
using TMDT.Web.Infrastructure.Extensions;
using BotDetect.Web.Mvc;
using TMDT.Common;
using TMDT.Web.Models.Common;

namespace TMDT.Web.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        IContactDetailService _contactDetailService;
        IFeedbackService _feedbackService;
        public ContactController(IContactDetailService contactDetailService,IFeedbackService feedbackService)
        {
            _contactDetailService = contactDetailService;
            _feedbackService = feedbackService;
        }
        public ActionResult Index()
        {
            FeedbackViewModel feedback = new FeedbackViewModel();
            feedback.ContactDetail = GetDetail();
            return View(feedback);
        }
        [HttpPost]
        [CaptchaValidation("CaptchaCode", "contactCaptcha", "Mã xác nhận không đúng")]
        public ActionResult SeedFeedback(FeedbackViewModel feedbackViewModel)
        {
            if (ModelState.IsValid)
            {
                Feedback newfeedBack = new Feedback();
                newfeedBack.UpdateFeedback(feedbackViewModel);
                _feedbackService.Add(newfeedBack);
                _feedbackService.Save();
                ViewData["SuccessMessage"] = "Gửi phản hồi thành công";


                string content = System.IO.File.ReadAllText(Server.MapPath("/Templates/contact_template.html"));
                content = content.Replace("{{Name}}",feedbackViewModel.Name);
                content = content.Replace("{{Email}}", feedbackViewModel.Email);
                content = content.Replace("{{Message}}", feedbackViewModel.Message);
                var adminEmail = ConfigHelper.GetByKey("AdminEmail");
                MailHelper.SendMail(adminEmail, "Thông tin liên hệ từ website",content);

                feedbackViewModel.Name = string.Empty;
                feedbackViewModel.Email = string.Empty;
                feedbackViewModel.Message = string.Empty;

            }
            feedbackViewModel.ContactDetail = GetDetail();
            
            return View("Index",feedbackViewModel);
        }
        private ContactDetailViewModel GetDetail()
        {
            var model = _contactDetailService.GetDefaultContact();
            var viewModel = Mapper.Map<ContactDetail, ContactDetailViewModel>(model);
            return viewModel;
        }
    }
}
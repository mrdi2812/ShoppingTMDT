using System.ComponentModel.DataAnnotations;

namespace TMDT.Web.Models.Common
{
    public class ContactDetailViewModel
    {
        public int ID { set; get; }

        [Required(ErrorMessage = "Tên không được trống")]
        public string Name { set; get; }

        [MaxLength(50, ErrorMessage = "Số điện thoại không vượt quá 50 ký tự")]
        public string Phone { set; get; }

        [MaxLength(250, ErrorMessage = "Email không vượt quá 50 ký tự")]
        public string Email { set; get; }

        [MaxLength(250, ErrorMessage = "Website không vượt quá 50 ký tự")]
        public string Website { set; get; }

        [MaxLength(250, ErrorMessage = "Địa chỉ không vượt quá 50 ký tự")]
        public string Adderss { set; get; }

        public string Other { set; get; }

        public string Lat { set; get; }

        public string Lng { set; get; }

        public bool Status { set; get; }
    }
}
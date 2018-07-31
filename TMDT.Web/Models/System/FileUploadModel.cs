using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMDT.Web.Models.System
{
    public class FileUploadModel
    {
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Chọn file ảnh đại diện.")]
        public string file { get; set; }
    }
}
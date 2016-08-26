using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieBooker.Models
{
    public class LoginM
    {
        [Required(ErrorMessage = "계정을 입력하세요")]
        [Display(Name = "사용자 계정")]
        public String ID { get; set; }

        [Required(ErrorMessage = "비밀번호를 입력하세요")]
        [Display(Name = "비밀번호")]
        public String Pass { get; set; }

    }
}
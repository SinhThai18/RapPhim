using System.ComponentModel.DataAnnotations;

namespace RapPhim3.ViewModel
{
    public class RegisterViewModel
    {
        [Required, Display(Name = "Họ và Tên")]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required, DataType(DataType.PhoneNumber), Display(Name = "Số Điện Thoại")]
        public string PhoneNumber { get; set; } = null!;

        [Required, DataType(DataType.Password), MinLength(6), Display(Name = "Mật Khẩu")]
        public string Password { get; set; } = null!;

        [Required, DataType(DataType.Password), Compare("Password"), Display(Name = "Nhập Lại Mật Khẩu")]
        public string ConfirmPassword { get; set; } = null!;
    }
}

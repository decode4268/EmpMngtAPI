namespace EmpMngtAPI.Model.RequestModel
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; } 
        public string EmailToken { get; set; } 
        public string NewPassword { get; set; } 
    }
}

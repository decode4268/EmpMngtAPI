using EmpMngtAPI.Model.RequestModel;

namespace EmpMngtAPI.UtilityService
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}

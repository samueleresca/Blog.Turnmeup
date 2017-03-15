using Microsoft.AspNetCore.Identity;

namespace Blog.Turnmeup.DL.Infrastructure.ErrorHandler
{
    public interface IErrorHandler
    {
        string GetMessage(ErrorMessagesEnum message);
        string ErrorIdentityResult(IdentityResult result);
    }


    public enum ErrorMessagesEnum
    {
        EntityNull = 1,
        ModelValidation = 2,
        AuthUserDoesNotExists = 3,
        AuthWrongCredentials = 4,
        AuthCannotCreate = 5,
        AuthCannotDelete = 6,
        AuthCannotUpdate = 7,
        AuthNotValidInformations = 8,
        AuthCannotRetrieveToken = 9
    }

}

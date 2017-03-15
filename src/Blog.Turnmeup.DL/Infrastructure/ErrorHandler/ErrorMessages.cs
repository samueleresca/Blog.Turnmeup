using System;
using Microsoft.AspNetCore.Identity;

namespace Blog.Turnmeup.DL.Infrastructure.ErrorHandler
{
    public class ErrorHandler : IErrorHandler
    {
        public string GetMessage(ErrorMessagesEnum message)
        {
            switch (message)
            {
                case ErrorMessagesEnum.EntityNull:
                    return "The entity passed is null {0}. Additional information: {1}";

                case ErrorMessagesEnum.ModelValidation:
                    return "The request data is not correct. Additional information: {0}";

                case ErrorMessagesEnum.AuthUserDoesNotExists:
                    return "The user doesn't not exists";

                case ErrorMessagesEnum.AuthWrongCredentials:
                    return "The email or password are wrong";

                case ErrorMessagesEnum.AuthCannotCreate:
                    return "Cannot create user";

                case ErrorMessagesEnum.AuthCannotDelete:
                    return "Cannot delete user";

                case ErrorMessagesEnum.AuthCannotUpdate:
                    return "Cannot update user";

                case ErrorMessagesEnum.AuthNotValidInformations:
                    return "Invalid informations";

                case ErrorMessagesEnum.AuthCannotRetrieveToken:
                    return "Cannot retrieve token";
                default:
                    throw new ArgumentOutOfRangeException(nameof(message), message, null);
            }

        }

        public string ErrorIdentityResult(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {

            }

            return string.Empty;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using AutoMapper;
using Blog.Turnmeup.API.Models.Users;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Infrastructure.ErrorHandler;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.DL.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Turnmeup.API.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;
        private readonly IErrorHandler _errorHandler;
        private readonly IUserValidator<AppUser> _userValidator;
        private readonly IPasswordValidator<AppUser> _passwordValidator;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly SignInManager<AppUser> _signInManager;

        public UsersController(IUsersService usersService, IErrorHandler errorHandler, IMapper mapper, IUserValidator<AppUser> userValidator, IPasswordValidator<AppUser> passwordValidator, IPasswordHasher<AppUser> passwordHasher, SignInManager<AppUser> signInManager)
        {
            _usersService = usersService;
            _errorHandler = errorHandler;
            _mapper = mapper;
            _userValidator = userValidator;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
            _signInManager = signInManager;
        }

        [HttpGet]
        public List<UserResponseModel> Get()
        {
            var returnedList = new List<UserResponseModel>();

            _usersService.Get().ToList().ForEach(u =>
            {
                returnedList.Add(_mapper.Map<AppUser, UserResponseModel>(u));
            });

            return returnedList;
        }

        [HttpPost("/api/[controller]/login")]
        public async Task<UserResponseModel> Login([FromBody]LoginRequestModel loginModel)
        {

            if (!ModelState.IsValid)
            {
                throw new HttpRequestException(string.Format(_errorHandler.GetMessage(ErrorMessagesEnum.ModelValidation), "", ModelState.Values.First().Errors.First().ErrorMessage));
            }

            var user = _usersService.GetByEmail(loginModel.Email);

            if (user == null)
                throw new HttpRequestException(_errorHandler.GetMessage(ErrorMessagesEnum.AuthUserDoesNotExists));

            await _signInManager.SignOutAsync();
            var result = await _signInManager.PasswordSignInAsync(
                user, loginModel.Password, false, false);

            if (result.Succeeded)
            {
                return _mapper.Map<AppUser, UserResponseModel>(user);
            }

            throw new HttpRequestException(_errorHandler.GetMessage(ErrorMessagesEnum.AuthWrongCredentials));
        }

        [HttpGet("{email}")]
        public UserResponseModel Get(string email)
        {
            return _mapper.Map<AppUser, UserResponseModel>(_usersService.GetByEmail(email));
        }


        [HttpPost("/api/[controller]/create")]
        public async Task<UserResponseModel> Create([FromBody]CreateRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpRequestException(string.Format(
                    _errorHandler.GetMessage(ErrorMessagesEnum.ModelValidation),
                    ModelState.Values.First().Errors.First().ErrorMessage));
            }
            var user = new AppUser
            {
                UserName = model.Name,
                Email = model.Email
            };

            var result = await _usersService.Create(user, model.Password);

            if (result.Succeeded)
            {
                return _mapper.Map<AppUser, UserResponseModel>(_usersService.GetByEmail(model.Email));
            }
            throw new HttpRequestException(_errorHandler.GetMessage(ErrorMessagesEnum.AuthCannotCreate));
        }


        [HttpPost("/api/[controller]/delete")]
        public async Task<UserResponseModel> Delete([FromBody] DeleteRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpRequestException(string.Format(
                    _errorHandler.GetMessage(ErrorMessagesEnum.ModelValidation),
                    ModelState.Values.First().Errors.First().ErrorMessage));
            }
            var user = _usersService.GetByEmail(model.Email);
            if (user == null)
                throw new HttpRequestException(_errorHandler.GetMessage(ErrorMessagesEnum.AuthUserDoesNotExists));


            var result = await _usersService.Delete(user);
            if (result.Succeeded)
            {
                return _mapper.Map<AppUser, UserResponseModel>(user);
            }

            throw new HttpRequestException(_errorHandler.GetMessage(ErrorMessagesEnum.AuthCannotDelete));
        }

        [HttpPost]
        public async Task<UserResponseModel> Edit(string email,
            string password)
        {
            var user = _usersService.GetByEmail(email);

            if (user == null) throw new HttpRequestException(_errorHandler.GetMessage(ErrorMessagesEnum.AuthUserDoesNotExists));

            user.Email = email;

            var validEmail = await _userValidator.ValidateAsync(_usersService.GetUserManager(), user);

            if (!validEmail.Succeeded)
            {
                _errorHandler.ErrorIdentityResult(validEmail);
            }

            IdentityResult validPass = null;

            if (!string.IsNullOrEmpty(password))
            {
                validPass = await _passwordValidator.ValidateAsync(_usersService.GetUserManager(),
                    user, password);
                if (validPass.Succeeded)
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user,
                        password);
                }
                else
                {
                    _errorHandler.ErrorIdentityResult(validPass);
                }
            }
            if (validPass != null && ((!validEmail.Succeeded || password == string.Empty || !validPass.Succeeded)))
                throw new HttpRequestException(_errorHandler.GetMessage(ErrorMessagesEnum.AuthNotValidInformations));

            var result = await _usersService.Update(user);

            if (result.Succeeded)
            {
                return _mapper.Map<AppUser, UserResponseModel>(user);
            }

            throw new HttpRequestException(_errorHandler.GetMessage(ErrorMessagesEnum.AuthCannotUpdate));

        }

        [HttpPost(), Produces("application/json")]
        public async Task<IActionResult> Token(TokenRequestModel request)
        {

            if (!ModelState.IsValid)
            {
                throw new HttpRequestException(string.Format(_errorHandler.GetMessage(ErrorMessagesEnum.ModelValidation), ModelState.Values.First().Errors.First().ErrorMessage));
            }

            var user = _usersService.GetByEmail(request.Username);
            if (user == null) throw new HttpRequestException(_errorHandler.GetMessage(ErrorMessagesEnum.AuthUserDoesNotExists));


            await _signInManager.SignOutAsync();
            var result = await _signInManager.PasswordSignInAsync(
                user, request.Password, false, false);

            if (!result.Succeeded) throw new HttpRequestException(_errorHandler.GetMessage(ErrorMessagesEnum.AuthCannotRetrieveToken));

            // Create a new ClaimsIdentity holding the user identity.
            var identity = new ClaimsIdentity(
                OpenIdConnectServerDefaults.AuthenticationScheme,
                OpenIdConnectConstants.Claims.Name, null);
            // Add a "sub" claim containing the user identifier, and attach
            // the "access_token" destination to allow OpenIddict to store it
            // in the access token, so it can be retrieved from your controllers.
            identity.AddClaim(OpenIdConnectConstants.Claims.Subject,
                "71346D62-9BA5-4B6D-9ECA-755574D628D8",
                OpenIdConnectConstants.Destinations.AccessToken);
            identity.AddClaim(OpenIdConnectConstants.Claims.Name, user.UserName,
                OpenIdConnectConstants.Destinations.AccessToken);
            // ... add other claims, if necessary.
            var principal = new ClaimsPrincipal(identity);
            // Ask OpenIddict to generate a new token and return an OAuth2 token response.
            return SignIn(principal, OpenIdConnectServerDefaults.AuthenticationScheme);
        }
    }
}


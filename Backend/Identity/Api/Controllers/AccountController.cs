using Api.Manager.Interface;
using Api.Models.Entites;
using Api.Models.IOptionModel;
using Api.Models.ViewModel.Account;
using Api.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtService _jwtService;
        private readonly IMapper _mapper;

        private readonly ILogger<AccountController> _logger;
        private readonly IAccountManager _accountManager;

        public AccountController(UserManager<User> userManager,
                                  RoleManager<IdentityRole> roleManager,
                                  SignInManager<User> signInManager,
                                  JwtService jwtService,
                                  IMapper mapper,

                                  IAccountManager accountManager,
                                  ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _mapper = mapper;

            _logger = logger;
            _accountManager = accountManager;
        }

        [Authorize]
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByNameAsync(email);
            var userViewModel = new UserViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = await _jwtService.CreateJWT(user)
            };

            return Ok(userViewModel);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null) return Unauthorized("Invalid username or password");

            if (user.EmailConfirmed == false) return BadRequest("Please confirm your email address first.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("User name or password invalid");

            var userViewModel = new UserViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = await _jwtService.CreateJWT(user)
            };

            return Ok(userViewModel);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (await _accountManager.CheckEmailExistsAsync(model.Email))
                    return BadRequest("Email already exists");

                var user = _mapper.Map<User>(model);

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded) return BadRequest(result.Errors);

                if (await _accountManager.SendConfirmEMailAsync(user))
                {
                    return Ok(new JsonResult(new { title = "Account Created", message = "Your account has been created, please confrim your email address" }));
                }

                return BadRequest("Failed to send email. Please contact admin");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to send email.");
            }

        }
        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel confirmEmailViewModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(confirmEmailViewModel.Email);
                if (user == null) return Unauthorized("This email has not registerd yet");

                if (user.EmailConfirmed == true) return BadRequest("Your email was confirmed before. Please login to your account");
                var decodeTokenBytes = WebEncoders.Base64UrlDecode(confirmEmailViewModel.Token);
                var decodedToken = Encoding.UTF8.GetString(decodeTokenBytes);
                var res=await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (res.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Email confirmed", message = "Your email address is confirmed. You can login now" }));
                }

                return BadRequest("Invalid token. Please try again");
            }
            catch (Exception ex)
            {
                return BadRequest("Something wrong");
            }


        }

        [HttpPost("resend-confirm-email/{email}")]
        public async Task<IActionResult> ResendConfirmEMail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return Unauthorized("This email has not registerd yet");

                if (user.EmailConfirmed == true) return BadRequest("Your email was confirmed before. Please login to your account");

                if (await _accountManager.SendConfirmEMailAsync(user))
                {
                    return Ok(new JsonResult(new { title = "Confirmation link sent", message = "Please confrim your email address" }));
                }

                return BadRequest("Failed to send email. Please contact admin");

            }
            catch (Exception ex)
            {
                return BadRequest("Something wrong");
            }


        }


        [HttpPost("forgot-email-password/{email}")]
        public async Task<IActionResult> forgotEmailOrPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return Unauthorized("This email has not registerd yet");

                if (user.EmailConfirmed == false) return BadRequest("Please confirm your email address first.");

                if (await _accountManager.SendEmailForgotEmailPassword(user))
                {
                    return Ok(new JsonResult(new { title = "Forgot email or password email sent", message = "Please check your email address" }));
                }

                return BadRequest("Failed to send email. Please contact admin");

            }
            catch (Exception ex)
            {
                return BadRequest("Something wrong");
            }


        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
                if (user == null) return Unauthorized("This email has not registerd yet");
                if(resetPasswordViewModel.Password!= resetPasswordViewModel.NewPassword) return BadRequest("password is not match");
                if (user.EmailConfirmed == false) return BadRequest("Please confirm your email address first.");

                var decodeTokenBytes = WebEncoders.Base64UrlDecode(resetPasswordViewModel.Token);
                var decodedToken = Encoding.UTF8.GetString(decodeTokenBytes);

                var res = await _userManager.ResetPasswordAsync(user, decodedToken,resetPasswordViewModel.NewPassword);
                if (res.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Password reset success", message = "Your password has been reset" }));
                }

                return BadRequest("Failed to send email. Please contact admin");

            }
            catch (Exception ex)
            {
                return BadRequest("Something wrong");
            }


        }
    }
}

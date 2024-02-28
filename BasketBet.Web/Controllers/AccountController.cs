using AutoMapper;
using BasketBet.EntityFramework.Entities;
using BasketBet.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BasketBet.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signIngManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AccountController(SignInManager<AppUser> signIngManager, UserManager<AppUser> userManager, IMapper mapper)
        {
            _signIngManager = signIngManager;
            _userManager = userManager;
            this._mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    Points = 300,
                    UserName = registerVM.Username,
                    Email = registerVM.Email,
                };
                var result = await _userManager.CreateAsync(user, registerVM.Password!);
                if(result.Succeeded)
                {
                    await _signIngManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(registerVM);
        }
    }
}

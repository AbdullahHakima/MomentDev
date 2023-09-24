using Bloggi.Contracts;
using Bloggi.Models;
using Bloggi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggi.Controllers
{
	
	public class UsersController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(IUnitOfWork unitOfWork,UserManager<User> userManager,SignInManager<User> signInManager)
		{
			_unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(IFormFile Image,[FromForm] UserSignUpViewModel AccountDetails)
		{
			if(ModelState.IsValid)
			{
				using var data = new MemoryStream();
				await AccountDetails.Image.CopyToAsync(data);
				var user = new User()
				{
					ProfileImageUrl = _unitOfWork.Users.GetImageUrl(data.ToArray()),
					Brief=AccountDetails.Brief,
					FirstName=AccountDetails.FirstName,
					LastName=AccountDetails.LastName,
					Email=AccountDetails.Email,
					UserName = AccountDetails.Email.Split('@')[0],
					ProfileImage=data.ToArray()
					

				};
				IdentityResult result = await _userManager.CreateAsync(user, AccountDetails.Password);
				if(result.Succeeded)
				{
					await _signInManager.SignInAsync(user, false);
					return RedirectToAction("CreatePost","Posts");
				}
				foreach (var error in result.Errors)
					ModelState.AddModelError("", error.Description);

            }
			return View(AccountDetails);
		}

		[HttpGet]
		public IActionResult SignIn(string ReturnUrl)
		{
			//ViewData["ReturnUrl"] = ReturnUrl;
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> SignIn(UserLoginViewModel accout,string ReturnUrl = "~/Home/Index")
		{
			if(ModelState.IsValid)
			{
				var user  = await _userManager.FindByNameAsync(accout.UserName.Split('@')[0]);
				if(user!=null)
				{
					var result = await _signInManager.PasswordSignInAsync(user, accout.Password, accout.RememberMe, false);
					if(result.Succeeded)
					{
						return LocalRedirect(ReturnUrl);
					}
					ModelState.AddModelError("", "Incorrect username or password!");
				}
				ModelState.AddModelError("", "Invaild Username ,password!");
				
			}
			return View(accout);
		}
		[HttpGet]
		public async Task<IActionResult> ViewAuthor([FromRoute] int id, [FromQuery]int pageIndex=1)
		{
			
			var user = await _unitOfWork.Users.FindOneAsync(u => u.Id == id, includes: new[] { "SocialLinks","Posts" });
			if (user == null)
				return NotFound($"No User with id:{id} was founded!");
			var userPosts=user.Posts.OrderByDescending(up=>up.CreatedOn);
			var postCount= userPosts.Count();
			decimal totalPages = Math.Ceiling((decimal)postCount / 5);
            ViewData["NumberOfPages"] = totalPages;
            ViewData["Index"] = pageIndex;
            var SocialNames=new List<string>();
			foreach(var social in user.SocialLinks)
				SocialNames.Add(social.Url);
			var paginatedPosts = _unitOfWork.Posts.ApplyPagination(userPosts, pageIndex, postCount,pageSize:5);
			var userView = new UserViewModel()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Brief = user.Brief,
				SocialLinks = SocialNames,
				Posts=user.Posts,
				ProfileImageUrl=user.ProfileImageUrl,
				UserPostsVM=new List<HomePostViewModel>()
			};
            foreach (var post in paginatedPosts)
            {
                var Converted = new HomePostViewModel()
                {
                    Brief = post.Brief,
                    Title = post.Title,
                    CreatedOn = post.CreatedOn.Date,
                    Id = post.Id,
                    ReadingTime = post.ReadingTime,
                    ImageUrl = post.ImageUrl,
                    Tags = (List<Tag>)post.Tags
                };
                userView.UserPostsVM.Add(Converted);
            }

            return View("Index",userView);
		}
		public async Task<IActionResult> SignOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index","Home");
		} 
		[HttpGet]
		public async Task<IActionResult> CreateUser()
		{
			var user = new CreateUserViewModel();
			return View(user);
		}
		[HttpPost]
		public async Task<IActionResult>CreateUser(IFormFile ProfileImage, [FromForm]CreateUserViewModel createdUser)
		{
			if(ModelState.IsValid)
			{

				using var data = new MemoryStream();
				await createdUser.ProfileImage.CopyToAsync(data);

				var user = new User()
				{
					FirstName = createdUser.FirstName,
					LastName = createdUser.LastName,
					Brief = createdUser.Brief,
					ProfileImageUrl = _unitOfWork.Users.GetImageUrl(data.ToArray()),
					SocialLinks=new List<SocialLinks>(),
                };
				if(createdUser.SocialLinks!=null)
				{
                    foreach (var social in createdUser.SocialLinks)
                    {
						var availableSocial = await _unitOfWork.SocialLinks.FindOneAsync(s => s.Name == social);
						if (availableSocial!=null)
						{
							user.SocialLinks.Add(availableSocial);
						}
                    }
                }
				await _unitOfWork.Users.CreateAsync(user);
				_unitOfWork.Complete();
                return RedirectToAction("Index", new {Id=user.Id });
            }
			return BadRequest(ModelState);
		}
		[HttpGet,Authorize]
		public async Task<IActionResult> AdminBoard()
		{
			var data = new AdminBoardViewModel()
			{
				posts=(List<Post>)await _unitOfWork.Posts.GetAllAsync(),
				tags=(List<Tag>) await _unitOfWork.Tags.GetAllAsync(),	
			};
			return View(data);
		}
		
		[HttpGet,Authorize]
		public async Task<IActionResult> EditUser(int id)
		{
			var user=await _unitOfWork.Users.FindOneAsync(u => u.Id == id, new[] {"SocialLinks"});
			if (user!=null)
			{
				var userView = new EditUserViewModel()
				{
					Brief = user.Brief,
					ProfileImageUrl = user.ProfileImageUrl,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Id = id,
					SocialLinks=new List<SocialLinksViewModel>()
				};
				if (user.SocialLinks.Count() > 0)
					foreach (var social in user.SocialLinks)
					{
						var socialView = new SocialLinksViewModel()
						{
							Name = social.Name,
							Url = social.Url,
							Id=social.Id,
						};
						userView.SocialLinks.Add(socialView);
					}
				return View(userView);
            }
			return NotFound();
		}
		[HttpPost]
		public async Task<IActionResult> EditUser(IFormFile ProfileImage, EditUserViewModel model)
		{
			if(ModelState.IsValid)
			{
				var oldUser = await _unitOfWork.Users.FindOneAsync(u => u.Id == model.Id, new []{ "SocialLinks" });
				using var data = new MemoryStream();
				await model.ProfileImage.CopyToAsync(data);
				if (oldUser != null)
				{
					oldUser.Brief = model.Brief;
					oldUser.ProfileImageUrl = _unitOfWork.Users.GetImageUrl(data.ToArray());
					oldUser.FirstName = model.FirstName;
					oldUser.LastName = model.LastName;
					oldUser.ProfileImage = data.ToArray();

					if (model.SocialLinks.Count() > 0)
						foreach (var social in model.SocialLinks)
						{   //MApping SocialLinks and ModelView
							//Add new Social
							var newSocial= new SocialLinks()
							{ 
								Name=social.Name,
								Url = social.Url
							};
							if (oldUser.SocialLinks.Any(s => s.Name != social.Name))
								oldUser.SocialLinks.Add(newSocial);
						}
					await _unitOfWork.Users.UpdateAsync(oldUser);
					_unitOfWork.Complete();
					return RedirectToAction("ViewAuthor", new { id = oldUser.Id });
				}
				return NotFound();
            }
			return View(model);
		}
	}
}

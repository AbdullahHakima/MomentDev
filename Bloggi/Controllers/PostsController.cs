using Bloggi.Contracts;
using Bloggi.Models;
using Bloggi.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bloggi.Controllers
{
    public class PostsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public PostsController(IUnitOfWork unitOfWork,IEmailService emailService)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> ViewPost([FromRoute] int id)
        {
            var post=await _unitOfWork.Posts.FindOneAsync(p => p.Id == id, new[] {"User","Tags"});
            if(post==null)
                return NotFound($"No post with id: {id} was founded!");
            var relatedposts = await _unitOfWork.Posts.GetAllRelatedToTag(post);

            var ViewPost = new PostViewModel()
            {
                Id = id,
                Brief = post.Brief,
                Content = post.Content,
                CreatedOn = post.CreatedOn.Date,
                ReadingTime = post.ReadingTime,
                Title = post.Title,
                UpdatedOn = post.UpdatedOn,
                ImageUrl = post.ImageUrlOutServer,
                Tags = (List<Tag>)post.Tags,
                User = post.User,
                UserImageUrl = post.User.ProfileImageUrl,
                Posts = relatedposts.Take(3).ToList(),
                RelatedPostCount = relatedposts.Count(),
        };

            return View(ViewPost);
        }
        [HttpGet]
        public async Task<IActionResult> RelatedPost(int id)
        {
            var post=await _unitOfWork.Posts.FindOneAsync(p => p.Id == id, new[] {"Tags"});
            if(post==null)
                return NotFound();
            var relatedPosts = await _unitOfWork.Posts.GetAllRelatedToTag(post);
            if (relatedPosts == null)
                return NotFound();
            var ViewList=new List<RelatedPostViewModel>();
            foreach (var related in relatedPosts)
            {
                var relatedPostsView = new RelatedPostViewModel()
                    {
                        Brief=related.Brief,
                        CreatedOn=related.CreatedOn,
                        ImageUrl=related.ImageUrlOutServer,
                        Title = related.Title,
                        Id = id,
                        ReadingTime=related.ReadingTime,
                };
                
                ViewList.Add(relatedPostsView);
            }
            ViewData["PostsCount"]=relatedPosts.Count();
            var postTags=new List<Tag>();
            foreach (var tag in post.Tags )
            {
                postTags.Add(tag);
            }
            ViewData["PostTags"] = postTags;
            return View(ViewList);

        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreatePost()
        {
            var post = new CreatePostViewModel();
            ViewData["Tags"]=await _unitOfWork.Tags.GetAllAsync();
           return View(post);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(IFormFile Image,[FromForm] CreatePostViewModel createdPost)
        {

            if(ModelState.IsValid)
            {
                using var dataStream=new MemoryStream();
                await createdPost.Image.CopyToAsync(dataStream);

                var post = new Post()
                {
                    Brief = createdPost.Brief,
                    Content = createdPost.Content,
                    Image = dataStream.ToArray(),
                    Title = createdPost.Title,
                    ReadingTime = createdPost.ReadingTime,
                    Tags = new List<Tag>(),
                    User = await _unitOfWork.Users.GetByIdAsync(1),
                    ImageUrlOutServer = createdPost.UploadedImageUrl
                };
                if (createdPost.SelectedTagIds != null)
                {
                    foreach (var tagId in createdPost.SelectedTagIds)
                    {
                        var Tag = await _unitOfWork.Tags.FindOneAsync(t=>t.Id==tagId);
                        if (Tag != null)
                            post.Tags.Add(Tag);
                    }

                }
                await _unitOfWork.Posts.CreateAsync(post);
                _unitOfWork.Complete();
                var subscribersList = _unitOfWork.Subscribers.GetAllAsync().GetAwaiter().GetResult().DistinctBy(s=>s.Email);
                var messagebody = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>New Post Published</title>\r\n    <style>\r\n        /* Responsive styles for mobile screens */\r\n        @media screen and (max-width: 600px) {{\r\n            table {{\r\n                width: 100% !important;\r\n            }}\r\n            img {{\r\n                max-width: 100% !important;\r\n                height: auto !important;\r\n            }}\r\n        }}\r\n    </style>\r\n</head>\r\n<body style=\"font-family: Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 0;\">\r\n\r\n    <!-- Email Container -->\r\n    <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"background-color: #f5f5f5;\">\r\n        <tr>\r\n            <td align=\"center\">\r\n                <table width=\"600\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"background-color: #ffffff; margin: 20px auto;\">\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding: 40px 0;\">\r\n                            <!-- Email Header (Logo) -->\r\n                            <img src=\"https://i.ibb.co/QK8MM6p/logo.png\" alt=\"Logo\">\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding: 20px;\">\r\n                            <!-- Email Content -->\r\n                            <h1>New Post Published!</h1>\r\n                            <p>We are excited to announce a new blog post:</p>\r\n                            <!-- Post Details -->\r\n                            <div class=\"card\">\r\n                                <a href=\"https://momentdeveloper.azurewebsites.net/Posts/ViewPost/{post.Id}\">\r\n                                    <img class=\"img-fluid\" src=\"{post.ImageUrlOutServer}\" alt=\"{post.Title}\" style=\"width: 100%;\">\r\n                                </a>\r\n                                <div class=\"card-block\">\r\n                                    <h2 class=\"card-title\">{post.Title}</h2>\r\n                                    <h4 class=\"card-text\">{post.Brief}</h4>\r\n                                    <div class=\"metafooter\">\r\n                                    </div>\r\n                                </div>\r\n                            </div>\r\n                            <!-- End Post Details -->\r\n                            <p>Read the full post by clicking <a href=\"https://momentdeveloper.azurewebsites.net/Posts/ViewPost/{post.Id}\">here.</a></p>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding: 10px;\">\r\n                            <!-- Email Footer -->\r\n                            <p>If you have any questions or need assistance, feel free to <a href=\"https://www.linkedin.com/in/abdullahhakimam\">contact us</a>.</p>\r\n                            <p>Stay tuned for more exciting updates!</p>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n\r\n</body>\r\n</html>\r\n";
                foreach (var subscriber in subscribersList)
                {
                    BackgroundJob.Enqueue(() => _emailService.SendEmail(subscriber.Email, "New Post Published!", messagebody));
                }
                
                
                return RedirectToAction("ViewPost",new {id=post.Id});
            }
            return BadRequest(ModelState);
            
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditPost([FromRoute]int id)
        {
            var post=await _unitOfWork.Posts.FindOneAsync(p=>p.Id==id);
            if (post != null)
            {
                
                var viewPost = new EditPostViewModel()
                {
                    Id = post.Id,
                    Brief = post.Brief,
                    Content = post.Content,
                    ReadingTime = post.ReadingTime,
                    Title = post.Title,
                    UploadedImageUrl=post.ImageUrlOutServer
                };
                ViewData["Tags"] = await _unitOfWork.Tags.GetAllAsync();
                return View(viewPost);
            }
            return BadRequest(ModelState);
        }
        [HttpPost]
        public async Task<IActionResult> EditPost(IFormFile Image,[FromForm] EditPostViewModel updatedPost)
        {
            var post =await _unitOfWork.Posts.FindOneAsync(p => p.Id == updatedPost.Id, new[] {"Tags"});
            if(post != null)
            if (ModelState.IsValid)
            {
                using var data = new MemoryStream();
                await updatedPost.Image.CopyToAsync(data);
                post.Brief=updatedPost.Brief;
                post.Content=updatedPost.Content;
                post.UpdatedOn = updatedPost.UpdateOn;
                post.Title = updatedPost.Title;
                post.Image=data.ToArray();
                post.ImageUrlOutServer = updatedPost.UploadedImageUrl;

                if (updatedPost.SelectedTagIds.Count() > 0)
                {
                    foreach (var tagId in updatedPost.SelectedTagIds)
                    {
                        var tag = await _unitOfWork.Tags.GetByIdAsync(tagId);
                        if (tag != null)
                            if (post.Tags.Any(t => t.Id != tagId))
                                post.Tags.Add(tag);
                    }
                }
                await _unitOfWork.Posts.UpdateAsync(post);
                _unitOfWork.Complete();
                return RedirectToAction("ViewPost", new {id=post.Id});
            }
            return BadRequest(ModelState);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post=await _unitOfWork.Posts.GetByIdAsync(id);
            if (post != null)
            {
               await _unitOfWork.Posts.DeleteAsync(post);
                _unitOfWork.Complete();
                return RedirectToAction("AdminBoard", "Users");
            }
            return BadRequest();
        }
        public async Task<IActionResult> Search(string content)
        {
            var result=new List<SearchResultViewModel>();
            if (!string.IsNullOrEmpty(content))
            {
                var searchContent = new SearchViewModel { Content = content };
                result = await _unitOfWork.Posts.search(searchContent);
                if (result != null)
                    return View(result);
            }
                
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> Subscribe(string subscriberEmail)
        {
            if(ModelState.IsValid)
            {
                
                var newSubscribe = new Subscriber()
                {
                    Email = subscriberEmail
                };

                await _unitOfWork.Subscribers.CreateAsync(newSubscribe);
                string emailmessage = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Thank You for Subscribing</title>\r\n</head>\r\n<body style=\"font-family: Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 0;\">\r\n\r\n    <!-- Email Container -->\r\n    <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"background-color: #f5f5f5;\">\r\n        <tr>\r\n            <td align=\"center\">\r\n                <table width=\"600\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"background-color: #ffffff; margin: 20px auto;\">\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding: 40px 0;\">\r\n                            <img src=\"https://i.ibb.co/QK8MM6p/logo.png\" alt=\"Logo\">\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding: 20px;\">\r\n                            <h1>Thank You for Subscribing!</h1>\r\n                            <p>We are excited to have you as a subscriber. You will receive our latest updates and blog posts directly in your inbox.</p>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding: 20px;\">\r\n                            <p>If you have any questions or need assistance, feel free to <a href=\"https://www.linkedin.com/in/abdullahhakimam\">contact us</a>.</p>\r\n                        </td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td align=\"center\" style=\"padding: 20px;\">\r\n                            <p>Stay tuned for our upcoming posts!</p>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n\r\n</body>\r\n</html>\r\n";
                BackgroundJob.Enqueue(
                     () =>  _emailService.SendEmail(newSubscribe.Email, "Thank You for Subscribing!", emailmessage));
                _unitOfWork.Complete();
            }
            return PartialView("_ThankYouSubscriberPartialView");
        }
      
    }
}

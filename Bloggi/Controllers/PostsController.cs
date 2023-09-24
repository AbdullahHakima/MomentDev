using Bloggi.Consts;
using Bloggi.Contracts;
using Bloggi.Models;
using Bloggi.Validations;
using Bloggi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Bloggi.Controllers
{
    public class PostsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostsController(IUnitOfWork unitOfWork)
        {
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
                ImageUrl = post.ImageUrl,
                Tags = (List<Tag>)post.Tags,
                User = post.User,
                UserImageUrl = post.User.ProfileImageUrl,
				Posts = relatedposts.Take(3).ToList(),
                RelatedPostCount=relatedposts.Count()
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
                        ImageUrl=related.ImageUrl,
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

                var post = new Post(dataStream.ToArray())
                {
                    Brief = createdPost.Brief,
                    Content = createdPost.Content,
                    Image = dataStream.ToArray(),
                    Title = createdPost.Title,
                    ReadingTime = createdPost.ReadingTime,
                    Tags = new List<Tag>(),
                    User = await _unitOfWork.Users.GetByIdAsync(1)
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
                post.ImageUrl= _unitOfWork.Posts.GetImageUrl(data.ToArray());
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
    }
}

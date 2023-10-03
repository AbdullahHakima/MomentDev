using Bloggi.Contracts;
using Bloggi.Models;
using Bloggi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggi.Controllers
{
	public class TagsController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public TagsController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		[HttpGet]
		public async Task<IActionResult> ViewTag([FromRoute] int id, [FromQuery] int pageIndex=1)
		{
			var tag=await _unitOfWork.Tags.FindOneAsync(t => t.Id == id, new[] {"Posts"});
			if (tag != null)
			{
                var TagPosts = tag.Posts.OrderByDescending(p => p.CreatedOn).ToList();
                var postTagCount= TagPosts.Count();
				var TotalPages = Math.Ceiling((decimal)postTagCount/5);
                ViewData["NumberOfPages"] = TotalPages;
                ViewData["Index"] = pageIndex;
                var paginatedPosts = _unitOfWork.Posts.ApplyPagination(TagPosts, pageIndex, postTagCount, 5);
                var tagView = new TagViewModel()
				{
					Description = tag.Description,
					Name = tag.Name,
					Posts = new List<TagPostViewModel>(),
					TagPostCount = postTagCount,
					Tags = (List<Tag>)await _unitOfWork.Tags.GetAllAsync()
				};
				if(tag.Posts.Count()>0)
                    foreach (var tagPost in paginatedPosts)
                    {
						var mappedPost = new TagPostViewModel()
						{
							Id = tagPost.Id,
							Brief = tagPost.Brief,
							Title = tagPost.Title,
							CreatedOn = tagPost.CreatedOn,
							ReadingTime = tagPost.ReadingTime,
							ImageUrl= tagPost.ImageUrlOutServer
						};
						tagView.Posts.Add(mappedPost);
                    }
				return View("Index", tagView);
			}
			return BadRequest();
		}
		[HttpGet]
		[Authorize]
		public IActionResult CreateTag()
		{
			var tag = new CreateTagViewModel();
			return View(tag);
		}
		[HttpPost]
		public async Task<IActionResult> CreateTag(CreateTagViewModel tag)
		{
			if(ModelState.IsValid)
			{
				var createdTag = new Tag()
				{
					Description = tag.Description,
					Name = tag.Name,
				};
				await _unitOfWork.Tags.CreateAsync(createdTag);
				_unitOfWork.Complete();
				return RedirectToAction("ViewTag", new {id=createdTag.Id});
			}
			return View(tag);
		}
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> EditTag(int id)
		{
			var tag = await _unitOfWork.Tags.GetByIdAsync(id);
			if(tag != null)
			{
				var tagView = new EditTagViewModel()
				{
					Description = tag.Description,
					Name = tag.Name,
				};
				return View(tagView);
			}
			return NotFound();
		}
		[HttpPost]
		public async Task<IActionResult> EditTag(int id, EditTagViewModel updatedtag)
		{
			if(ModelState.IsValid)
			{
				var oldTag=await _unitOfWork.Tags.GetByIdAsync(id);
				if (oldTag != null)
				{
					oldTag.Description = updatedtag.Description;
					oldTag.Name = updatedtag.Name;
					await _unitOfWork.Tags.UpdateAsync(oldTag);
					 _unitOfWork.Complete();
					return RedirectToAction("ViewTag", new { id = oldTag.Id });
				}

			}
			return View(updatedtag);
		}
		[HttpGet]
		[Authorize]
		public async Task<ActionResult> DeleteTag(int id)
		{
			var tag = await _unitOfWork.Tags.GetByIdAsync(id);
			if(tag != null)
			{
                try
                {
                    await _unitOfWork.Tags.DeleteAsync(tag);
                    _unitOfWork.Complete();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var Tag in ex.Entries)
                    {
                        Tag.Reload();
                        await _unitOfWork.Tags.DeleteAsync(tag);

                    }
                }
                return RedirectToAction("AdminBoard", "Users");
            }
            return NotFound();
		}
	}
}

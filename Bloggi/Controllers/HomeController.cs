using Bloggi.Consts;
using Bloggi.Contracts;
using Bloggi.Models;
using Bloggi.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Bloggi.Controllers;

public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery]int pageIndex=1)
    {
        var posts=await _unitOfWork.Posts.GetAllAsync(new[] {"Tags"},p=>p.CreatedOn,OrderByConst.Descending);
        var postCount=await _unitOfWork.Posts.CountAsync();
        decimal TotalPages = Math.Ceiling((decimal)postCount / 10);
        ViewData["NumberOfPages"] = TotalPages;
        ViewData["Index"] = pageIndex;
        List<HomePostViewModel> ViewPost=new List<HomePostViewModel>();
        var paginatedPost=_unitOfWork.Posts.ApplyPagination(posts, pageIndex, postCount,pageSize:10);
        foreach (var post in paginatedPost)
        {
            var Converted = new HomePostViewModel()
            {
                Brief = post.Brief,
                Title = post.Title,
                CreatedOn = post.CreatedOn.Date,
                Id = post.Id,
                ReadingTime = post.ReadingTime,
                ImageUrl = post.ImageUrlOutServer,
                Tags=(List<Tag>)post.Tags
            };
            ViewPost.Add(Converted);
        }
        return View("Index", ViewPost);
    }



}
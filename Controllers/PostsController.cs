using CachedPostsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CachedPostsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;

    public PostsController(IPostService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts()
    {
        var posts = await _service.GetPostsAsync();

        return Ok(posts);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPost(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "ID must be greater than zero."
            });
        }

        var post = await _service.GetPostByIdAsync(id);

        if (post == null)
        {
            return NotFound(new
            {
                message = $"Post with ID {id} was not found."
            });
        }

        return Ok(post);
    }
}
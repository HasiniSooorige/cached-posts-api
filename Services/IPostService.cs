using CachedPostsApi.Models;

namespace CachedPostsApi.Services;

public interface IPostService
{
    Task<List<Post>> GetPostsAsync();

    Task<Post?> GetPostByIdAsync(int id);
}
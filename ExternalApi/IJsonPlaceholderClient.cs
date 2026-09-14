using CachedPostsApi.Models;

namespace CachedPostsApi.ExternalApi;

public interface IJsonPlaceholderClient
{
    Task<List<Post>> GetPostsAsync();

    Task<Post?> GetPostByIdAsync(int id);
}
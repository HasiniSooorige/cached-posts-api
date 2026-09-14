using CachedPostsApi.Models;

namespace CachedPostsApi.Repositories;

public interface IPostRepository
{
    Task<List<Post>> GetAllAsync();

    Task<Post?> GetByIdAsync(int id);

    Task AddAsync(Post post);
}
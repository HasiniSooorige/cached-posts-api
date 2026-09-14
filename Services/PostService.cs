using CachedPostsApi.ExternalApi;
using CachedPostsApi.Models;
using CachedPostsApi.Repositories;

namespace CachedPostsApi.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repository;
    private readonly IJsonPlaceholderClient _externalApi;

    public PostService(
        IPostRepository repository,
        IJsonPlaceholderClient externalApi)
    {
        _repository = repository;
        _externalApi = externalApi;
    }

    public async Task<List<Post>> GetPostsAsync()
    {
        // 1. Check database first
        var cachedPosts = await _repository.GetAllAsync();

        if (cachedPosts.Count > 0)
        {
            return cachedPosts;
        }

        // 2. Nothing in database, call external API
        var externalPosts = await _externalApi.GetPostsAsync();

        // 3. Save external data into database
        foreach (var post in externalPosts)
        {
            await _repository.AddAsync(post);
        }

        // 4. Return data
        return externalPosts;
    }

    public async Task<Post?> GetPostByIdAsync(int id)
    {
        // 1. Check database first
        var cachedPost = await _repository.GetByIdAsync(id);

        if (cachedPost != null)
        {
            return cachedPost;
        }

        // 2. Not found in DB, call external API
        var externalPost = await _externalApi.GetPostByIdAsync(id);

        if (externalPost == null)
        {
            return null;
        }

        // 3. Save to DB
        await _repository.AddAsync(externalPost);

        // 4. Return
        return externalPost;
    }
}
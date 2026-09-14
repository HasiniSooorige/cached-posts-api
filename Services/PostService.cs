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
        var cachedPosts = await _repository.GetAllAsync();

        if (cachedPosts.Count == 0)
        {
            var externalPosts = await _externalApi.GetPostsAsync();

            foreach (var post in externalPosts)
            {
                await _repository.AddAsync(post);
            }

            return externalPosts;
        }

        var cachedIds = cachedPosts
            .Select(post => post.Id)
            .ToHashSet();

        var externalData = await _externalApi.GetPostsAsync();

        var missingPosts = externalData
            .Where(post => !cachedIds.Contains(post.Id))
            .ToList();

        foreach (var post in missingPosts)
        {
            await _repository.AddAsync(post);
        }

        return cachedPosts
            .Concat(missingPosts)
            .OrderBy(post => post.Id)
            .ToList();
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
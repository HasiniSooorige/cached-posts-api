using System.Net;
using System.Net.Http.Json;
using CachedPostsApi.Models;

namespace CachedPostsApi.ExternalApi;

public class JsonPlaceholderClient : IJsonPlaceholderClient
{
    private readonly HttpClient _httpClient;

    public JsonPlaceholderClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Post>> GetPostsAsync()
    {
        var posts = await _httpClient
            .GetFromJsonAsync<List<Post>>("posts");

        return posts ?? new List<Post>();
    }

    public async Task<Post?> GetPostByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"posts/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Post>();
    }
}
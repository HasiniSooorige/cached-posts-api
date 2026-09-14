using CachedPostsApi.Models;
using Microsoft.Data.SqlClient;

namespace CachedPostsApi.Repositories;

public class PostRepository : IPostRepository
{
    private readonly IConfiguration _configuration;

    public PostRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string ConnectionString =>
        _configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Database connection string is missing.");

    public async Task<List<Post>> GetAllAsync()
    {
        var posts = new List<Post>();

        const string sql = """
            SELECT Id, UserId, Title, Body, CreatedAt
            FROM Posts
            ORDER BY Id
            """;

        await using var connection = new SqlConnection(ConnectionString);
        await using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            posts.Add(new Post
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                Title = reader.GetString(2),
                Body = reader.GetString(3),
                CreatedAt = reader.GetDateTime(4)
            });
        }

        return posts;
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT Id, UserId, Title, Body, CreatedAt
            FROM Posts
            WHERE Id = @Id
            """;

        await using var connection = new SqlConnection(ConnectionString);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Post
        {
            Id = reader.GetInt32(0),
            UserId = reader.GetInt32(1),
            Title = reader.GetString(2),
            Body = reader.GetString(3),
            CreatedAt = reader.GetDateTime(4)
        };
    }

    public async Task AddAsync(Post post)
    {
        const string sql = """
            INSERT INTO Posts
                (Id, UserId, Title, Body)
            VALUES
                (@Id, @UserId, @Title, @Body)
            """;

        await using var connection = new SqlConnection(ConnectionString);
        await using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", post.Id);
        command.Parameters.AddWithValue("@UserId", post.UserId);
        command.Parameters.AddWithValue("@Title", post.Title);
        command.Parameters.AddWithValue("@Body", post.Body);

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }
}
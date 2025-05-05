using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ToDoApi;
using ToDoApi.Tests.Fixtures;
using Xunit;

namespace ToDoApi.Tests;

public class TodoApiTests : IClassFixture<TestContextFixture>
{
    private readonly HttpClient _client;

    public TodoApiTests(TestContextFixture factory)
    {
        _client = factory.TestClient;
    }

    [Fact]
    public async Task GetAllTodos_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/todoitems");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var todos = JsonSerializer.Deserialize<List<ToDo>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(todos);
    }

    [Fact]
    public async Task CreateTodo_ShouldReturnCreated()
    {
        var todo = new ToDo
        {
            Title = "Test",
            Description = "Test description",
            Expiry = DateTime.Today,
            CompletePercent = 0
        };

        var response = await _client.PostAsJsonAsync("/todoitems", todo);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ToDo>();
        Assert.NotNull(created);
        Assert.Equal("Test", created!.Title);
    }

    [Fact]
    public async Task GetTodoById_ShouldReturnTodo()
    {
        var todo = new ToDo
        {
            Title = "FindMe",
            Description = "To find",
            Expiry = DateTime.Today,
            CompletePercent = 0
        };
        var post = await _client.PostAsJsonAsync("/todoitems", todo);
        var created = await post.Content.ReadFromJsonAsync<ToDo>();

        var response = await _client.GetAsync($"/todoitems/{created!.Id}");

        response.EnsureSuccessStatusCode();
        var returned = await response.Content.ReadFromJsonAsync<ToDo>();
        Assert.NotNull(returned);
        Assert.Equal("FindMe", returned!.Title);
    }

    [Fact]
    public async Task UpdateTodo_ShouldReturnNoContent()
    {
        var todo = new ToDo { Title = "ToUpdate", Description = "Desc", Expiry = DateTime.Today, CompletePercent = 0 };
        var post = await _client.PostAsJsonAsync("/todoitems", todo);
        var created = await post.Content.ReadFromJsonAsync<ToDo>();

        created!.Title = "Updated";
        var response = await _client.PutAsJsonAsync($"/todoitems/{created.Id}", created);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_ShouldReturnNoContent()
    {
        var todo = new ToDo { Title = "ToDelete", Description = "", Expiry = DateTime.Today, CompletePercent = 0 };
        var post = await _client.PostAsJsonAsync("/todoitems", todo);
        var created = await post.Content.ReadFromJsonAsync<ToDo>();

        var response = await _client.DeleteAsync($"/todoitems/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task MarkAsDone_ShouldReturnOk()
    {
        var todo = new ToDo { Title = "ToMark", Description = "", Expiry = DateTime.Today, CompletePercent = 0, IsDone = false };
        var post = await _client.PostAsJsonAsync("/todoitems", todo);
        var created = await post.Content.ReadFromJsonAsync<ToDo>();

        var patch = await _client.PatchAsync($"/todoitems/{created!.Id}/done", null);
        var updated = await patch.Content.ReadFromJsonAsync<ToDo>();

        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
        Assert.True(updated!.IsDone);
    }

    [Fact]
    public async Task PatchCompletePercent_ShouldReturnOk()
    {
        var todo = new ToDo { Title = "Progress", Description = "", Expiry = DateTime.Today, CompletePercent = 50 };
        var post = await _client.PostAsJsonAsync("/todoitems", todo);
        var created = await post.Content.ReadFromJsonAsync<ToDo>();

        var patch = await _client.PatchAsync($"/todoitems/{created!.Id}", null);
        var updated = await patch.Content.ReadFromJsonAsync<ToDo>();

        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
        Assert.Equal(100, updated!.CompletePercent);
    }

    [Fact]
    public async Task GetTodosToday_ShouldReturnOkOrNotFound()
    {
        var response = await _client.GetAsync("/todoitems/today");
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
    }

    [Fact]
    public async Task GetTodosNextDay_ShouldReturnOkOrNotFound()
    {
        var response = await _client.GetAsync("/todoitems/nextday");
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
    }

    [Fact]
    public async Task GetTodosThisWeek_ShouldReturnOkOrNotFound()
    {
        var response = await _client.GetAsync("/todoitems/thisweek");
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
    }
}

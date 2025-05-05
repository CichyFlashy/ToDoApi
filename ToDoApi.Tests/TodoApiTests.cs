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

    // Constructor to initialize the HttpClient for the tests
    public TodoApiTests(TestContextFixture factory)
    {
        _client = factory.TestClient;
    }

    // Test to ensure that the "Get All Todos" endpoint returns a successful response
    [Fact]
    public async Task GetAllTodos_ShouldReturnOk()
    {
        // Act: Send GET request to retrieve all todos
        var response = await _client.GetAsync("/todoitems");

        // Assert: Ensure the response is successful (status code 200-299)
        response.EnsureSuccessStatusCode();

        // Deserialize the response content into a list of ToDo items
        var content = await response.Content.ReadAsStringAsync();
        var todos = JsonSerializer.Deserialize<List<ToDo>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert that the todos list is not null
        Assert.NotNull(todos);
    }

    // Test to ensure that an invalid ToDo (with no title) returns a BadRequest (400) response
    [Fact]
    public async Task CreateTodo_ShouldReturnBadRequest_WhenInvalidData()
    {
        // Arrange: Create an invalid ToDo object (no title)
        var todo = new ToDo
        {
            Title = "",
            Description = "Test task",
            Expiry = DateTime.Now.AddDays(1),
            CompletePercent = 50
        };

        // Act: Attempt to create the invalid todo item
        var response = await _client.PostAsJsonAsync("/todoitems", todo);

        // Assert: Expecting a 400 BadRequest response due to invalid data
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Test to ensure that creating a valid ToDo item returns a Created (201) response
    [Fact]
    public async Task CreateTodo_ShouldReturnCreated()
    {
        // Arrange: Create a valid ToDo object
        var todo = new ToDo
        {
            Title = "Test",
            Description = "Test description",
            Expiry = DateTime.Today,
            CompletePercent = 0
        };

        // Act: Attempt to create the valid todo item
        var response = await _client.PostAsJsonAsync("/todoitems", todo);

        // Assert: Ensure that the response status code is "Created" (201)
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Deserialize the response content to verify the created ToDo item
        var created = await response.Content.ReadFromJsonAsync<ToDo>();
        Assert.NotNull(created);
        Assert.Equal("Test", created!.Title);
    }

    // Test to ensure that retrieving a specific ToDo by ID returns the correct ToDo item
    [Fact]
    public async Task GetTodoById_ShouldReturnTodo()
    {
        // Arrange: Create a new ToDo item
        var todo = new ToDo
        {
            Title = "FindMe",
            Description = "To find",
            Expiry = DateTime.Today,
            CompletePercent = 0
        };

        // Post the ToDo item to create it in the database
        var post = await _client.PostAsJsonAsync("/todoitems", todo);
        var created = await post.Content.ReadFromJsonAsync<ToDo>();

        // Act: Retrieve the created ToDo item by its ID
        var response = await _client.GetAsync($"/todoitems/{created!.Id}");

        // Assert: Ensure the response is successful
        response.EnsureSuccessStatusCode();

        // Deserialize the response content and verify the retrieved ToDo item
        var returned = await response.Content.ReadFromJsonAsync<ToDo>();
        Assert.NotNull(returned);
        Assert.Equal("FindMe", returned!.Title);
    }

    // Test to ensure that updating a ToDo item returns a NoContent (204) response
    [Fact]
    public async Task UpdateTodo_ShouldReturnNoContent()
    {
        // Arrange: Create a new ToDo item to update
        var todo = new ToDo { Title = "ToUpdate", Description = "Desc", Expiry = DateTime.Today, CompletePercent = 0 };
        var post = await _client.PostAsJsonAsync("/todoitems", todo);
        var created = await post.Content.ReadFromJsonAsync<ToDo>();

        // Modify the ToDo item (update the title)
        created!.Title = "Updated";

        // Act: Send PUT request to update the ToDo item
        var response = await _client.PutAsJsonAsync($"/todoitems/{created.Id}", created);

        // Assert: Ensure the response status code is NoContent (204), meaning the update was successful
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // Test to ensure that deleting a ToDo item returns a NoContent (204) response
    [Fact]
    public async Task DeleteTodo_ShouldReturnNoContent()
    {
        // Arrange: Create a ToDo item to delete
        var todo = new ToDo { Title = "ToDelete", Description = "", Expiry = DateTime.Today, CompletePercent = 0 };
        var post = await _client.PostAsJsonAsync("/todoitems", todo);
        var created = await post.Content.ReadFromJsonAsync<ToDo>();

        // Act: Send DELETE request to delete the ToDo item
        var response = await _client.DeleteAsync($"/todoitems/{created!.Id}");

        // Assert: Ensure the response status code is NoContent (204), indicating the deletion was successful
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // Test to ensure that marking a ToDo as done returns a successful response
    [Fact]
    public async Task MarkAsDone_ShouldReturnOk()
    {
        // Arrange: Create a ToDo item and set IsDone to false
        var todo = new ToDo { Title = "ToMark", Description = "", Expiry = DateTime.Today, CompletePercent = 0, IsDone = false };
        var post = await _client.PostAsJsonAsync("/todoitems", todo);
        var created = await post.Content.ReadFromJsonAsync<ToDo>();

        // Act: Send PATCH request to mark the ToDo item as done
        var patch = await _client.PatchAsync($"/todoitems/{created!.Id}/done", null);
        var updated = await patch.Content.ReadFromJsonAsync<ToDo>();

        // Assert: Ensure the response status code is OK (200) and IsDone is true
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
        Assert.True(updated!.IsDone);
    }

    // Test to ensure that patching the CompletePercent field returns a successful response
    [Fact]
    public async Task PatchCompletePercent_ShouldReturnOk()
    {
        // Arrange: Create a ToDo item with a 50% complete status
        var todo = new ToDo { Title = "Progress", Description = "", Expiry = DateTime.Today, CompletePercent = 50 };
        var post = await _client.PostAsJsonAsync("/todoitems", todo);
        var created = await post.Content.ReadFromJsonAsync<ToDo>();

        // Act: Send PATCH request to set the complete percent to 100%
        var patch = await _client.PatchAsync($"/todoitems/{created!.Id}", null);
        var updated = await patch.Content.ReadFromJsonAsync<ToDo>();

        // Assert: Ensure the response status code is OK (200) and CompletePercent is 100
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
        Assert.Equal(100, updated!.CompletePercent);
    }

    // Test to ensure that retrieving todos for today returns either OK or NotFound
    [Fact]
    public async Task GetTodosToday_ShouldReturnOkOrNotFound()
    {
        // Act: Send GET request to retrieve todos for today
        var response = await _client.GetAsync("/todoitems/today");

        // Assert: The response should either be OK or NotFound
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
    }

    // Test to ensure that retrieving todos for the next day returns either OK or NotFound
    [Fact]
    public async Task GetTodosNextDay_ShouldReturnOkOrNotFound()
    {
        // Act: Send GET request to retrieve todos for the next day
        var response = await _client.GetAsync("/todoitems/nextday");

        // Assert: The response should either be OK or NotFound
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
    }

    // Test to ensure that retrieving todos for this week returns either OK or NotFound
    [Fact]
    public async Task GetTodosThisWeek_ShouldReturnOkOrNotFound()
    {
        // Act: Send GET request to retrieve todos for the current week
        var response = await _client.GetAsync("/todoitems/thisweek");

        // Assert: The response should either be OK or NotFound
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
    }
}

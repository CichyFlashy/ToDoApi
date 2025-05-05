using Microsoft.EntityFrameworkCore;
using ToDoApi;

//Build application
var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<TodoDb>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    builder.Services.AddDbContext<TodoDb>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

// Apply database migrations (Ensure the database is up-to-date)
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<TodoDb>();
if (dbContext.Database.IsRelational())
{
    dbContext.Database.Migrate();
}

// Get all todos
app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());

//Get Specific Todo
app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is ToDo todo
            ? Results.Ok(todo)
            : Results.NotFound());

//Get Incoming ToDo(for today)
app.MapGet("/todoitems/today", async (TodoDb db) =>
{
    var today = DateTime.Today;
    var todosToday = await db.Todos.Where(t => t.Expiry.Date == today).ToListAsync();

    return todosToday.Any() ? Results.Ok(todosToday) : Results.NotFound();
});

//Get Incoming ToDo(for next day)
app.MapGet("/todoitems/nextday", async (TodoDb db) =>
{
    var nextDay = DateTime.Today.AddDays(1);
    var todosNextDay = await db.Todos.Where(t => t.Expiry.Date == nextDay).ToListAsync();

    return todosNextDay.Any() ? Results.Ok(todosNextDay) : Results.NotFound();
});

//Get Incoming ToDo(for this week)
app.MapGet("/todoitems/thisweek", async (TodoDb db) =>
{
    var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);  
    var endOfWeek = startOfWeek.AddDays(6); 

    var todosThisWeek = await db.Todos.Where(t => t.Expiry.Date >= startOfWeek && t.Expiry.Date <= endOfWeek).ToListAsync();

    return todosThisWeek.Any() ? Results.Ok(todosThisWeek) : Results.NotFound();
});

//Create todo
app.MapPost("/todoitems", async (ToDo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

//Update todo
app.MapPut("/todoitems/{id}", async (int id, ToDo updatedTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = updatedTodo.Title;
    todo.Description = updatedTodo.Description;
    todo.Expiry = updatedTodo.Expiry;
    todo.CompletePercent = updatedTodo.CompletePercent;

    await db.SaveChangesAsync();
    return Results.NoContent();
});


//Set todo percent complete
app.MapPatch("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.CompletePercent = 100;
    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

//Delete todo
app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is ToDo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

//Mark todo as done
app.MapPatch("/todoitems/{id}/done", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.IsDone = true;
    await db.SaveChangesAsync();
    return Results.Ok(todo);
});


app.Run();

public partial class Program { }
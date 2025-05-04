using Microsoft.EntityFrameworkCore;
using ToDoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is ToDo todo
            ? Results.Ok(todo)
            : Results.NotFound());

app.MapGet("/todoitems/today", async (TodoDb db) =>
{
    var today = DateTime.Today;
    var todosToday = await db.Todos.Where(t => t.Expiry.Date == today).ToListAsync();

    return todosToday.Any() ? Results.Ok(todosToday) : Results.NotFound();
});

app.MapGet("/todoitems/nextday", async (TodoDb db) =>
{
    var nextDay = DateTime.Today.AddDays(1);
    var todosNextDay = await db.Todos.Where(t => t.Expiry.Date == nextDay).ToListAsync();

    return todosNextDay.Any() ? Results.Ok(todosNextDay) : Results.NotFound();
});

app.MapGet("/todoitems/thisweek", async (TodoDb db) =>
{
    var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);  
    var endOfWeek = startOfWeek.AddDays(6); 

    var todosThisWeek = await db.Todos.Where(t => t.Expiry.Date >= startOfWeek && t.Expiry.Date <= endOfWeek).ToListAsync();

    return todosThisWeek.Any() ? Results.Ok(todosThisWeek) : Results.NotFound();
});

app.MapPost("/todoitems", async (ToDo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

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


app.MapPatch("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.CompletePercent = 100;
    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

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

app.Run();

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



app.Run();

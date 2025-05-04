using Microsoft.EntityFrameworkCore;
using ToDoApi;

class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

    public DbSet<ToDo> Todos => Set<ToDo>();
}

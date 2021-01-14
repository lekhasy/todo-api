using System;

namespace todo_api
{
    public record Todo(string Id, DateTime CreatedDate, DateTime? CompletedDate, string TaskName, bool IsFavorite, bool IsCompleted, string User);
}

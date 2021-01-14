using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace todo_api.Controllers
{

    public record AddTodoViewModel(string User, string TaskName);

    public record ChangeTaskCompleteStateViewModel(string TaskId, bool IsCompleted);

    public record ChangeTaskFavoriteStateViewModel(string TaskId, bool IsFavorite);

    public record RequestResult<T>(T Data, string Message = "");

    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private static List<Todo> todos = new(MockData.GenerateData());

        private static object todoLock = new();

        [HttpGet("[action]")]
        public RequestResult<IEnumerable<Todo>> GetTodos(string user)
        {

            return AccqiureTodoList(() =>
            {
                if (string.IsNullOrEmpty(user))
                {
                    throw new ArgumentNullException("user không được phép trống");
                }
                return new RequestResult<IEnumerable<Todo>>(todos.Where(t => t.User == user));
            });
        }

        [HttpPost("[action]")]
        public RequestResult<Todo> AddTodo(AddTodoViewModel viewModel)
        {
            return AccqiureTodoList(() =>
            {
                if (string.IsNullOrEmpty(viewModel.User))
                {
                    throw new ArgumentNullException("user không được phép trống");
                }

                Todo newTodo = new(
                    Id: Guid.NewGuid().ToString(),
                    TaskName: viewModel.TaskName,
                    IsCompleted: false,
                    IsFavorite: false,
                    User: viewModel.User,
                    CreatedDate: DateTime.UtcNow,
                    CompletedDate: null
                );

                todos = todos.Select(t => t).ToList();
                todos.Add(newTodo);

                return new RequestResult<Todo>(newTodo);
            });
        }

        [HttpPost("[action]")]
        public RequestResult<bool> ChangeTaskCompletedState(ChangeTaskCompleteStateViewModel viewModel)
        {
            return AccqiureTodoList(() =>
            {
                var match = todos.SingleOrDefault(t => t.Id == viewModel.TaskId);
                if (match != null)
                {
                    var modifiedRecord = match with { IsCompleted = viewModel.IsCompleted };
                    if (modifiedRecord.IsCompleted)
                    {
                        modifiedRecord = modifiedRecord with { CompletedDate = DateTime.UtcNow };
                    }

                    todos = todos.Select(t => t.Id == viewModel.TaskId ? modifiedRecord : t).ToList();

                    return new RequestResult<bool>(true);
                }

                return new RequestResult<bool>(false, $"Không tìm thấy todo với Id {viewModel.TaskId}");
            });
        }


        [HttpPost("[action]")]
        public RequestResult<bool> ChangeTaskFavoriteState(ChangeTaskFavoriteStateViewModel viewModel)
        {
            return AccqiureTodoList(() =>
            {
                var match = todos.SingleOrDefault(t => t.Id == viewModel.TaskId);
                if (match != null)
                {
                    if (match.IsCompleted)
                    {
                        return new RequestResult<bool>(false, "Không thể thay đổi favorite với task đã hoàn thành");
                    }

                    var modifiedRecord = match with { IsFavorite = viewModel.IsFavorite };

                    todos = todos.Select(t => t.Id == viewModel.TaskId ? modifiedRecord : t).ToList();

                    return new RequestResult<bool>(true);
                }

                return new RequestResult<bool>(false, $"Không tìm thấy todo với Id {viewModel.TaskId}");
            });
        }

        T AccqiureTodoList<T>(Func<T> cb)
        {
            T result = default(T);
            lock (todoLock)
            {
                result = cb();
            };
            return result;
        }

    }

}
using System;
using System.Collections.Generic;
using System.Linq;

namespace todo_api
{
    public static class MockData
    {
        public static IEnumerable<Todo> GenerateData()
        {
            var dataForOneUser = new Todo[] {
                new(
                  Id: "",
                  CreatedDate: DateTime.Parse("2021-01-14T05:57:32.319456Z"),
                  CompletedDate: null,
                  TaskName: "Mua gạo",
                  IsFavorite: false,
                  IsCompleted: false,
                  User: ""
                ),
                new(
                  Id: "",
                  CreatedDate: DateTime.Parse("2021-01-14T05:57:44.440391Z"),
                  CompletedDate: null,
                  TaskName: "Làm bài tập",
                  IsFavorite: false,
                  IsCompleted: true,
                  User: ""
                ),
                new(
                  Id: "",
                  CreatedDate: DateTime.Parse("2021-01-14T05:57:20.660233Z"),
                  CompletedDate: null,
                  TaskName: "Lau nhà",
                  IsFavorite: false,
                  IsCompleted: false,
                  User: ""
                )
            };

            var users = new string[] { "sylk", "vu" };

            return users.SelectMany(u => dataForOneUser.Select(t => t with { User = u, Id = Guid.NewGuid().ToString() }));
        }
    }
}

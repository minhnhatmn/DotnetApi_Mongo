using MongoDB.Bson;

namespace ApiTodo.Model
{
    public class TodoItem
    {
        public ObjectId Id { get; set; }
        public string? Content { get; set; }
        public string? Color { get; set; }
        public int? Order { get; set; }
    }
}

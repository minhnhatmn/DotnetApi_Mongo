using MongoDB.Bson;

namespace ApiTodo.Model
{
    public class Color
    {
        public ObjectId Id { get; set; }
        public string? color { get; set; }
    }
}

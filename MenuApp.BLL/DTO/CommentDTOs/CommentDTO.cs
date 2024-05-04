using MenuApp.BLL.DTO.UserDTOs;

namespace MenuApp.BLL.DTO.CommentDTOs
{
    public class CommentDTO
    {
        public required string Id { get; set; }
        public required string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required UserDTO User { get; set; }
        public bool IsOwner { get; set; }
    }
}

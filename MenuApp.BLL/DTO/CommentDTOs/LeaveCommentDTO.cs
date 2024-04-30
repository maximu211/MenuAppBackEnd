using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.DTO.CommentDTOs
{
    public class LeaveCommentDTO
    {
        public required string Comment { get; set; }
        public required string RecipeId { get; set; }
    }
}

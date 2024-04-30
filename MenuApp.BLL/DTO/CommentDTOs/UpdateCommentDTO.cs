using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.DTO.CommentDTOs
{
    public class UpdateCommentDTO
    {
        public required string CommentId { get; set; }
        public required string Comment { get; set; }
    }
}

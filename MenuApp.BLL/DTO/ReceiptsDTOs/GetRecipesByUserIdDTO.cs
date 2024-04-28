using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp.BLL.DTO.RecipesDTOs
{
    public class GetRecipesByUserIdDTO
    {
        public required string UserId { get; set; }
    }
}

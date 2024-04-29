using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MenuApp.BLL.DTO.RecipesDTOs
{
    public class DeleteRecipeDTO
    {
        public required string RecipeId { get; set; }
    }
}

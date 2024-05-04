using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.BLL.DTO.RecipesDTOs;
using MenuApp.BLL.DTO.UserDTOs;

namespace MenuApp.BLL.Services
{
    public class SearchServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<UserDTO>? Users { get; set; }
        public List<CardRecipeDTO>? Recipes { get; set; }

        public SearchServiceResult(
            bool success,
            string message,
            List<CardRecipeDTO>? recipes = null,
            List<UserDTO>? users = null
        )
        {
            Success = success;
            Message = message;
            Recipes = recipes;
            Users = users;
        }
    }
}

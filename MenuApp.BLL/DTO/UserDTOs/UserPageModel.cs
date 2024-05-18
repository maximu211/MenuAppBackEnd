using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuApp.BLL.DTO.RecipesDTOs;

namespace MenuApp.BLL.DTO.UserDTOs
{
    public class UserPageModel
    {
        public required UserDTO User { get; set; }
        public required List<CardRecipeDTO> CardRecipes { get; set; }
        public required int SubscribedUsersCount { get; set; }
        public required int SubscribedToCount { get; set; }
        public bool IsOwner { get; set; }
        public bool IsSubscribed { get; set; }
    }
}

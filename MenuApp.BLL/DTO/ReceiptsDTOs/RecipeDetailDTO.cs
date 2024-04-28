using MenuApp.DAL.Models.EntityModels;

namespace MenuApp.BLL.DTO.ReceiptsDTOs
{
    public class RecipeDetailDTO
    {
        public required List<RecipeDescriptionElement> RecipeDescriptionElements { get; set; }
        public required List<string> RecipeIngradients { get; set; }
    }
}

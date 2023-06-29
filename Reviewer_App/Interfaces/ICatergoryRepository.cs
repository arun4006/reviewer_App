using Reviewer_App.Models;

namespace Reviewer_App.Interfaces
{
    public interface ICatergoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int id);
        ICollection<Pokemon> GetPokemonsByCategory(int categoryId);
        bool CategoryExists(int id);

        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(Category category);
        bool Save();
    }
}

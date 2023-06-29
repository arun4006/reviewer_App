using Reviewer_App.Models;

namespace Reviewer_App.Interfaces
{
    public interface IOwnerRepository
    {
        ICollection<Owner> GetOwners();
        Owner GetOwner(int id);
        ICollection<Owner> GetOwnerofAPokemon(int pokemon_id);
        ICollection<Pokemon> GetPokemonByOwner(int id);
        bool IsOwnerExist(int id);
        bool CreateOwner(Owner owner);
        bool UpdateOwner(Owner owner);
        bool DeleteOwner(Owner owner);
        bool Save();

    }
}

using Reviewer_App.Models;

namespace Reviewer_App.Interfaces
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemonById(int Pokemonid);
        Pokemon GetPokemon(string name);
        Decimal GetPokemonRating(int Pokemonid);
        bool PokemonExist(int Pokemonid);

        bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
        bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon);
        bool DeletePokemon(Pokemon pokemon);
        bool Save();
    }
}

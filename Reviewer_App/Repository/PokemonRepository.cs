using Reviewer_App.Data;
using Reviewer_App.Interfaces;
using Reviewer_App.Models;

namespace Reviewer_App.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;

        public PokemonRepository(DataContext context)
        {
            _context = context;
        }
        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemons.OrderBy(i => i.Id).ToList();
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemons.Where(p => p.Name == name).FirstOrDefault();
        }

        public Pokemon GetPokemonById(int Pokemonid)
        {
            return _context.Pokemons.Where(p => p.Id == Pokemonid).FirstOrDefault();
        }

        public decimal GetPokemonRating(int Pokemonid)
        {
            var review=_context.Reviews.Where(p=>p.Pokemon.Id == Pokemonid);
            if (review.Count()<= 0)
            {
                return 0;
            }
            return ((decimal)review.Sum(r=>r.Rating)/review.Count());
        }

       
        public bool PokemonExist(int Pokemonid)
        {
           return _context.Pokemons.Any(p => p.Id == Pokemonid);  
            //return true;
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.Where(a => a.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(a => a.Id == categoryId).FirstOrDefault();

            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon,
            };

            _context.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon,
            };

            _context.Add(pokemonCategory);

            _context.Add(pokemon);

            return Save();
        }

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            //if (ownerId != 0)
            //{
            //    PokemonOwner obj=new PokemonOwner{
            //        PokemonId = pokemon.Id,
            //        OwnerId=ownerId
            //    };

            //    _context.PokemonOwners.Update(obj);
            //    return Save();
            //}
            _context.Update(pokemon);
            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}

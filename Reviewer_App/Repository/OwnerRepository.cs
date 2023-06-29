using Reviewer_App.Data;
using Reviewer_App.Interfaces;
using Reviewer_App.Models;

namespace Reviewer_App.Repository
{
    public class OwnerRepository: IOwnerRepository
    {
        private readonly DataContext _context;

        public OwnerRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateOwner(Owner owner)
        {
           _context.Add(owner);
           return Save(); 
        }

        public bool DeleteOwner(Owner owner)
        {
            _context.Remove(owner);
            return Save();
        }

        public Owner GetOwner(int id)
        {
            return _context.Owners.Where(o => o.Id == id).FirstOrDefault();        }

        public ICollection<Owner> GetOwnerofAPokemon(int pokemon_id)
        {
            return _context.PokemonOwners.Where(o => o.Pokemon.Id == pokemon_id)
                .Select(c => c.Owner).ToList();
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.ToList();
        }

        public ICollection<Pokemon> GetPokemonByOwner(int id)
        {
            return _context.PokemonOwners.Where(o => o.Owner.Id == id)
                 .Select(c => c.Pokemon).ToList();
        }

        public bool IsOwnerExist(int id)
        {
            return _context.Owners.Any(p => p.Id == id);
        }

        public bool Save()
        {

            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateOwner(Owner owner)
        {
            _context.Update(owner);
            return Save();
        }
    }
}

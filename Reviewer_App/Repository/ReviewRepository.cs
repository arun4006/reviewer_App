using AutoMapper;
using Reviewer_App.Data;
using Reviewer_App.Interfaces;
using Reviewer_App.Models;

namespace Reviewer_App.Repository
{
    public class ReviewRepository: IReviewRepository
    {
        private readonly DataContext _context;
        

        public ReviewRepository(DataContext context)
        {
            _context = context;
          
        }

        public bool CreateReview(Review review)
        {
            _context.Add(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _context.Remove(review);
            return Save();
        }

        public bool DeleteReviews(List<Review> reviews)
        {
            _context.RemoveRange(reviews);
            return Save();
        }

        public Review GetReview(int id)
        {
            return _context.Reviews.Where(i => i.Id == id).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.ToList();
        }

        public ICollection<Review> GetReviewsByPokemon(int pokemonId)
        {
            return _context.Reviews.Where(i => i.Pokemon.Id == pokemonId).ToList();
        }

        public bool IsReviewExist(int id)
        {
            return _context.Reviews.Any(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReview(Review review)
        {
            _context.Update(review);
            return Save();
        }
    }
}

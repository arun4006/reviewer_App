using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reviewer_App.Data;
using Reviewer_App.Interfaces;
using Reviewer_App.Models;

namespace Reviewer_App.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReviewerRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            _context.Add(reviewer);
            return Save();
        }

        public bool DeleteReviewer(Reviewer reviewer)
        {  _context.Remove(reviewer);
            return Save();
        }

        public Reviewer GetReviewer(int id)
        {
            return _context.Reviewers.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return _context.Reviewers.ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int id)
        {
            return _context.Reviews.Where(i => i.Reviewer.Id == id).ToList();
        }

        public bool IsReviewerExist(int id)
        {
            return _context.Reviewers.Any(i => i.Id == id);
        }

        public bool Save()
        {

            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            _context.Update(reviewer);
            return Save();
        }
    }
}

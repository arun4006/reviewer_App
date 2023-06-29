using Reviewer_App.Models;

namespace Reviewer_App.Interfaces
{
    public interface IReviewerRepository
    {
        ICollection<Reviewer> GetReviewers();

        Reviewer GetReviewer(int id);

        bool IsReviewerExist(int id);

        ICollection<Review> GetReviewsByReviewer(int id);

        bool CreateReviewer(Reviewer reviewer);
        bool UpdateReviewer(Reviewer reviewer);
        bool DeleteReviewer(Reviewer reviewer);
        bool Save();
    }
}

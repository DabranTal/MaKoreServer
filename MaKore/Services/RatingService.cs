
using MaKore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaKore.Services
{
    public class RatingService : IRatingService
    {
        private readonly MaKoreContext _context;


        public RatingService(MaKoreContext context)
        {
            _context = context;
        }

        public void Create(Rating rating)
        {
            _context.Add(rating);
            _context.SaveChanges(); 
        }

        public void Delete(int id)
        {
            var rating = _context.Rating.Find(id);
            _context.Rating.Remove(rating);
            _context.SaveChanges();
        }

        public void Edit(Rating rating)
        {
            _context.Update(rating);
            _context.SaveChanges();
        }

        public List<Rating> Get()
        {
            return _context.Rating.ToList();
        }

        public Rating GetByIndex(int id)
        {
            return _context.Rating.FirstOrDefault(m => m.ID == id);
        }

        public List<Rating> Search(string query)
        {
            var search = _context.Rating.Where(rating => rating.Name.Contains(query));
            if (search.Any())
            {
                return search.ToList();
            }
            return null;
        }
    }
}
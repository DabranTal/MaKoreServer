using MaKore.JsonClasses;
using MaKore.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaKore.Services
{
    public interface IRatingService

    {
        public List<Rating> Get();

        public Rating GetByIndex(int id);

        public void Create(Rating rating);

        public List<Rating> Search(string query);

        public void Edit(Rating rating);

        public void Delete(int id);
    }
}

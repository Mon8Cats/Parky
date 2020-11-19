using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _db;

        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool CreateTrail(Trail trail)
        {
            _db.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _db.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int id)
        {
            return _db.Trails.Include(x=>x.NationalPark).FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Trail> GetTrails()
        {
            return _db.Trails.Include(x=>x.NationalPark).OrderBy(x => x.Name).ToList();
        }

        public ICollection<Trail> GetTrailsInNationalPark(int parkId)
        {
            return _db.Trails.Include(x => x.NationalPark).Where(x => x.NationalParkId == parkId).ToList();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool TrailExists(string name)
        {
            return _db.Trails.Any(x => x.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool TrailExists(int id)
        {
            return _db.Trails.Any(x => x.Id == id);
        }

        public bool UpdateTrail(Trail trail)
        {
            _db.Trails.Update(trail);
            return Save();
        }
    }
}

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
    public class NationalParkRepository : INationalParkRepository
    {
        private readonly ApplicationDbContext _db;

        public NationalParkRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool CreateNationalPark(NationalPark park)
        {
            _db.NationalParks.Add(park);
            return Save();
        }

        public bool DeleteNationalPark(NationalPark park)
        {
            _db.NationalParks.Remove(park);
            return Save();
        }

        public NationalPark GetNationalPark(int id)
        {
            return _db.NationalParks.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<NationalPark> GetNationalParks()
        {
            return _db.NationalParks.OrderBy(x => x.Name).ToList();
        }

        public bool NationalParkExists(string name)
        {
            return _db.NationalParks.Any(x => x.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool NationalParkExists(int id)
        {
            return _db.NationalParks.Any(x => x.Id == id);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateNationalPark(NationalPark park)
        {
            _db.NationalParks.Update(park);
            return Save();
        }
    }
}

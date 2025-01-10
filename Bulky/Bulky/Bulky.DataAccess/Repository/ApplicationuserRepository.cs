using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ApplicationuserRepository : Repository<ApplicationUser>, IApplicationuserRepository
    {
        private ApplicationDbContext _db;
        public ApplicationuserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }
        public void Update(ApplicationUser applicationUser)
        {
            _db.ApplicationUsers.Update(applicationUser);
        }
    }
}

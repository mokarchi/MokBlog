using Microsoft.EntityFrameworkCore;
using Mok.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Mok.Medias
{
    public class SqlMediaRepository : EntityRepository<Media>, IMediaRepository
    {
        private readonly ApplicationDbContext _db;
        public SqlMediaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns <see cref="Media"/> by filename and upload date, returns null if not found.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<Media> GetAsync(string fileName, int year, int month)
        {
            return isSqlite ?
                _entities.ToList().SingleOrDefault(m =>
                        m.FileName == fileName &&
                        m.UploadedOn.Year == year &&
                        m.UploadedOn.Month == month) :
                await _entities.SingleOrDefaultAsync(m =>
                        m.FileName == fileName &&
                        m.UploadedOn.Year == year &&
                        m.UploadedOn.Month == month);
        }
    }
}

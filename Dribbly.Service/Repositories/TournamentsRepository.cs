using Dribbly.Model;
using Dribbly.Model.Tournaments;
using System.Collections.Generic;

namespace Dribbly.Service.Repositories
{
    public interface ITournamentsRepository: IBaseRepository<TournamentModel>
    {
        IEnumerable<TournamentModel> GetAll();
    }
    public class TournamentsRepository : BaseRepository<TournamentModel>, ITournamentsRepository
    {
        public TournamentsRepository(IAuthContext context) : base(context.Tournaments) { }

        public IEnumerable<TournamentModel> GetAll()
        {
            return _dbSet;
        }
    }
}
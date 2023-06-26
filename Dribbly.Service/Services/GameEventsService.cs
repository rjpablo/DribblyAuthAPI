using Dribbly.Core.Utilities;
using Dribbly.Model;
using Dribbly.Model.Entities;
using Dribbly.Model.Fouls;
using Dribbly.Service.Repositories;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Services
{
    public class GameEventsService : BaseEntityService<GameEventModel>, IGameEventsService
    {
        IAuthContext _context;
        ISecurityUtility _securityUtility;
        private readonly IIndexedEntitysRepository _indexedEntitysRepository;
        private readonly IMemberFoulsRepository _memberFoulsRepository;
        private readonly IBaseRepository<GameEventModel> _gameEventsRepository;

        public GameEventsService(IAuthContext context,
            ISecurityUtility securityUtility,
            IIndexedEntitysRepository indexedEntitysRepository) : base(context.GameEvents)
        {
            _context = context;
            _securityUtility = securityUtility;
            _indexedEntitysRepository = indexedEntitysRepository;
            _memberFoulsRepository = new MemberFoulsRepository(context);
            _gameEventsRepository = new BaseRepository<GameEventModel>(context.GameEvents);
        }

        public async Task<UpsertFoulResultModel> UpsertFoulAsync(MemberFoulModel foul)
        {
            return await _memberFoulsRepository.UpsertFoul(foul);
        }
    }

    public interface IGameEventsService
    {
        Task<UpsertFoulResultModel> UpsertFoulAsync(MemberFoulModel foul);
    }
}
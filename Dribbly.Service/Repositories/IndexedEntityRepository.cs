using Dribbly.Model;
using Dribbly.Model.Shared;
using Dribbly.Service.Enums;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    #region Interface
    public interface IIndexedEntitysRepository
    {
        Task Add(IAuthContext db, IIndexedEntity entity);
        Task SetIconUrl(IAuthContext db, IIndexedEntity entity, string url);
        Task Update(IAuthContext db, IIndexedEntity entity);
    }
    #endregion

    public class IndexedEntitysRepository : BaseRepository<IndexedEntityModel>, IIndexedEntitysRepository
    {
        public IndexedEntitysRepository(IAuthContext context) : base(context.IndexedEntities) { }

        public async Task Add(IAuthContext db, IIndexedEntity entity)
        {
            db.IndexedEntities.Add(new IndexedEntityModel(entity));
            await db.SaveChangesAsync();
        }

        public async Task Update(IAuthContext db, IIndexedEntity entity)
        {
            var u = db.IndexedEntities.Find(entity.Id, entity.EntityType);
            u.Name = entity.Name;
            u.EntityStatus = entity.EntityStatus;
            u.Description = entity.Description;
            // We do not want to update IconUrl here because sometime the photo is not loading, giving us a null value
            // u.IconUrl = entity.IconUrl;
            await db.SaveChangesAsync();
        }

        public async Task SetIconUrl(IAuthContext db, IIndexedEntity entity, string url)
        {
            var u = db.IndexedEntities.Find(entity.Id, entity.EntityType);
            u.IconUrl = url;
            await db.SaveChangesAsync();
        }

    }
}
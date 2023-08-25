using Dribbly.Core.Enums;
using Dribbly.Core.Models;
using Dribbly.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    #region Interface
    public interface IIndexedEntitysRepository
    {
        Task Add(IAuthContext db, IIndexedEntity entity, string additionalData = null);
        Task SetIconUrl(IAuthContext db, IIndexedEntity entity, string url);
        Task Update(IAuthContext db, IIndexedEntity entity);
    }
    #endregion

    public class IndexedEntitysRepository : IIndexedEntitysRepository
    {
        public IndexedEntitysRepository(IAuthContext context) { }

        public async Task Add(IAuthContext db, IIndexedEntity entity, string additionalData = null)
        {
            db.IndexedEntities.Add(new IndexedEntityModel(entity, additionalData));
            await db.SaveChangesAsync();
        }

        public async Task Update(IAuthContext db, IIndexedEntity entity)
        {
            if(entity.EntityType == EntityTypeEnum.Account)
            {
                entity = await db.Players.Include(a => a.ProfilePhoto).Include(a => a.User).Include(a => a.HomeCourt)
                .Include(a => a.HomeCourt.PrimaryPhoto).SingleOrDefaultAsync(a => a.Id == entity.Id);
            }
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
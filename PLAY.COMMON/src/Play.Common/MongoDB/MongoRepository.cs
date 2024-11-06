using System.Collections.ObjectModel;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace Play.Common.MongoDb
{
    public class MongoRepository<T>:IRepository<T> where T: IEntity
    {
    
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;
        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            
            dbCollection = database.GetCollection<T>(collectionName);

        }

        public async Task<ReadOnlyCollection<T>> GetAllAsync()
        {
            var entities = await dbCollection.Find(filterBuilder.Empty).ToListAsync();
            return entities.AsReadOnly();
        }
         public async Task<ReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            var entities = await dbCollection.Find(filter).ToListAsync();
            return entities.AsReadOnly();
        }

        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }
         public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }

       
       
    }
}
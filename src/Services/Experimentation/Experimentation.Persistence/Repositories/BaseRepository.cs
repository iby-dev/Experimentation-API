using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Experimentation.Domain.Entities;
using Experimentation.Domain.Exceptions;
using Experimentation.Domain.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using Polly;

namespace Experimentation.Persistence.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity, string> where TEntity : IEntity
    {
        protected abstract IMongoCollection<TEntity> Collection { get; }

        public virtual async Task<TEntity> GetByIdAsync(string id)
        {
            return await Retry(async () => await Collection.Find(x => x.Id.Equals(id)).FirstOrDefaultAsync());
        }

        public virtual async Task<bool> Exists(string id)
        {
            var count =  await Retry(async () => await Collection.CountAsync(x => x.Id.Equals(id)));
            return count > 0;
        }

        public virtual async Task<TEntity> GetByFriendlyIdAsync(int id)
        {
            return await Retry(async () => await Collection.Find(x => x.FriendlyId.Equals(id)).FirstOrDefaultAsync());
        }

        public virtual async Task<TEntity> SaveAsync(TEntity entity)
        {
            var isUniqueId = Collection.Count(item => item.FriendlyId == entity.FriendlyId);
            if (isUniqueId > 0)
            {
                throw new NonUniqueValueDetectedException(GetType().FullName, entity.FriendlyId.ToString());
            }

            var isUniqueName = Collection.Count(item => item.Name == entity.Name);
            if (isUniqueName > 0)
            {
                throw new NonUniqueValueDetectedException(GetType().FullName, entity.Name);
            }

            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                entity.Id = ObjectId.GenerateNewId().ToString();
            }
            return await Save(entity);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                throw new ArgumentNullException($"{nameof(entity.Id)}", "The given entity does not have an id set on it.");
            }

            var original = await Collection.Find(x => x.Id.Equals(entity.Id)).FirstOrDefaultAsync();
            if (original.FriendlyId != entity.FriendlyId) // Friendly id has changed from original.
            {
                var isUniqueId = Collection.Count(item => item.FriendlyId == entity.FriendlyId);
                if (isUniqueId > 0)
                {
                    throw new NonUniqueValueDetectedException(GetType().FullName, entity.FriendlyId.ToString());
                }
            }

            if (original.Name != entity.Name) // name has changed from original.
            {
                var isUniqueName = Collection.Count(item => item.Name == entity.Name);
                if (isUniqueName > 0)
                {
                    throw new NonUniqueValueDetectedException(GetType().FullName, entity.Name);
                }
            }

            await Save(entity);
        }

        private async Task<TEntity> Save(TEntity entity)
        {
            return await Retry(async () =>
            {
                await Collection.ReplaceOneAsync(
                    x => x.Id.Equals(entity.Id),
                    entity,
                    new UpdateOptions
                    {
                        IsUpsert = true
                    });

                return entity;
            });
        }

        public virtual async Task DeleteAsync(string id)
        {
            await Retry(async () => await Collection.DeleteOneAsync(x => x.Id.Equals(id)));
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Retry(async () => await Collection.Find(predicate).ToListAsync());
        }

        protected virtual TResult Retry<TResult>(Func<TResult> action)
        {
            return Policy
                .Handle<MongoConnectionException>(i => i.InnerException.GetType() == typeof(IOException))
                .Retry(3)
                .Execute(action);
        }
    }
}
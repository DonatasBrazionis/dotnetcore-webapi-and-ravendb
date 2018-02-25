using System.Collections.Generic;
using System.Threading.Tasks;
using Raven.Client.Documents;

namespace dotnetcore_webapi_and_ravendb.Providers
{
    public class RavenDBProvider
    {
        public RavenDBProvider(IDocumentStore documentStore)
        {
            DocumentStore = documentStore;
        }
        public IDocumentStore DocumentStore { get; set; }

        public async Task CreateEntity<T>(T entity)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                await session.StoreAsync(entity);
                await session.SaveChangesAsync();
            }
        }

        public async Task UpdateEntity<T>(string entityId, T entity)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                await session.StoreAsync(entity, entityId);
                await session.SaveChangesAsync();
            }
        }

        public async Task DeleteEntity(string entityId)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                session.Delete(entityId);
                await session.SaveChangesAsync();
            }
        }

        public async Task<T> GetEntity<T>(string entityId)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                var entity = await session.LoadAsync<T>(entityId);
                return entity;
            }
        }

        public async Task<List<T>> GetEntities<T>()
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                var entities = await session.Query<T>().ToListAsync();
                return entities;
            }
        }

        public async Task<bool> IsEntityExists(string entityId)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                bool exists = await session.Advanced.ExistsAsync(entityId);
                return exists;
            }
        }
    }
}

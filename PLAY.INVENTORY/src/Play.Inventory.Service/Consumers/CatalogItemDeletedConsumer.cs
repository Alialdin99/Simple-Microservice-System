using Catalog.Contracts;
using MassTransit;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service
{
    public class CatalogItemDeletedConsumer: IConsumer<CatalogItemDeleted>
    {
        private readonly IRepository<CatalogItem> _repository;
        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
        {
            _repository = repository;
        }
        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            var message = context.Message;
            var item = await _repository.GetAsync(message.ItemId);

            if (item == null)
                return;
            else
                await _repository.DeleteAsync(message.ItemId);
        }
    }
}
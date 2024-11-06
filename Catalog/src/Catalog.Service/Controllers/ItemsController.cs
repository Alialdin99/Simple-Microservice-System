using Microsoft.AspNetCore.Mvc;
using Catalog.Service.Entities;
using Catalog.Contracts;
using Play.Common;
using MassTransit;
namespace Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> _itemsRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        //  private static int requestCounter = 0;
        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            _itemsRepository = itemsRepository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            // requestCounter++;
            // Console.WriteLine($"Request {requestCounter}: stating...");
            // if(requestCounter <= 2)
            // {
            //     Console.WriteLine($"Request {requestCounter}: Delaying...");
            //     await Task.Delay(TimeSpan.FromSeconds(10));
            // }
            // if(requestCounter <= 4)
            // {
            //     Console.WriteLine($"Reguest {requestCounter}:500 (Internal Server Error).");
            //     return StatusCode(500);
            // }
            var items = (await _itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());
            //Console.WriteLine($"Request {requestCounter}: 200 (ok).");
            return Ok(items);
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await _itemsRepository.GetAsync(id);
            if (item is null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto itemDto)
        {
            Item item = new(Guid.NewGuid(), itemDto.Name, itemDto.Description, itemDto.Price, DateTimeOffset.UtcNow);
            await _itemsRepository.CreateAsync(item);

            await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await _itemsRepository.GetAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            existingItem.Name = itemDto.Name;
            existingItem.Description = itemDto.Description;
            existingItem.Price = itemDto.Price;

            await _itemsRepository.UpdateAsync(existingItem);
            await _publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));


            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {

            var item = await _itemsRepository.GetAsync(id);
            if (item is null)
            {
                return NotFound();
            }


            await _itemsRepository.DeleteAsync(id);
            await _publishEndpoint.Publish(new CatalogItemDeleted(id));


            return NoContent();
        }
    }

}
using Grpc.Net.Client;
using ItemGRPC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MicroSer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
      

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<model.Item>>> GetItem()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7132");
            var ItemClient = new Items.ItemsClient(channel);
            var getRequest = new GetItemRequest();
            var ItemReturn = new List<model.Item>();

            try
            {
                var Itemlist = await ItemClient.GetItemAsync(getRequest);
                if (Itemlist.Code != 200)
                {
                    return NotFound();
                }
                foreach (var Item in Itemlist.Data)
                {
                    var tempCust = new model.Item();
                    tempCust.UnitPrice = (decimal)Item.UnitPrice;
                    tempCust.Name = Item.Name;
                    tempCust.Id = Item.Id;
                    ItemReturn.Add(tempCust);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ItemReturn;
        }


        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<model.Item>> GetItems(int id)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7132");
            var ItemClient = new Items.ItemsClient(channel);
            var getRequest = new GetItemDetailRequest()
            {
                ID = id,
            };
            var ItemReturn = new model.Item();

            try
            {
                var Item = await ItemClient.GetItemDetailAsync(getRequest);
                if (Item.Code != 200)
                {
                    return NotFound();
                }
                ItemReturn.UnitPrice = (decimal)Item.Data.UnitPrice;
                ItemReturn.Name = Item.Data.Name;
                ItemReturn.Id = Item.Data.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ItemReturn;

        }

    }
}

using GeneralStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI.Controllers
{
    public class TransactionController : ApiController
    {
        GeneralStoreDbContext _context = new GeneralStoreDbContext();

        //POST
        [HttpPost]
        public async Task<IHttpActionResult> CreateNewTrans([FromBody] Transaction model)
        {   
            
            Product product = await _context.Products.FindAsync(model.SKU);
            if (product is null)
                return BadRequest("Product not found.");

            if (model.ItemCount > product.NumberInInventory)
                return BadRequest($"There are only {product.NumberInInventory} in stock at this time.");

            if (ModelState.IsValid)
            {
                int invUpt = product.NumberInInventory - model.ItemCount;
                _context.Transactions.Add(model);
                product.NumberInInventory = invUpt;

                await _context.SaveChangesAsync();
                return Ok("Transaction Successful!");
                
            }
            return BadRequest(ModelState);

            
        }
    }
}

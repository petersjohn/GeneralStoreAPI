using GeneralStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI.Controllers
{
    public class ProductController : ApiController
    {
        private readonly GeneralStoreDbContext _context = new GeneralStoreDbContext();

        //POST
        //api/Product
        [HttpPost]
        public async Task<IHttpActionResult> CreateProduct([FromBody] Product model)
        {
            if (model is null)
                return BadRequest("Request body cannot be empty");
            if (ModelState.IsValid)
            {
                _context.Products.Add(model);
                await _context.SaveChangesAsync();
                return Ok("Product saved!");

            }
            return BadRequest(ModelState);
             
        }
        //GET
        //api/Product
        [HttpGet]
        public async Task<IHttpActionResult> GetAllProducts()
        {
            List<Product> product = await _context.Products.ToListAsync();
            if (product is null)
                return NotFound();
            return Ok(product);
        }

        //GET
        //api/Product
        [HttpGet]
        public async Task<IHttpActionResult> GetProductBySKU([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (product is null)
                return NotFound();
            return Ok(product);
        }

        //PUT
        //api/Product

        [HttpPut]
        public async Task<IHttpActionResult> UpdateProduct([FromUri] string sku, [FromBody] Product updatedProduct)
        {
            //do the id's match?
            if (sku == (updatedProduct?.SKU))
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //find the product in the database
                Product product = await _context.Products.FindAsync(sku);
                if (product is null)
                    return NotFound();
                product.Name = updatedProduct.Name;
                product.Cost = updatedProduct.Cost;
                product.NumberInInventory = updatedProduct.NumberInInventory;

                await _context.SaveChangesAsync();
                return Ok("Update Successful!");
            }

            return BadRequest("Product SKUs do not match");

        }

        //DELETE
        //api/Product
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteProduct([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (product is null)
                return NotFound();
            _context.Products.Remove(product);
            if (await _context.SaveChangesAsync() == 1)
                return Ok("The product was deleted.");
            return InternalServerError();

        }

    }
}

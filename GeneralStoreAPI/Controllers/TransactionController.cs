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
    public class TransactionController : ApiController
    {
       private  GeneralStoreDbContext _context = new GeneralStoreDbContext();

        //POST
        [HttpPost]
        public async Task<IHttpActionResult> CreateNewTrans([FromBody] Transaction model)
        {

            Product product = await _context.Products.FindAsync(model.SKU);
            if (product is null)
                return BadRequest("Product not found.");

            if (model.ItemCount > product.NumberInInventory)
                return BadRequest($"There are only {product.NumberInInventory} in stock at this time.");

            Customer customer = await _context.Customers.FindAsync(model.CustomerId);

            if (customer is null)
                return BadRequest("Transaction customer not found.");

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

        //GET all transactions
        //api/Transaction
        [HttpGet]
        public async Task<IHttpActionResult> GetAllTransactions()
        {
            List<Transaction> transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);

        }

        //GET all transactions by foreign key CustomerID
        //api/Transaction
        [HttpGet]
        public async Task<IHttpActionResult> GetAllTransByCustomerAsync(int CustomerID)
        {
            Customer transaction = await _context.Customers.FindAsync(CustomerID);
            
                if (transaction is null)
                return NotFound();

            //List<Transaction> transactions = customer.transactions;

            return Ok(transaction);

        }



        //GET Transaction by Transaction ID
        [HttpGet]
        public async Task<IHttpActionResult> GetTransByIdAsync([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);
            if (transaction is null)
                return NotFound();
            return Ok(transaction);

        }

        //PUT Update transaction by ID
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTransAsync([FromUri] int id, [FromBody] Transaction updtTransaction)
        {
            if (id != updtTransaction?.ID)
                return BadRequest("The uri ID does not match the id in the request");
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (updtTransaction is null)
                return BadRequest("Request body is empty");
            //Get the original transaction
            Transaction originalTransaction = await _context.Transactions.FindAsync(id);
            
            if (originalTransaction is null)
                return NotFound();
            
            Product productFromRequest = await _context.Products.FindAsync(updtTransaction.SKU);
            
            if (productFromRequest is null)
                return BadRequest("Product in request was not found");
            int updtItemCnt = updtTransaction.ItemCount;
            int newInv;
            if (originalTransaction.SKU == updtTransaction.SKU)
            {
                newInv = (productFromRequest.NumberInInventory + originalTransaction.ItemCount) - updtTransaction.ItemCount;
                if (newInv < 0)
                    return BadRequest("There is not enough inventory to cover the transaction items.");
                productFromRequest.NumberInInventory = newInv;
                
            }
            else
            {
                if (productFromRequest.NumberInInventory < updtTransaction.ItemCount)
                    return BadRequest($"The requested amount of product exceed the in stock amount of {productFromRequest.NumberInInventory}.");
                Product productFromOrigTrans = await _context.Products.FindAsync(originalTransaction.SKU);
                productFromOrigTrans.NumberInInventory = productFromOrigTrans.NumberInInventory + originalTransaction.ItemCount; //give the inventory back from the original transaction
                productFromRequest.NumberInInventory = productFromRequest.NumberInInventory - updtTransaction.ItemCount;
            }
            if(originalTransaction.DateOfTransaction != updtTransaction.DateOfTransaction)
            {
                originalTransaction.DateOfTransaction = updtTransaction.DateOfTransaction;
            }

            if(originalTransaction.DateOfTransaction != updtTransaction.DateOfTransaction)
            {
                originalTransaction.DateOfTransaction = updtTransaction.DateOfTransaction;
            }

            if(originalTransaction.CustomerId != updtTransaction.CustomerId)
            {
                originalTransaction.DateOfTransaction = updtTransaction.DateOfTransaction;
            }
               
            if(originalTransaction.ItemCount != updtTransaction.ItemCount)
            {
                originalTransaction.ItemCount = updtTransaction.ItemCount;
            }
            if (originalTransaction.SKU != updtTransaction.SKU)
            {
                originalTransaction.SKU = updtTransaction.SKU;
            }
            await _context.SaveChangesAsync();
            return Ok("Update Successful!");


        }

        //DELETE 
        //api/Transaction

        [HttpDelete]

        public async Task<IHttpActionResult> DeleteTransByIdAsync([FromUri] int id)
        {
            //get the transaction
            Transaction transaction = await _context.Transactions.FindAsync(id);
            if (transaction is null)
                return InternalServerError(); 

            Product product = await _context.Products.FindAsync(transaction.SKU);
            if (product is null)
                return InternalServerError();

            product.NumberInInventory = product.NumberInInventory + transaction.ItemCount; //give back the inventory from the deleted transactiom
            _context.Transactions.Remove(transaction);

            await _context.SaveChangesAsync();
            return Ok("Transaction Deleted");
  
        }



       
    }
}

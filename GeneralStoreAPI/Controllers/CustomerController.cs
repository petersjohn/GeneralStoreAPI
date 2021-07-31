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
    public class CustomerController : ApiController
    {
        private readonly GeneralStoreDbContext _context = new GeneralStoreDbContext();
        //POST
        // api/Customer
        [HttpPost]
        public async Task<IHttpActionResult> CreateCustomer([FromBody]Customer model)
        {
            if (model is null)
                return BadRequest("Request body not provided");
            if (ModelState.IsValid)
            {
                _context.Customers.Add(model);
                await _context.SaveChangesAsync();
                return Ok("New customer created!");
            }
            return BadRequest(ModelState);
        }
        [HttpGet]
        //api/Customer
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            List<Customer> customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        //GET by id
        //api/Customer
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerById([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);
            if (customer is null)
                return NotFound();
            return Ok(customer);
        }

        //PUT
        //api/Customer
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomer([FromUri] int id,[FromBody] Customer updatedCustomer)
        {
            if (id != updatedCustomer?.Id) //prevents an exception if the model is null (can't access a property of a null)
                return BadRequest("Ids do not match");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Customer customer = await _context.Customers.FindAsync(id);

            if (customer is null)
                return NotFound();

            //Update the customer props.
            //prevent overwriting firstName on a null inbound name since FirstName is not required
            if (updatedCustomer.FirstName != null)
               customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;

            //Save Changes to DB
            await _context.SaveChangesAsync();
            return Ok("Customer updated!");
        }

        [HttpDelete]
        public async Task<IHttpActionResult> DeleteCustomerById([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);
            if (customer is null)
                return NotFound();
            _context.Customers.Remove(customer);

            if(await _context.SaveChangesAsync() == 1)
            {
                return Ok("Delete successful.");
            }
            return InternalServerError();

        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        [Required]
        public string  LastName { get; set; }
        public string FullName
        {
            get
            {
                return ($"{FirstName} {LastName}");
            }
        }
        public virtual List<Transaction> transactions { get; set; }


    }
}
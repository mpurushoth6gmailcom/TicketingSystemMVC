using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicketingSystemMVC.Models
{
    public class TicketLogModel
    {
        [Required(ErrorMessage = "Enter Ticket Title")]
        public Int64 Ticket_Id { get; set; }
        public string Ticket_Title { get; set; }

        [Required(ErrorMessage = "Enter Title Description")]
        public string Ticket_Description { get; set; }

        [Required(ErrorMessage = "Select Category")]
        public Int64 Ticket_Category { get; set; }

        public List<TicketCategory> TicketCategoryList { get; set; }

        [DataType(DataType.Date)]
        public DateTime Create_Date { get; set; }
    }

    public class TicketCategory
    {
        public Int64 Id { get; set; }

        public string Category_Name { get; set; }


    }
}

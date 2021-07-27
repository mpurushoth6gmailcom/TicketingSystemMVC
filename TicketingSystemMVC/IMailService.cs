using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TicketingSystemMVC.Models.MailerModel;

namespace TicketingSystemMVC
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}

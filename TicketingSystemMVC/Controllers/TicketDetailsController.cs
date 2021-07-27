using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TicketingSystemMVC.Models;
using static TicketingSystemMVC.Models.MailerModel;

namespace TicketingSystemMVC.Controllers
{
    public class TicketDetailsController : Controller
    {
        private readonly ILogger<TicketDetailsController> _logger;
        private readonly IConfiguration _Configure;
        string apiBaseUrl = string.Empty;
        string _mailtosend = string.Empty;
        private readonly IMailService _mailService;
        public TicketDetailsController(ILogger<TicketDetailsController> logger, IConfiguration configuration, IMailService mailService)
        {
            _logger = logger;
            _Configure = configuration;
            _mailService = mailService;

            apiBaseUrl = _Configure.GetValue<string>("WebAPIBaseUrl");
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<TicketLogModel> ticketLogModels = new List<TicketLogModel>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(apiBaseUrl+ "/ListTicketDetails"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ticketLogModels = JsonConvert.DeserializeObject<List<TicketLogModel>>(apiResponse);
                }
            }
            return View(ticketLogModels);
        }

        public async Task<IActionResult> Edit(Int64 Id)
        {
            TicketLogModel ticketLogModels = new TicketLogModel();
            using (var httpClient = new HttpClient())
            {
                string endpoint = apiBaseUrl + "/ListTicketDetailsById?Id="+ Id;
                using (var response = await httpClient.GetAsync(endpoint))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ticketLogModels = JsonConvert.DeserializeObject<TicketLogModel>(apiResponse);
                }
            }
            ticketLogModels.TicketCategoryList = await LoadTicketCategory();
            return View("Create", ticketLogModels);
        }

        [HttpPost]
        public async Task<IActionResult> AddTicket(TicketLogModel ticketLogModel)
        {
            TicketLogModel objTicketLogModel = new TicketLogModel();
            MailRequest mailRequest = new MailRequest();
            mailRequest.ToEmail = _Configure.GetValue<string>("Mail");
            mailRequest.Body = "Ticket_Title " + ticketLogModel.Ticket_Title + "Ticket_Description" + ticketLogModel.Ticket_Description + "Ticket_Category" + ticketLogModel.Ticket_Category;
            mailRequest.Subject = "Ticket newly created";
            Send(mailRequest);
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(ticketLogModel), Encoding.UTF8, "application/json");

                string endpoint = apiBaseUrl + "/SaveTicketDetails";
                using (var response = await httpClient.PostAsync(endpoint , content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                         return  RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                        return RedirectToAction("Index");
                    }
                }
            }
            //reawait Index();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTicket(TicketLogModel ticketLogModel)
        {
            int result = 0;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(ticketLogModel), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("apiBaseUrl" + "/UpdateTicketDetails", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    result = JsonConvert.DeserializeObject<int>(apiResponse);
                }
            }
            return await Index();
        }

        public async Task<int> DeleteTicketDetails(Int64 Id)
        {
            int result = 0;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync(apiBaseUrl + "/DeleteTicketDetails?Id=" + Id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    result = JsonConvert.DeserializeObject<int>(apiResponse);
                }
            }
            return result;
        }

        public async Task<List<TicketCategory>> LoadTicketCategory()
        {
            List<TicketCategory> ticketLogModels = new List<TicketCategory>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(apiBaseUrl + "/ListTicketCategory"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ticketLogModels = JsonConvert.DeserializeObject<List<TicketCategory>>(apiResponse);
                }
            }
            return ticketLogModels;
        }

        public async Task<ActionResult> Create()
        {
            TicketLogModel objTicketLogModel = new TicketLogModel();
            objTicketLogModel.TicketCategoryList = await LoadTicketCategory();
            return View("Create", objTicketLogModel);
            //ViewBag.TicketCategoryList= LoadTicketCategory();
            //return View("Create");
        }

        public async void Send([FromForm] MailRequest request)
        {
            try
            {
                await _mailService.SendEmailAsync(request);
                //return Ok();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}

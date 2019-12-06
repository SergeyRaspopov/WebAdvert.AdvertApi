using AdvertApi.Models;
using AdvertApi.Models.Messages;
using AdvertApi.Services;
using Amazon.SimpleNotificationService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertApi.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    public class AdvertController : Controller
    {
        private IAdvertStorageService _advertStorageService;
        private IConfiguration _configuration;
        private IMapper _mapper;

        public AdvertController(IAdvertStorageService advertStorageService, IConfiguration configuration, IMapper mapper)
        {
            _advertStorageService = advertStorageService;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(400)]
        [ProducesResponseType(201, Type = typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            try
            {
                var recordId = await _advertStorageService.Add(model);
                return StatusCode(201, new CreateAdvertResponse() { Id = recordId });
            }
            catch(KeyNotFoundException ex)
            {
                return new NotFoundResult();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            try
            {
                var dbModel = await _advertStorageService.Confirm(model);
                if (dbModel != null)
                    await RaiseAdvertConfirmMessage(dbModel);
                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private async Task RaiseAdvertConfirmMessage(AdvertDbModel model)
        {
            var topicArn = _configuration.GetValue<string>("TopicArn");
            using (var client = new AmazonSimpleNotificationServiceClient())
            {
                var msg = JsonConvert.SerializeObject(_mapper.Map<AdvertConfirmedMessage>(model));
                await client.PublishAsync(topicArn, msg);
            }
        }
    }
}

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MaKore.Models;
using MaKore.JsonClasses;
using MaKore.Services;

namespace MaKore.Controllers
{

    [ApiController]
    [Route("api")]
    public class InvitationsController : BaseController
    {
        public IInvitationsService _service;
        public IConfiguration _configuration;

        public InvitationsController(MaKoreContext context, IConfiguration config)
        {
            _service = new InvitationsService(context);
            _configuration = config;
        }

        // a different server serfs here, thus no authantication. Someone wants to talk to OUR user
        [HttpPost("invitations")]
        public IActionResult Invitations([Bind("From,To,Server")] JsonInvitation info)
        {
            if (_service.CreateOnInvitation(info) == true)
            {
                return StatusCode(201);
            }
            return BadRequest();
        }
    }
}
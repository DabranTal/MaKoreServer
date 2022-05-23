using MaKore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaKore.JsonClasses;
using MaKore.Services;

namespace MaKore.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        public IConfiguration _configuration;
        public IUserService _serviceU;
        public IConversationService _serviceC;
        public IMessageService _serviceM;

        public TransferController(MaKoreContext context, IConfiguration config)
        {
            _configuration = config;
            _serviceU = new UserService(context);
            _serviceC = new ConversationService(context);
            _serviceM = new MessageService(context);
        }


        [HttpPost]
        [ActionName("transfer")]
        public IActionResult Transfer([Bind("From, To, Content")] JsonTransfer mpl)
        {

            // TO CHECK
            string username = mpl.From;
            string partner = mpl.To;

            // Make Sure conversation exist other case create her
            if ((_serviceU.IsRemoteUser(username) || _serviceU.IsLocalUser(username)) && (_serviceU.IsRemoteUser(partner) || _serviceU.IsLocalUser(partner)))
            {
                if (!(_serviceC.DoesConversationExist(username, partner) || _serviceC.DoesConversationExist(partner, username)))
                {
                    if (_serviceU.IsLocalUser(username))
                    {
                        if (_serviceU.IsLocalUser(partner))
                        {
                            _serviceC.ConvWithLocals(username, partner);
                        }
                        else
                        {
                            _serviceC.ConvWithRemote(username, partner);
                        }
                    }
                    else if (_serviceU.IsLocalUser(partner))
                    {
                        if (_serviceU.IsRemoteUser(username))
                        {
                            _serviceC.ConvWithRemote(partner, username);
                        }
                    }
                }
            }

            if (_serviceM.Transfer(username, partner, mpl) == true)
            {
                return StatusCode(201);
            }
            return BadRequest();
        }
    }
}
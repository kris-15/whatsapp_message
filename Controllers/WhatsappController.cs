using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Fax;
using Twilio.Types;
using WhatsappApiProject.Helpers;
using WhatsappApiProject.Models;

namespace WhatsappProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {
        [HttpPost]
        public IActionResult PostMessage([FromBody] Receiver receiver)
        {
            const string CODE = "+243";
            bool validNumberFormat = false;
            string phoneNumber = "";
            receiver.PhoneNumber = receiver.PhoneNumber.Trim(' ');
            string telephone = receiver.PhoneNumber;
            if (telephone.StartsWith("0") | telephone.StartsWith("+243") | telephone.StartsWith("243"))
            {
                if (telephone.StartsWith("0") & telephone.Length == 10)
                {
                    phoneNumber = CODE + telephone.Substring(1);
                    validNumberFormat = true;
                }
                if (telephone.StartsWith("243"))
                {
                    validNumberFormat = (telephone.Length > 12 | telephone.Length < 12) ? false : true;
                    phoneNumber = validNumberFormat == true ? "+" + telephone : "";
                }
                if (telephone.StartsWith("+243"))
                {
                    validNumberFormat = (telephone.Length > 13 | telephone.Length < 13) ? false : true;
                    phoneNumber = validNumberFormat ? telephone : "";
                }
            }
            else
            {
                validNumberFormat = false;
            }
            if (validNumberFormat)
            {
                TwilioClient.Init(Setting.APIKEY, Setting.APISECRET);
                PhoneNumber sender = new PhoneNumber("whatsapp:" + Setting.WHATSAPPNUMBER);
                PhoneNumber target = new PhoneNumber("whatsapp:" + phoneNumber);
                CreateMessageOptions options = new CreateMessageOptions(target);
                options.From = sender;
                options.Body = receiver.MessageContent;
                MessageResource.Create(options);
                return Ok(new { phone = phoneNumber, message = receiver.MessageContent, status = "Sent" });
            }
            return Ok(new { status = validNumberFormat, error = "Invalid phone number format" });
        }
    }
}

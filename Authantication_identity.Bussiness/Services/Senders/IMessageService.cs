using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authantication_identity.Bussiness.Enums;

namespace Authantication_identity.Bussiness.Services.Senders
{
   public interface IMessageService
   {
       MessageStates messageStates { get; }
       Task SendAsync(IdentityMessage message, params string[] contacts);
       void Send(IdentityMessage message, params string[] contacts);
   }
}

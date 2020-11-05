using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authantication_identity.Bussiness.Helpers
{
   public class StringHelper
    {
        public StringHelper()
        {
            
        }
        public string GetCode()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}

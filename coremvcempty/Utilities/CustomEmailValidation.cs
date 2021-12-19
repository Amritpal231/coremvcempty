using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace coremvcempty.Utilities
{
    public class CustomEmailValidation : ValidationAttribute
    {
        private readonly string allowedDomain;
    
        public CustomEmailValidation(string allowedeDomain)
        {
            this.allowedDomain = allowedeDomain;
        }

      

        public override bool IsValid(object value)
        {
           string domain = value.ToString().Split('@')[1];

            return domain.ToUpper() == allowedDomain.ToUpper();        
        }
    }
}

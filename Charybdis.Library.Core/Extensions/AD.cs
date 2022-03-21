using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;

namespace Charybdis.Library.Core
{
    public class AD
    {
        public static bool ValidateNTLogin(string userName, string password)
        {
            bool result = false;
            string domain = "localhost"; //Assume local if no domain specified.
            string userOnly = userName; //Assume only a username if no known split found.
            if (userName.Contains('\\')) //Handle DOMAIN\USER format.
            {
                var split = userName.Split('\\');
                domain = split[0];
                userOnly = split[1];
            }
            else if (userName.Contains('@')) //Handle USER@DOMAIN format.
            {
                var split = userName.Split('@');
                userOnly = split[0];
                domain = split[1];
            }
            using (var pc = new PrincipalContext(domain.ToLower() == "localhost" ? ContextType.Machine : ContextType.Domain, domain)) //Try local machine if local, otherwise domain.
            {
                result = pc.ValidateCredentials(userName, password); //Validate.
                //Not sure why this step is necessary, but replicating it because it was in place in LSAM and it might be for a certain customer or something. -UdderlyEvelyn 1/31/17
                if (!result && !domain.ToLower().EndsWith(".com")) //If that doesn't work, and the domain doesn't end in ".com"..
                    pc.ValidateCredentials(userOnly + '@' + domain + ".com", password); //Try with the USER@DOMAIN.COM format.
            }
            return result;
        }
    }
}

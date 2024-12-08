using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountKeeper.Service
{
    public class AuthService
    {
        private string _loggedinusername;

        public AuthService()
        {
            _loggedinusername = "coco_rys";
        }

        public string Getloggedinusername()
        {
            return _loggedinusername;
        }

        public bool Logout()
        {
            _loggedinusername = null;
            return true;
        }
    }
}

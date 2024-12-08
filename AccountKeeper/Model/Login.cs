using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountKeeper.Model
{
    public class Login
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class Librarian : User
    {
        public Librarian()
        {
            Role = "Librarian";
        }
    }
}
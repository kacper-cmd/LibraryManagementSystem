using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Factory.Contracts
{
    public interface IUserFactory
    {
        User CreateUser(string userRole, string name, string email, string password);
    }
}

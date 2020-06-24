using System.Collections.Generic;
using Microtransection.DB;

public interface IUserService
{
    public List<Users> GetAll();
    public string InsertUser(UserShop model);
}
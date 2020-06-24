public class UserShop
{
    //dotnet ef dbcontext Scaffold "Server=tcp:192.168.1.7,1433;Database=Deno;User ID=xxxx;Password=xxxx;" Microsoft.EntityFrameworkCore.SqlServer --force -o DB -c DenoContext

    public string name { get; set; }
    public string surname { get; set; }
    public int id { get; set; }
    public int no { get; set; }
    public string productName { get; set; }
    public decimal? productPrice { get; set; }
}
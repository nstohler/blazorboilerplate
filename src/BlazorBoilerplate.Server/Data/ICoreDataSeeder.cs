using System.Threading.Tasks;

namespace BlazorBoilerplate.Server.Data
{
    public interface ICoreDataSeeder
    {
        Task Seed();
    }
}
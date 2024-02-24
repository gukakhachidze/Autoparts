using Autoparts.Models;
using System.Runtime.CompilerServices;

namespace Autoparts.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetByIdNoTrackingAsync(int id);
        Task<IEnumerable<Product>> GetProdudctByTitle(string title);
        bool Add(Product product);
        bool Update(Product product);
        bool Delete(Product product);
        bool Save();
    }
}

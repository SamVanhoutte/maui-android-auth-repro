using Sfinx.Backend.WebAPI.Client;

namespace Sfinx.App.Shared.Interfaces;

public interface IVendorService
{
    Task<ICollection<Vendor>> GetVendorsAsync(bool forceRefresh = false);
    Task<ICollection<Product>> GetVendorProductsAsync(string vendorId, bool forceRefresh = false);
}
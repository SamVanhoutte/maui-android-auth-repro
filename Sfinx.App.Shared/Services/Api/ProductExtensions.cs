using Sfinx.Backend.WebAPI.Client;

namespace Sfinx.App.Shared.Services.Api;

public static class ProductService
{
    public static Product SimulatedLockType => new Product()
    {
        Id = "885c7eba-0daa-48b0-af8c-a3375cef5620",
        VendorId = "sfinx",
        Name = "Simulated"
    };
}
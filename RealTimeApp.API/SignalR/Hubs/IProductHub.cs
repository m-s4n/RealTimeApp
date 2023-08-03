using RealTimeApp.API.Models;

namespace RealTimeApp.API.SignalR.Hubs
{
    public interface IProductHub
    {
        Task ReceiveProduct(Product product);
    }
}

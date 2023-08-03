using Microsoft.AspNetCore.SignalR;
using RealTimeApp.API.Models;

namespace RealTimeApp.API.SignalR.Hubs
{
    // Strongly type hub - tip güvenli hub
    public class ProductHub:Hub<IProductHub>
    {
        public async Task SendProduct(Product product)
        {
            // Client'ta tetiklenecek fonksiyonlar tip güvenli yazılıyor
            await Clients.All.ReceiveProduct(product);
        }
    }
}

using Microsoft.AspNetCore.SignalR;

namespace RealTimeApp.API.SignalR.Hubs
{
    // hub içerisinde client'ların cagıracağı metotlar tanımlanır
    public class MyHub:Hub
    {
        public static List<string> Names = new List<string>();
        // client'lar hub içerisindeki metotlara erişecek
        static int ClientCount { get; set; } = 0; 
        public static int TeamCount { get; set; } = 7;
        public async Task  SendName(string name)
        {
            if(Names.Count >= TeamCount)
            {
                Console.WriteLine(Names.Count);
                await Clients.Caller.SendAsync(method: "AddedError", arg1: $"Takım sayısı en fazla {TeamCount} kişi olabilir");
            }
            else
            {
                // Clients ile client'lerdeki metotların çalışması için bir istek göndericem
                // client'larda bu metot tanımlıysa çalışacak
                //All bu huba bağlı olan tüm client'lara bildiri gönderir
                // hub bu mesajı client'lara serialize edecektir
                Names.Add(name);
                await Clients.All.SendAsync(method: "ReceiveName", arg1: name);
            }
            
        }

        // clientlar sendmessage'a istek yapacaklar ve message adında parametre gönderecekler
        // daha sonra bu metot çalıştığı zaman client'lar üzerindeki 'ReceiveMessage' isimli
        // metotlara bir bildiri gönderecek
        // Client'lar üzerindeki şu metot şu parametre ile çalışssın

        // Client 'ReceiveMessage' şu metotu tanımladığı zaman otomatik bir şekilde tanımlanacaktır

        //2. metot client'lar memory'de olan name'leri alsın
        // Client'ler GetNames'a çağrı yaptığı zaman bu metot'ta client'lardaki ReceiveNames isimli metota çağrı yapar
        public async Task GetNames()
        {
            await Clients.All.SendAsync(method: "ReceiveNames", arg1: Names);
        }

        // Her bir client bağlandığında metot tetiklenir
        public override async Task OnConnectedAsync()
        {
            ClientCount++;
            // bir clinet bağlanınca event gönderilir.
            await Clients.All.SendAsync(method:"ReceiveClientCount", arg1: ClientCount);
        }

        // Her bir client bağlantısı koptuğunda tetiklenir

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ClientCount--;
            await Clients.All.SendAsync(method: "ReceiveClientCount", arg1: ClientCount, arg2: "client_sayisi");
        }
    }
}

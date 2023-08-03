using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealTimeApp.API.Database;
using RealTimeApp.API.Entities;
using RealTimeApp.API.Models;

namespace RealTimeApp.API.SignalR.Hubs
{
    // hub içerisinde client'ların cagıracağı metotlar tanımlanır
    public class MyHub:Hub
    {
        private readonly AppDbContext _dbContext;
        public MyHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
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

        // group işlemeleri
        // bir client'i gruba ekleme işlemi yapalım
        // client addtogroup işlemine istek attığında gruba üye olacak
        public async Task AddToGroup(string teamName)
        {
            // connectionId her bir client'ı kimliklendiriyor
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
        }

        // client'ilgili group'dan çıkmak isteyebilir
        public async Task RemoveToGroup(string teamName)
        {
            // kim hangi grup'tan çıkıyor verilir
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
        }


        // takımlara isim ekleme işlemleri
        public async Task SendNameByGroup(string name, string teamName)
        {
            var team = await _dbContext.Teams.Where(x => x.Name == teamName).FirstOrDefaultAsync();

            if(team != default)
            {
                await _dbContext.Users.AddAsync(new User { Name = name, Team = team });
            }
            else
            {
                Team newTeam = new() { Name = name };
                newTeam.Users.Add(new User { Name=name});
                await _dbContext.Teams.AddAsync(newTeam);
            }

            await _dbContext.SaveChangesAsync();

            // ekleme işlemi yapıldı client'ları bilgilendirelim
            // hangi gruba ne event yayınlamak istiyoruz
            // bu team'e üye olanlar mesaj/bildirim alabilir
            await Clients.Group(teamName).SendAsync("ReceiveMessageByGroup", arg1: name, arg2: teamName);

        }

        // tüm takım iismlerini döndürme
        public async Task GetNamesByGroup()
        {
            // client ilk girdiğinte iki takımında isimlerini görsün
            var teams = _dbContext
                .Teams
                .Include(x => x.Users)
                .Select(x => new {teamName =x.Name, users=x.Users.ToList()})
                .ToList();

            await Clients.All.SendAsync("ReceiveNamesByGroup", arg1: teams);
        }


        // Client'tan server'a complex type gönderme
        public async Task SendProduct(Product product)
        {
            await Clients.All.SendAsync("ReceiveProduct", product);
        }
    }
}

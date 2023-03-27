using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Arabam.com'dan ilan URL'lerini al
            List<string> ilanUrls = await Class1.GetIlanUrlsAsync();
            Console.WriteLine("https://www.arabam.com Vitrin Bolumu ilan bilgileri getiriliyor");
            Console.Write("Yukleniyor");

            for (int i = 0; i < 10; i++)
            {
                Console.Write("~");
                Thread.Sleep(200); 
            }

            Console.WriteLine("Basarili!");
           

            // Tüm ilanlar için sırayla bilgi al ve ilanlar listesine ekle

            List<Ilan> ilanlar = new List<Ilan>();

            foreach (string url in ilanUrls)
            {
                List<Ilan> ilanList = await Class2.GetIlanAsync("https://www.arabam.com" + url);
                ilanlar.AddRange(ilanList);
            }
            // İlanlar listesi doluysa ortalama fiyatı ve toplam fiyatı hesapla ve ekrana yazdır

            if (ilanlar.Count > 0)
            {
                double totalPrice = ilanlar.Sum(ilan => ilan.Price);
                double averagePrice = totalPrice / ilanlar.Count;
                Console.WriteLine("Average Price: " + averagePrice.ToString("#.###"));
                Console.WriteLine("Total Prices: " + totalPrice.ToString("#.###"));
            }
        }
    }
}
/*Bu kod, Arabam.com web sitesindeki Vitrin bölümündeki ilanlar için ilan URL'lerini alır ve her bir URL'yi kullanarak ilan bilgilerini çeker. Daha sonra, tüm ilanların fiyatlarının ortalamasını ve toplam fiyatını hesaplar ve ekrana yazdırır.

Kod, "Class1" adlı başka bir sınıfın "GetIlanUrlsAsync" adlı bir metodu aracılığıyla ilan URL'lerini alır. Bu metod, web sayfasından HTML kodunu indirir, HtmlAgilityPack kütüphanesi kullanarak ilanların URL'lerini seçer ve bir liste olarak döndürür.

Daha sonra, her bir ilan URL'si için "Class2" adlı başka bir sınıfın "GetIlanAsync" adlı asenkron metodu çağrılır. Bu metot, ilgili ilan sayfasından HTML kodunu indirir, ilanın adını ve fiyatını seçer ve bir "Ilan" nesnesi olarak döndürür. Bu ilanlar, bir liste olarak toplanır ve sonunda ortalama fiyat ve toplam fiyat hesaplanır ve ekrana yazdırılır.

Bu kod, asenkron işlemleri kullandığı için ilan bilgilerini daha hızlı çeker ve programın donmasını önler. Ayrıca, CultureInfo.InvariantCulture kullanarak sayı formatlarını işler ve Regex kullanarak ilan adındaki gereksiz karakterleri kaldırır.*/



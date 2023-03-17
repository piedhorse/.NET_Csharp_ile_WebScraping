
# .NET_Csharp_ile_WebScraping
Merhaba, bu Program  HtmlAgilityPack kullanarak bir web sitesinden bilgi çıkarmak için kullanılıyor. Daha spesifik olarak, [arabam.com](http://arabam.com/) sitesinden araç ilanlarındaki isimleri ve fiyatları çıkarmak için kullanılıyor.



![Logo](https://z2c2b4z9.stackpathcdn.com/images/logo256X256.png)  ![Logo](https://z2c2b4z9.stackpathcdn.com/images/logo256X256.png)

    
## HtmlAgilityPack Nuget Paketini Projenize yukleyin

```bash
 > dotnet add package HtmlAgilityPack --version 1.11.46
```
veya
```bash
PM> NuGet\Install-Package HtmlAgilityPack -Version 1.11.46
```
komutları ile projenize bu paketi indirebilirsiniz.



Bu işlem, HtmlWeb kullanılarak web sitesi yüklenerek, XPath kullanılarak ilgili HTML düğümleri seçilerek ve ardından bu düğümlerin metin değerleri ayrıştırılarak yapılıyor.

Class2 adlı statik bir sınıf tek bir yöntem içeriyor. Bu yöntem GetIlanAsync adını taşıyor ve tek bir argüman olan [arabam.com](http://arabam.com/)'daki bir ilanın URL'sini alıyor. Yöntem, bir veya daha fazla Ilan nesnesi içeren Task<List<Ilan>> nesnesi döndürür. List<Ilan> nesnesi, her biri bir araç ilanının adını ve fiyatını temsil eden Ilan nesnelerini içerir.

Yöntem, ilk olarak HtmlWeb kullanarak sağlanan URL'den HTML belgesini yükler. Daha sonra XPath sorgusu kullanarak ilanın adı ve fiyat bilgilerini içeren HTML düğümünü seçer. İsim ve fiyat bilgilerini seçmek için ayrı XPath sorguları kullanır.

Her bir ilan için, ad ve fiyat düğümlerinin metin değerleri çıkartılır, gereksiz boşluk veya karakterlerden temizlenir ve fiyatı InvariantCulture ile double.TryParse kullanarak double türüne dönüştürür. Ardından, çıkarılan ad ve fiyat değerleriyle yeni bir Ilan nesnesi oluşturulur ve Ilan nesneleri listesine eklenir.

Yöntem ayrıca, StreamWriter nesnesi kullanarak çıkarılan ad ve fiyat değerlerini "ilanlar.txt" adlı bir metin dosyasına yazar.

Genel olarak, bu Program HtmlAgilityPack ve XPath kullanarak bir HTML belgesinden bilgi çıkarmayı ve bu bilgiyi bir dosyaya yazmayı nasıl yapacağımızı gösteriyor.


## Proje Kaynak Kodları ve yorum satırları 
## Program.cs
```csharp 
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
            Console.WriteLine("https://www.arabam.com Vitrin Bolumu ilan bilgileri getiriliyor");
            Console.Write("Yukleniyor");

            for (int i = 0; i < 10; i++)
            {
                Console.Write("~");
                Thread.Sleep(200); 
            }

            Console.WriteLine("Basarili!");
            // Arabam.com'dan ilan URL'lerini al
            List<string> ilanUrls = await Class1.GetIlanUrlsAsync();

            Thread.Sleep(2000);
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




```
Bu kod, Arabam.com web sitesindeki Vitrin bölümündeki ilanlar için ilan URL'lerini alır ve her bir URL'yi kullanarak ilan bilgilerini çeker. Daha sonra, tüm ilanların fiyatlarının ortalamasını ve toplam fiyatını hesaplar ve ekrana yazdırır.

Kod, "Class1" adlı başka bir sınıfın "GetIlanUrlsAsync" adlı bir metodu aracılığıyla ilan URL'lerini alır. Bu metod, web sayfasından HTML kodunu indirir, HtmlAgilityPack kütüphanesi kullanarak ilanların URL'lerini seçer ve bir liste olarak döndürür.

Daha sonra, her bir ilan URL'si için "Class2" adlı başka bir sınıfın "GetIlanAsync" adlı asenkron metodu çağrılır. Bu metot, ilgili ilan sayfasından HTML kodunu indirir, ilanın adını ve fiyatını seçer ve bir "Ilan" nesnesi olarak döndürür. Bu ilanlar, bir liste olarak toplanır ve sonunda ortalama fiyat ve toplam fiyat hesaplanır ve ekrana yazdırılır.

Bu kod, asenkron işlemleri kullandığı için ilan bilgilerini daha hızlı çeker ve programın donmasını önler. Ayrıca, CultureInfo.InvariantCulture kullanarak sayı formatlarını işler ve Regex kullanarak ilan adındaki gereksiz karakterleri kaldırır.
## Class2.cs
  ```csharp
 using System;
using System.Collections.Generic;
using System.Text;


using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    public static class Class2
    {
        public static async Task<List<Ilan>> GetIlanAsync(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(url);

            List<Ilan> ilanlar = new List<Ilan>();

            // Web sayfasından gelen HTML dokümanı üzerinde XPath ile belirtilen elemanları seçiyoruz

            var ads = doc.DocumentNode.SelectNodes("//*[@id=\"wrapper\"]/div[2]/div[3]/div/div[1]");

            // StreamWriter nesnesi tanımlanıyor
            using (StreamWriter outputFile = new StreamWriter("ilanlar.txt", true))
            {
                // Seçilen her bir ilan için aşağıdaki işlemler yapılıyor

                foreach (var adss in ads)
                {
                    // İlanın ismi seçiliyor ve yazdırılıyor

                    var name = adss.ParentNode.SelectSingleNode("//*[@id=\"wrapper\"]/div[2]/div[3]/div/div[1]/p");
                    if (name != null)
                    {
                        string ilanName = name.InnerText.TrimEnd('.', ' ', '\n', '\r', '\t', '\0');
                        Console.WriteLine("Name: " + ilanName);
                        outputFile.WriteLine("Name: " + ilanName);
                    }

                    // İlanın fiyatı seçiliyor ve yazdırılıyor

                    var price = adss.ParentNode.SelectSingleNode("//*[@id=\"js-hook-for-observer-detail\"]/div[2]/div[1]/div/div/text()");
                    if (price != null)
                    {
                        string priceText = price.InnerText.Trim().Replace("TL", "").Replace(".", "");
                        priceText = priceText.Replace(".", ""); // Nokta karakterini kaldır
                     
                        // İlan fiyatı sayısal bir değere dönüştürülüyor ve yazdırılıyor

                        double ilanPrice;
                        if (double.TryParse(priceText, NumberStyles.Float, CultureInfo.InvariantCulture, out ilanPrice))
                        {
                            Console.WriteLine("Price: " + ilanPrice.ToString("#.###"));
                            outputFile.WriteLine("Price: " + ilanPrice.ToString("#.###"));
                            string ilanName = Regex.Replace(name.InnerText, @"[\n\r\t]+", "").TrimEnd('.', ' ');
                            ilanlar.Add(new Ilan(ilanName, ilanPrice));

                        }
                        else
                        {
                            Console.WriteLine("Invalid price format for: " + priceText);
                        }
                    }
                    Console.WriteLine("=======================================================================");
                }
                // StreamWriter nesnesi kapatılıyor
                outputFile.Close();
            }

            return ilanlar;
        }

    }
}

```
Bu kod, "HtmlAgilityPack" kütüphanesi kullanılarak bir web sitesinden ilanların isimleri ve fiyatları çekmek için yazılmış bir C# kodudur.

"GetIlanAsync" adlı bir metod, bir ilanın web sayfasının URL'sini alır ve ilanın adını ve fiyatını bir liste olarak döndürür.

İlk olarak, HtmlWeb sınıfından bir nesne oluşturulur ve bu nesne aracılığıyla verilen URL'den web sayfası yüklenir.

Daha sonra, XPath ifadeleri kullanarak ilanların isimleri ve fiyatları seçilir. İlanların isimleri ve fiyatları seçildikten sonra, StreamWriter sınıfı kullanılarak ilanlar.txt dosyasına yazdırılır.

Son olarak, her bir ilan için bir Ilan nesnesi oluşturulur ve bu nesneler bir liste içinde saklanır. Bu liste, metodun döndürdüğü sonuçtur.
## Class1.cs


```csharp
 using System;
using System.Collections.Generic;
using System.Text;

using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApp1
{
    public static class Class1
    {
       public static async Task<List<string>> GetIlanUrlsAsync()
        {
            HtmlWeb web = new HtmlWeb();
            int maxRetries = 3; // İsteği en fazla 3 kez tekrar deneyecek
            int retryCount = 0;// Şu ana kadar yapılan deneme sayısı
            while (retryCount < maxRetries)
            {
                try
                {
                    HtmlDocument doc = await web.LoadFromWebAsync("https://www.arabam.com");
                    // ilanUrls adında bir List<string> nesnesi tanımlanıyor
                    var ilanUrls = new List<string>();
                    // XPath ifadesiyle belirtilen ilanlar div elementlerinin linkleri toplanıyor

                    var divs = doc.DocumentNode.SelectNodes("//*[@id=\"wrapper\"]/div/div/div/div/div/div/div/div/div/div[1]/a");
                    // Toplanan her bir link ilanUrls listesine ekleniyor
                    foreach (var div in divs)
                    {
                        var ilanUrl = div.GetAttributeValue("href", string.Empty);
                        if (!string.IsNullOrEmpty(ilanUrl))
                        {
                            ilanUrls.Add(ilanUrl);
                        }
                    }
                    // elde edilen ilan url'leri geri döndürülüyor

                    return ilanUrls;
                }
                catch (Exception ex)
                {
                    // Hata mesajı yazdırılıyor

                    Console.WriteLine($"Web isteği sırasında hata oluştu: {ex.Message}");

                    // Yeniden deneme yapılacağına dair mesaj yazdırılıyor

                    Console.WriteLine($"Yeniden deneme yapılıyor ({retryCount + 1}/{maxRetries})...");

                    // Deneme sayısı arttırılıyor

                    retryCount++;

                    // 3 saniye bekletiliyor

                    Thread.Sleep(3000);
                }
            }
            // İstenilen sayıda tekrar denemeden sonra bile başarısızlık yaşanırsa bir Exception fırlatılıyor.

            throw new Exception($"Web isteği {maxRetries} kez denendi fakat başarısız oldu.");
        }
    }
}

```
Bu kod, "https://www.arabam.com" web sitesinden araç ilanlarının sayfa linklerini almayı sağlar.

İlk olarak, HtmlWeb sınıfından bir nesne oluşturulur ve ardından en fazla 3 kez yeniden deneme yapılacak şekilde bir yeniden deneme sayacı tanımlanır. Ardından, while döngüsü, yeniden deneme sayısı limitine ulaşana kadar web sayfasını yüklemeye çalışır.

Web sayfası yüklendikten sonra, ilan linklerinin listesi olan ilanUrls oluşturulur. Ardından, web sayfasındaki ilan linklerini içeren div etiketleri seçilir ve ilgili linkler ilanUrls listesine eklenir.

Daha sonra, ilanUrls listesi geri döndürülür. Ancak, web isteği sırasında hata oluşursa, hata mesajı yazdırılır, yeniden deneme sayacı artırılır ve 3 saniye beklendikten sonra yeniden deneme yapılır.

Yeniden deneme sayısı limitine ulaşıldığında, bir istisna fırlatılır ve "Web isteği {maxRetries} kez denendi fakat başarısız oldu." mesajı yazdırılır.

## Ilan.cs

```csharp
 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public class Ilan
    {

        public string Name { get; set; }
        public double Price { get; set; }

        public Ilan(string name, double price)
        {
            Name = name;
            Price = price;
        }
    }


}

```

 Bu kod, "Ilan" adında bir sınıf tanımlamaktadır. Bu sınıf, bir araç ilanının ismi ve fiyatını tutmak için kullanılır. Sınıf iki özellik içerir: "Name" ve "Price". "Name" özelliği, ilanın ismini, "Price" özelliği ise ilanın fiyatını tutar.

"Ilan" sınıfının kurucu metodu, isim ve fiyat parametreleri alır ve sınıfın özelliklerine atama yapar. Bu sayede, "Ilan" nesneleri oluşturulurken bu iki bilgi özelliklere kaydedilir.

## Proje Demo
https://user-images.githubusercontent.com/96746943/226051664-5890e482-5f79-4e4a-bed6-315320e5621b.mp4


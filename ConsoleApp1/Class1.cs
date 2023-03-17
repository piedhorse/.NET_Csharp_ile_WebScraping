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
/*Bu kod, "https://www.arabam.com" web sitesinden araç ilanlarının sayfa linklerini almayı sağlar.

İlk olarak, HtmlWeb sınıfından bir nesne oluşturulur ve ardından en fazla 3 kez yeniden deneme yapılacak şekilde bir yeniden deneme sayacı tanımlanır. Ardından, while döngüsü, yeniden deneme sayısı limitine ulaşana kadar web sayfasını yüklemeye çalışır.

Web sayfası yüklendikten sonra, ilan linklerinin listesi olan ilanUrls oluşturulur. Ardından, web sayfasındaki ilan linklerini içeren div etiketleri seçilir ve ilgili linkler ilanUrls listesine eklenir.

Daha sonra, ilanUrls listesi geri döndürülür. Ancak, web isteği sırasında hata oluşursa, hata mesajı yazdırılır, yeniden deneme sayacı artırılır ve 3 saniye beklendikten sonra yeniden deneme yapılır.

Yeniden deneme sayısı limitine ulaşıldığında, bir istisna fırlatılır ve "Web isteği {maxRetries} kez denendi fakat başarısız oldu." mesajı yazdırılır.
*/
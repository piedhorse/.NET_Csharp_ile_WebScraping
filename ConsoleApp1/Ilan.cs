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
/*
 Bu kod, "Ilan" adında bir sınıf tanımlamaktadır. Bu sınıf, bir araç ilanının ismi ve fiyatını tutmak için kullanılır. Sınıf iki özellik içerir: "Name" ve "Price". "Name" özelliği, ilanın ismini, "Price" özelliği ise ilanın fiyatını tutar.

"Ilan" sınıfının kurucu metodu, isim ve fiyat parametreleri alır ve sınıfın özelliklerine atama yapar. Bu sayede, "Ilan" nesneleri oluşturulurken bu iki bilgi özelliklere kaydedilir.
*/
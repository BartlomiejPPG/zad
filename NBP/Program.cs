using System.Xml;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
namespace NBP
{
    class Program
    {
        static async Task Main()
        {
            try
            {
                double kursUSD = await PobierzKursUSD();
                Console.Write("Podaj kwotę w PLN: ");
                double kwotaPLN = Convert.ToDouble(Console.ReadLine());
                double kwotaUSD = kwotaPLN / kursUSD;
                Console.WriteLine("Kwota w USD: " + kwotaUSD.ToString("0.00"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wystąpił błąd: " + ex.Message);
            }
        }
        static async Task<double> PobierzKursUSD()
        {
            string url = "http://www.nbp.pl/kursy/kursya.html";
            // Tworzenie klienta HTTP
            using (HttpClient client = new HttpClient())
            {
                // Wysłanie żądania GET i pobranie zawartości strony
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Upewnienie się, że odpowiedź jest udana
                string responseBody = await response.Content.ReadAsStringAsync();
                // Analiza HTML za pomocą HtmlAgilityPack
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(responseBody);
                // Wyszukanie elementu zawierającego kurs USD
                HtmlNode kursNode = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='table table-hover table-striped table-bordered']//td[text()='dolar amerykański']/following-sibling::td[2]");
                if (kursNode != null)
                {
                    return Convert.ToDouble(kursNode.InnerText);
                }
                else
                {
                    throw new Exception("Nie udało się odnaleźć kursu USD na stronie.");
                }
            }
        }
    }
}

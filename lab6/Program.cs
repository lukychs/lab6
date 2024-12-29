using System.Net.Http.Headers;
using System.Text.Json.Nodes;

struct Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public double Temp { get; set; }
    public string Description { get; set; }


    public Weather(string country, string name, double temp, string description)
    {
        Country = country;
        Name = name;
        Temp = temp;
        Description = description;
    }
    public override string ToString()
    {
        return $"Country: {Country}, Name: {Name}, Temp: {Temp}, Description: {Description}\n";
    }
}
class Program
{
    static void Main(string[] args)
    {
        string apiKey = "c30b0660de71e1821d7da8149a211b98";
        string URL = $"https://api.openweathermap.org/data/2.5/weather";

        Weather[] weathers = new Weather[50];
        Random random = new Random();
        int i = 0;
        while (i < weathers.Length)
        {
            {
                double minLatitude = -90;
                double maxLatitude = 90;
                double minLongtitude = -180;
                double maxLongtitude = 180;
                double latitude = random.NextDouble() * (maxLatitude - minLatitude) + minLatitude;
                double longtitude = random.NextDouble() * (maxLongtitude - minLongtitude) + minLongtitude;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string urlParameters = $"?lat={latitude}&lon={longtitude}&appid={apiKey}";
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;
                //Console.WriteLine(response.StatusCode);
                //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    var json = JsonObject.Parse(data);
                    Weather res = new Weather();
                    res.Country = (string)json["sys"]["country"];
                    res.Name = (string)json["name"];
                    res.Temp = (double)json["main"]["temp"];
                    res.Description = (string)json["weather"][0]["main"];
                    if (res.Country == "" || res.Name == "") continue;
                    else
                    {
                        weathers[i] = res;
                        i += 1;
                    }

                    Console.WriteLine(i);
                }
                else
                {
                    Console.WriteLine("Not succesful");
                }
                client.Dispose();
            }
            Console.WriteLine("Succesful");
        }
        Console.WriteLine("Minimum temperature: ");
        Weather minTemp = (from w in weathers
                           orderby w.Temp
                           select w).First();
        Console.WriteLine(minTemp);
        Console.WriteLine("Maximum temperature: ");
        Weather maxTemp = weathers.OrderByDescending(w => w.Temp).First();
        Console.WriteLine(maxTemp);
        Console.WriteLine("Average:");
        double average = weathers.Average(w => w.Temp);
        Console.WriteLine(average);
        Console.WriteLine("Quantity of countries: ");
        int quantity = weathers.GroupBy(w => w.Country).Count();
        Console.WriteLine(quantity);
        var last = weathers.Where(w => w.Description == "Clear sky" || w.Description == "Rain" || w.Description == "Few clouds");
        if (last.Any())
        {
            Weather weather = last.First();
            Console.WriteLine("Point with clear sky or rain or clouds ");
            Console.WriteLine(weather);
        }
        else
        {
            Console.WriteLine("No such place found");
        }
        Console.WriteLine("List of indications: ");
        for (int j = 0; j < weathers.Length; j++)
        {
            Console.WriteLine($"{j}: {weathers[j]}");
            Console.WriteLine();
        }
    }
}

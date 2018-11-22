using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace StarWars
{
    class Program
    {
        static void Main(string[] args)
        {



            Console.WriteLine("введите id  планеты");
            int idPlanet = int.Parse(Console.ReadLine());
            StarWarsResponse starWarsResponse = new StarWarsResponse();
            List<People> list = new List<People>();
            starWarsResponse.Planet(idPlanet, list);
            Console.WriteLine("введите id чела: ");
            int idPerson = int.Parse(Console.ReadLine());
            starWarsResponse.People(idPerson);
        }
    }
}










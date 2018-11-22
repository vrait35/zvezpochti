using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace StarWars
{
    public class StarWarsResponse
    {

        public Planet Planet(int index,List<People> peoplesInPlanet)
        {           
            try
            {
                WebRequest planets = WebRequest.Create($"https://swapi.co/api/planets/{index}/");
                WebResponse responsePlanet = planets.GetResponse();

                Stream streamPlanet = responsePlanet.GetResponseStream();
                StreamReader readerPlanet = new StreamReader(streamPlanet);
                var resultPlanet = readerPlanet.ReadToEnd();

                //List<People> peoplesInPlanet = new List<People>();

                Planet objectPlanet = JsonConvert.DeserializeObject<Planet>(resultPlanet);

                using (SqlConnection sqlConnection = new SqlConnection())
                {                   
                    string connectionString = ConfigurationManager.ConnectionStrings["UsserConnectionString"].ConnectionString;
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    //sqlCommand
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "select * from Planets";
                    //sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    int isExist = 0;
                    while (reader.Read())
                    {
                        if (int.Parse(reader["id_Planet"].ToString()) == index) isExist++;                        
                    }                    
                    reader.Close();
                    if (isExist == 0)
                    {
                     //   sqlCommand.CommandText = "insert  into Planets( id_Planet, climate, diameter, gravity, name, orbital_period, population, rotation_period," +
                     //       "surface_water, terrain, url) values(" + index.ToString() + ",'" + objectPlanet.Climate + "'," + objectPlanet.Diameter + "," +
                     //       objectPlanet.Gravity + ",'" + objectPlanet.Name + "'," + objectPlanet.OrbitalPeriod + "," + objectPlanet.Population + "," +
                     //       objectPlanet.RotationPeriod + "," + objectPlanet.SurfaceWater + ",'" + objectPlanet.Terrain + "','" +
                     //       objectPlanet.Url + "')";
                     //   sqlCommand.ExecuteNonQuery();


                        sqlCommand.CommandText = "insert into Planets(id_Planet, climate, diameter, name,url) " +
                            "values(@id_Planet, @climate, @diameter, @name, @iji)";
                        var idParameteer = sqlCommand.CreateParameter();
                        idParameteer.ParameterName = "@id";
                        idParameteer.DbType = System.Data.DbType.Int32;
                        idParameteer.Value = 9;
                        sqlCommand.Parameters.Add(idParameteer);

                        sqlCommand.Parameters.Add(new SqlParameter() {
                            ParameterName = "@id_Planet",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Value = index,
                            IsNullable = false
                        });
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@climate",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value = objectPlanet.Climate,
                            IsNullable = false
                        });
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@diameter",
                            SqlDbType = System.Data.SqlDbType.Float,
                            Value = float.Parse(objectPlanet.Diameter),
                            IsNullable = false
                        });                      
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@name",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value = objectPlanet.Name,
                            IsNullable = false
                        });
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@iji",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value = objectPlanet.Url,
                            IsNullable = false
                        });

                        sqlCommand.ExecuteNonQuery();
                    }
                }



                Console.WriteLine("obj:  "+objectPlanet.Name);
                Console.WriteLine("count::: "+objectPlanet.Residents.Count);
                Console.WriteLine("objResident : "+objectPlanet.Residents[0].ToString());
                //Console.WriteLine("planet: "+objectPlanet.Name+"pers: "+objectPlanet.Residents[0]);
                int[] array = new int[objectPlanet.Residents.Count];
                string str="";
                int indexArray = 0;
                if (objectPlanet.Residents.Count > 0)
                {
                    Console.WriteLine("tut");
                    foreach (var item in objectPlanet.Residents)
                    {
                        for(int i=item.Length-2; ;)
                        {
                            Console.WriteLine("item[i]="+item[i]);
                            if (item[i] == '/') break;
                            str += item[i];
                            i--;
                        }
                        Console.WriteLine("str: "+str);
                        if (str.Length > 0)
                        {
                            Array.Reverse(str.ToArray());
                            array[indexArray] = int.Parse(str);
                            indexArray++;
                        }
                        //else
                        //{
                        //    array[indexArray] = 0;
                        //}
                      
                        WebRequest request = WebRequest.Create(item);
                        WebResponse response = request.GetResponse();

                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        var result = reader.ReadToEnd();

                        People people = JsonConvert.DeserializeObject<People>(result);
                        Console.WriteLine("peopleName:"+people.Name);
                        peoplesInPlanet.Add(people);
                    }
                                        
                    
                }
                using (SqlConnection sqlConnection = new SqlConnection())
                {
                    //string isTest = ConfigurationManager.AppSettings["isTest"].ToString();
                    string connectionString = ConfigurationManager.ConnectionStrings["UsserConnectionString"].ConnectionString;
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    //sqlCommand
                    sqlCommand.Connection = sqlConnection;
                    SqlDataReader reader1 = null;
                    for (int i = 0; i < indexArray; i++)
                    {
                        sqlCommand.CommandText = "update People set id_Planet=" + index.ToString() + "where id_Person=" + array[i].ToString();
                        reader1 = sqlCommand.ExecuteReader();
                        reader1.Close();
                    }
                    sqlCommand.CommandText = "select*from People";
                    reader1 = sqlCommand.ExecuteReader();
                    List<int> listIdPerson = new List<int>();
                    while (reader1.Read())
                    {
                        listIdPerson.Add(int.Parse(reader1["id_Person"].ToString()));
                    }
                    reader1.Close();
                    foreach (int num in listIdPerson)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (num == array[i]) array[i] = -1;
                        }
                    }
                    SqlCommand sqlCommand1;
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > 0)
                        {
                            Console.WriteLine("CREATE PERSON");
                            sqlCommand1 = new SqlCommand();
                            sqlCommand1.Connection = sqlConnection;
                            sqlCommand1.CommandText = "insert People(id_Person,id_Planet,birth_year,eye_color,gender,hair_color,height," +
                                "homeworld,mass,name,skin_color,url) values('" +
                            array[i].ToString() + "','" + index.ToString() + "','" + peoplesInPlanet[i].BirthYear + "','" + peoplesInPlanet[i].EyeColor + "','" +
                            peoplesInPlanet[i].Gender + "','" + peoplesInPlanet[i].HairColor + "','" + peoplesInPlanet[i].Height + "','" +
                            peoplesInPlanet[i].Homeworld + "','" + peoplesInPlanet[i].Mass + "','" + peoplesInPlanet[i].Name + "','" +
                            peoplesInPlanet[i].SkinColor + "','"  + peoplesInPlanet[i].Url+ "')";
                            reader1 = sqlCommand1.ExecuteReader();
                            reader1.Close();
                        }
                    }
                    reader1.Close();
                }
                return objectPlanet;
            }
            catch (WebException ex) when (ex.Response != null)
            {
                Console.WriteLine("cath");
                return null ;
            }
        }


        public People People(int index)
        {
            try
            {
                WebRequest peopls = WebRequest.Create($"https://swapi.co/api/people/{index}/");
                WebResponse responsePeopls = peopls.GetResponse();

                Stream streamPeople = responsePeopls.GetResponseStream();
                StreamReader readerPeople = new StreamReader(streamPeople);
                var resultPeople = readerPeople.ReadToEnd();

                People pl = JsonConvert.DeserializeObject<People>(resultPeople);
                using (SqlConnection sqlConnection = new SqlConnection())
                {                   
                    string connectionString = ConfigurationManager.ConnectionStrings["UsserConnectionString"].ConnectionString;
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand();
                    //sqlCommand
                    int isExist = 0;
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandText = "select * from People";
                    //sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        if (int.Parse(reader["id"].ToString()) == index) isExist++;                        
                    }
                    reader.Close();
                    if (isExist == 0)
                    {
                        Console.WriteLine("pd.name: "+pl.Name);
                        sqlCommand.CommandText = "insert into People(id_Person,name)" +
                            "values(@id_Person,@name)";

                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@id_Person",
                            SqlDbType = System.Data.SqlDbType.Int,
                            Value = index,
                            IsNullable = false
                        });

                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@name",
                            SqlDbType = System.Data.SqlDbType.VarChar,
                            Value=pl.Name,
                            IsNullable = true
                        });                      
                        sqlCommand.ExecuteNonQuery();
                    }                  
                }
                return pl;
            }
            catch (WebException ex) when (ex.Response != null)
            {
                return null;
            }

            
        }


        public Starships Starship(int index)
        {

            try
            {
                WebRequest starships = WebRequest.Create($"https://swapi.co/api/planets/{index}/");
                WebResponse responseStarships = starships.GetResponse();

                Stream streamStarship = responseStarships.GetResponseStream();
                StreamReader readerStarship = new StreamReader(streamStarship);
                var resultStarship = readerStarship.ReadToEnd();

                List<People> peoplesInStarship = new List<People>();

                Starships objectStarship = JsonConvert.DeserializeObject<Starships>(resultStarship);

                if (objectStarship.Pilots.Count > 0)
                {
                    foreach (var item in objectStarship.Pilots)
                    {
                        WebRequest request = WebRequest.Create(item);
                        WebResponse response = request.GetResponse();

                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        var result = reader.ReadToEnd();

                        People people = JsonConvert.DeserializeObject<People>(result);
                        peoplesInStarship.Add(people);
                    }
                }

                return objectStarship;
            }
            catch (WebException ex) when (ex.Response != null)
            {
                return null;
            }


        }


    }
}

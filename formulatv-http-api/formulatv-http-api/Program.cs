using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArangoDB.Client;
using formulatv_datamodel;
using IMDb_local_datamodel;

namespace formulatv_http_api {
    public class Program {
        public static void Main(string[] args) {
            List<string> allLinks = new List<string>();
            Random rnd = new Random();

            for (char letter = 'a'; letter <= 'z'; letter++) {
                Console.Write("Getting links for letter: " + letter);
                allLinks.AddRange(Utils.GetSeriesByLetter(letter));
                Thread.Sleep(1500 + 100 * rnd.Next(0, 3));
                Console.WriteLine(" DONE!");
            }



            /* ServicePointManager.Expect100Continue = true;
             ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

             const string URI = "http://localhost:8529";
             ArangoDatabase.ChangeSetting(s => {
                 s.Database = "imdb";
                 s.Url = URI;
                 s.Credential = new NetworkCredential("root", "");
                 s.SystemDatabaseCredential = new NetworkCredential("root", "");
                 s.WaitForSync = true;
             });
             Random rnd = new Random();

             List<Filtered_title_es> filteredTitles;
             using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                 filteredTitles = db.CreateStatement<Filtered_title_es>("for r in filtered_title_es filter r.titleType =='tvSeries' and r.startYear>1980 sort r.title ASC return r").ToList();
             }

             for (int i= 0; i<filteredTitles.Count; i++) {
                 Console.WriteLine("[{0}/{1}] Getting information for {2}...", i, filteredTitles.Count, filteredTitles[i].title);
                 FormulaTV_Title search = Utils.FormulaTV_Search(filteredTitles[i].title);
                 if (search != null && search.titulo!=null) {
                     search._key = filteredTitles[i]._key;
                     Utils.FormulaTV_GetEpisodes(ref search);
                     Utils.FormulaTV_GetAudiences(ref search);
                     using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                         try {
                             db.Insert<FormulaTV_Title>(search);
                         } catch (Exception ex) {
                             try {
                                 db.UpdateById<FormulaTV_Title>(search._key, search);
                             } catch (Exception ex2) { }
                         }
                     }
                     Console.WriteLine("[{0}/{1}] Getting information for {2}... DONE!", i, filteredTitles.Count, filteredTitles[i].title);
                 }else {
                     Console.WriteLine("[{0}/{1}] Getting information for {2}... ERROR\n", i, filteredTitles.Count, filteredTitles[i].title);
                 }

                 Thread.Sleep(1500 + 100 * rnd.Next(0, 3));
             }
             */
            //FormulaTV_Title title = Utils.FormulaTV_Search("LOS HOMBRES DE PACO");
            // FormulaTV_Title title = Utils.FormulaTV_Search("Ana y los siete");
            /*if(title != null) {
                Utils.FormulaTV_GetEpisodes(ref title);
                Utils.FormulaTV_GetAudiences(ref title);

            }*/
            //FormulaTV_Title title = Utils.FormulaTV_Search("farmacia de guardia");

            //FormulaTV_Title title = Utils.FormulaTV_Search("LOS HOMBRES DE PACO");
            //FormulaTV_Title title = Utils.FormulaTV_Search("ana y los 7");

            Console.WriteLine("---------------------LISTO-------------------");
            Console.ReadKey();
        }


       

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArangoDB.Client;
using formulatv_datamodel;
using formulatv_http_api;

namespace formulatv_processor {
    class Program {
        public static void Main(string[] args) {

            /*const string URI = "http://localhost:8529";
            ArangoDatabase.ChangeSetting(s => {
                s.Database = "imdb";
                s.Url = URI;
                s.Credential = new NetworkCredential("root", "");
                s.SystemDatabaseCredential = new NetworkCredential("root", "");
                s.WaitForSync = true;
            });*/

            /*************just for update repartos******************************/
            /*List<FormulaTV_Title> listToProcess2 = null;
            int remaining2 = 0;
            do {
                using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                    try {
                        listToProcess2 = db.CreateStatement<FormulaTV_Title>("FOR r IN formulaTV FILTER r.actorsUrls==null LIMIT 100 return r").ToList();
                    } catch { }
                }
                Random rnd = new Random();
                for (int i = 0; i < listToProcess2?.Count; i++) {
                    FormulaTV_Title title_aux = listToProcess2[i];
                    Console.WriteLine("Getting actors for: " + title_aux?.titulo);
                    Utils.FormulaTV_GetActors(ref title_aux);
                    using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                        try {
                            db.UpdateById<FormulaTV_Title>(listToProcess2[i]._key, title_aux);
                        } catch { }
                    }
                    Thread.Sleep(1500 + 100 * rnd.Next(0, 3));
                }
                using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                    try {
                        listToProcess2 = db.CreateStatement<FormulaTV_Title>("FOR r IN formulaTV FILTER r.actorsUrls==null LIMIT 100 return r").ToList();
                        remaining2 = db.CreateStatement<int>("RETURN COUNT(FOR r IN formulaTV FILTER r.actorsUrls==null return r)").ToList().FirstOrDefault();
                        Console.WriteLine("REMAINING: " + remaining2);

                    } catch { }
                }
            } while (listToProcess2.Count > 0);*/
            /*************END just for update repartos******************************/

            //Fifth Step - Get People Info
            /*List<FormulaTV_Person> listToProcess = null;
            int remaining = 0;
            do {
                using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                    try {
                        listToProcess = db.CreateStatement<FormulaTV_Person>("FOR r IN formulaTV_people FILTER r.aka==null LIMIT 100 return r").ToList();
                    } catch { }
                }
                Random rnd = new Random();
                for (int i = 0; i < listToProcess?.Count; i++) {
                    FormulaTV_Person person_aux = new FormulaTV_Person();
                    person_aux = Utils.FormulaTV_PeopleByUrl(listToProcess[i].formulaTV_URL);
                    if (person_aux != null) {
                        person_aux.formulaTV_URL = listToProcess[i].formulaTV_URL;
                        person_aux._key = listToProcess[i]._key;
                        Console.WriteLine("Getting information for: " + person_aux?.aka);

                        using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                            try {
                                db.UpdateById<FormulaTV_Person>(listToProcess[i]._key, person_aux);
                            } catch { }
                        }
                    } else {
                        Console.WriteLine("{0} NO EXISTE", listToProcess[i].formulaTV_URL);
                    }
                    Thread.Sleep(1500 + 100 * rnd.Next(0, 3));
                }
                using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                    try {
                        listToProcess = db.CreateStatement<FormulaTV_Person>("FOR r IN formulaTV_people FILTER r.aka==null LIMIT 100 return r").ToList();
                        remaining = db.CreateStatement<int>("RETURN COUNT(FOR r IN formulaTV_people FILTER r.aka==null return r)").ToList().FirstOrDefault();
                        Console.WriteLine("REMAINING: " + remaining);

                    } catch { }
                }
            } while (listToProcess.Count > 0);
            */

            //Fourth Step - Get People
            /*Random rnd = new Random();

            for (char letter = 'a'; letter <= 'z'; letter++) {
                Console.WriteLine("Getting links for letter: " + letter);
                List<string> urls = Utils.GetPeopleByLetter(letter);
                if (urls != null && urls.Count>0) {
                    List<FormulaTV_Person> formulaTV_People = new List<FormulaTV_Person>();
                    foreach (string personUri in urls) {
                        FormulaTV_Person person = new FormulaTV_Person();
                        person.formulaTV_URL = personUri;
                        formulaTV_People.Add(person);
                    }
                    using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                        try {
                            db.InsertMultiple<FormulaTV_Person>(formulaTV_People);
                        } catch { }
                    }
                }
                //Thread.Sleep(1500 + 100 * rnd.Next(0, 3));
                Console.WriteLine("Getting links for letter: {0} DONE!",letter);
            }*/

            //Third Step - Get Series Information
            /*List<FormulaTV_Title> listToProcess = null;
            int remaining = 0;
            do {
                using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                    try {
                        listToProcess = db.CreateStatement<FormulaTV_Title>("FOR r IN formulaTV FILTER r.titulo==null LIMIT 100 return r").ToList();
                    } catch { }
                }
                Random rnd = new Random();
                for (int i = 0; i < listToProcess?.Count; i++) {
                    FormulaTV_Title title_aux = new FormulaTV_Title();
                    title_aux = Utils.FormulaTV_ByUrl(listToProcess[i].formulaTV_URL);
                    Console.WriteLine("Getting information for: " + title_aux?.titulo);
                    Utils.FormulaTV_GetActors(ref title_aux);
                    if (listToProcess[i].formulaTV_Type != "PROGRAMS") {
                        Utils.FormulaTV_GetEpisodes(ref title_aux);
                        Utils.FormulaTV_GetAudiences(ref title_aux);
                    }


                    using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                        try {
                            db.UpdateById<FormulaTV_Title>(listToProcess[i]._key, title_aux);
                        } catch { }
                    }
                    Thread.Sleep(1500 + 100 * rnd.Next(0, 3));
                }
                using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                    try {
                        listToProcess = db.CreateStatement<FormulaTV_Title>("FOR r IN formulaTV FILTER r.titulo==null LIMIT 100 return r").ToList();
                        remaining = db.CreateStatement<int>("RETURN COUNT(FOR r IN formulaTV FILTER r.titulo==null return r)").ToList().FirstOrDefault();
                        Console.WriteLine("REMAINING: " + remaining);

                    } catch { }
                }
            } while (listToProcess.Count > 0);*/


            //First Step - Get Series URLs
            /*List<string> seriesUri = new List<string>();
            Random rnd = new Random();

            for (char letter = 'a'; letter <= 'z'; letter++) {
                Console.Write("Getting links for letter: " + letter);
                seriesUri.AddRange(Utils.GetSeriesByLetter(letter));
                Thread.Sleep(1500 + 100 * rnd.Next(0, 3));
                Console.WriteLine(" DONE!");
            }

            List<FormulaTV_Title> formulaTV_Titles = new List<FormulaTV_Title>();
            foreach(string serieUri in seriesUri) {
                FormulaTV_Title title = new FormulaTV_Title();
                title.formulaTV_URL = serieUri;
                title.formulaTV_Type = "SERIES";
                formulaTV_Titles.Add(title);
            }
            
            using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                try {
                    db.InsertMultiple<FormulaTV_Title>(formulaTV_Titles);
                } catch { }
            }

            //Second Step - Get Programs URLs
            List<string> programsUri = new List<string>();

            for (char letter = 'a'; letter <= 'z'; letter++) {
                Console.Write("Getting links for letter: " + letter);
                seriesUri.AddRange(Utils.GetProgramsByLetter(letter));
                Thread.Sleep(1500 + 100 * rnd.Next(0, 3));
                Console.WriteLine(" DONE!");
            }

            formulaTV_Titles = new List<FormulaTV_Title>();
            foreach (string serieUri in seriesUri) {
                FormulaTV_Title title = new FormulaTV_Title();
                title.formulaTV_URL = serieUri;
                title.formulaTV_Type = "PROGRAMS";
                formulaTV_Titles.Add(title);
            }

            using (IArangoDatabase db = ArangoDatabase.CreateWithSetting()) {
                try {
                    db.InsertMultiple<FormulaTV_Title>(formulaTV_Titles);
                } catch { }
            }
            */

            Console.ReadLine();

        }
    }
}

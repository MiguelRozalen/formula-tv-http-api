using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using formulatv_datamodel;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace formulatv_http_api {
    public class Utils {

        const string FORMULA_TV_URI = "http://www.formulatv.com";
        const string FORMULA_TV_SERIES = "http://www.formulatv.com/series/";
        const string FORMULA_TV_PROGRAMAS = "http://www.formulatv.com/programas/";

        const string FORMULA_TV_BUSCADOR = "http://www.formulatv.com/buscar/?q=";
        const string FORMULA_TV_BUSCADOR_SERIES = "http://www.formulatv.com/buscar/series/?q=";
        const string FORMULA_TV_BUCADOR_GOOGLE = "https://www.google.com/search?q={0}+site%3Aformulatv.com";
        const string formatContainsSeries = "//*[contains(@class,'{0}')]";
        const string classToSinopsis = "txt";
        const string posterClass = "poster";
        const string coverlist = "coverlist";

        public static List<string> GetSeriesByLetter(char letter) {
            List<string> links = null;
            try {
                const string URL_SERIES = "http://www.formulatv.com/series/indice";
                links = GetIndexByURL(string.Format("{0}/{1}", URL_SERIES, letter));
            } catch {
                Console.WriteLine("ERROR Getting series for {0}", letter);
            }
            return links;
        }

        public static List<string> GetPeopleByLetter(char letter) {
            List<string> links = null;
            List<string> alllinks = new List<string>();
            Random rnd = new Random();
            try {
                const string URL_SERIES = "http://www.formulatv.com/personas/indice/";

                int page = 1;
                do {
                    Console.WriteLine("---LETTER: {0} & PAGE:{1}---", letter, page);
                    links = GetPeopleIndexByUrl(string.Format("{0}/{1}/{2}", URL_SERIES, letter, page));
                    if (links != null && links.Count > 0) {
                        alllinks.AddRange(links);
                    }
                    Thread.Sleep(1500 + 100 * rnd.Next(0, 3));
                    page++;
                }
                while (links != null && links.Count > 0);

            } catch {
                Console.WriteLine("ERROR Getting people for {0}", letter);
            }
            return alllinks;
        }

        private static List<string> GetPeopleIndexByUrl(string url) {
            List<string> links = null;
            try {
                // Create web request to obtain HTML.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                // With response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    // Response OK.
                    switch (response.StatusCode) {
                        case HttpStatusCode.OK:
                            // Obtain HTML.
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            if (response.CharacterSet == null) {
                                readStream = new StreamReader(receiveStream);
                            } else {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            // With HTML.
                            using (readStream) {
                                HtmlDocument htmlDocument = new HtmlDocument();
                                htmlDocument.LoadHtml(readStream.ReadToEnd());
                                string classToFind = "peoplelist";

                                links = new List<string>();

                                List<HtmlNode> htmlPeople = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToFind))?.ToList().FirstOrDefault()?.Descendants("a").ToList();
                                if (htmlPeople != null) {
                                    foreach (HtmlNode htmlNode in htmlPeople) {
                                        try {
                                            string linkNode = htmlNode.Attributes["href"]?.Value;
                                            if (linkNode != null) {
                                                links.Add(linkNode);
                                            }
                                        } catch { }
                                    }
                                }
                                break;
                            }
                        case HttpStatusCode.Moved:
                        default:
                            break;
                    }
                }
            } catch { }
            return links;
        }



        public static List<string> GetProgramsByLetter(char letter) {
            List<string> links = null;
            try {
                const string URL_PROGRAMS = "http://www.formulatv.com/programas/indice";
                links = GetIndexByURL(string.Format("{0}/{1}", URL_PROGRAMS, letter));
            } catch  {
                Console.WriteLine("ERROR Getting series for {0}", letter);
            }
            return links;
        }

        private static List<string> GetIndexByURL(string url) {
            List<string> links = null;
            try {
                // Create web request to obtain HTML.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                // With response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    // Response OK.
                    switch (response.StatusCode) {
                        case HttpStatusCode.OK:
                            // Obtain HTML.
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            if (response.CharacterSet == null) {
                                readStream = new StreamReader(receiveStream);
                            } else {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            // With HTML.
                            using (readStream) {
                                HtmlDocument htmlDocument = new HtmlDocument();
                                htmlDocument.LoadHtml(readStream.ReadToEnd());
                                string classToFind = "coverlist";

                                links = new List<string>();

                                List<HtmlNode> htmlAudiencias = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToFind))?.ToList().FirstOrDefault()?.Descendants("a").ToList();
                                foreach (HtmlNode htmlNode in htmlAudiencias) {
                                    try {
                                        string linkNode = htmlNode.Attributes["href"]?.Value;
                                        if (linkNode != null) {
                                            links.Add(linkNode);
                                        }
                                    } catch { }
                                }
                                break;
                            }
                        case HttpStatusCode.Moved:
                        default:
                            break;
                    }
                }
            } catch { }
            return links;
        }

        private static FormulaTV_Person ParsePeopleHtml(string html) {
            FormulaTV_Person result = null;
            try {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                result = new FormulaTV_Person();
                string classToFind = "topserie";
                List<HtmlNode> ps = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToFind))?.FirstOrDefault().Descendants("p")?.ToList();
                foreach (HtmlNode htmlNode in ps) {
                    try {
                        if (htmlNode.InnerText.Contains("Nombre: ")) {
                            result.name = htmlNode.InnerText.Replace("Nombre: ", "");
                        }
                        if (htmlNode.InnerText.Contains("Apellido: ")) {
                            result.surname = htmlNode.InnerText.Replace("Apellido: ", "");
                        }
                        if (htmlNode.InnerText.Contains("Profesión: ") || htmlNode.InnerText.Contains("Profesi&oacute;n: ")) {
                            result.profesion = htmlNode.InnerText.Replace("Profesión: ", "").Replace("Profesi&oacute;n: ", "");
                        }
                        if (htmlNode.InnerText.Contains("Origen: ")) {
                            result.origin = htmlNode.InnerText.Replace("Origen: ", "");
                        }
                        if (htmlNode.InnerText.Contains("Fecha de nacimiento: ")) {
                            result.birthday = htmlNode.InnerText.Replace("Fecha de nacimiento: ", "");
                        }

                        if (htmlNode.InnerText.Contains("Edad: ")) {
                            string edad = htmlNode.InnerText.Replace("Edad: ", "").Split(' ').FirstOrDefault().Replace('.', ',');
                            if (edad != null) {
                                result.age = int.Parse(edad, NumberStyles.Any, CultureInfo.InvariantCulture);
                            }
                        }
                        if (htmlNode.InnerText.Contains("Popularidad: ")) {
                            string ranking = htmlNode.InnerText.Replace("Popularidad: ", "").Split(' ').FirstOrDefault().Replace('.', ',');
                            if (ranking != null) {
                                result.ranking_popularidad = int.Parse(ranking, NumberStyles.Any, CultureInfo.InvariantCulture);
                            }
                        }
                        if (htmlNode.InnerText.Contains("Ranking votos: ")) {
                            string ranking_votes = htmlNode.InnerText.Replace("Ranking votos: ", "").Split(' ').FirstOrDefault().Replace('.', ',');
                            if (ranking_votes != null) {
                                result.ranking_votos = int.Parse(ranking_votes, NumberStyles.Any, CultureInfo.InvariantCulture);
                            }
                        }
                    } catch (Exception ex) {
                        Console.WriteLine("ERROR: " + ex.Message);
                    }
                }
                List<HtmlNode> spans = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToFind))?.FirstOrDefault().Descendants("span")?.ToList();
                try {
                    string nota = htmlDocument?.GetElementbyId("not_med")?.InnerText?.Replace(',', '.');
                    if (nota != null) {
                        result.nota = double.Parse(nota, NumberStyles.Any, CultureInfo.InvariantCulture);
                    }
                } catch {

                }
                try {
                    string voto = htmlDocument?.GetElementbyId("vot_tot")?.InnerText?.Replace('.', ',');
                    if (voto != null) {
                        result.votos = long.Parse(voto, NumberStyles.Any, CultureInfo.InvariantCulture);
                    }
                } catch {

                }
                try {
                    result.aka = htmlDocument?.DocumentNode?.Descendants("h2")?.FirstOrDefault()?.InnerText;
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                }

                try {
                    result.biography = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToSinopsis))?.FirstOrDefault()?.Descendants("p")?.ToList()?.FirstOrDefault()?.InnerText;
                } catch {

                }
                try {
                    result.imageURL = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, posterClass))?.FirstOrDefault()?.Attributes["src"]?.Value;
                } catch {

                }

            } catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            return result;
        }

        private static FormulaTV_Title ParseTitleHtml(string html) {
            FormulaTV_Title result = null;
            try {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                result = new FormulaTV_Title();
                string classToFind = "topserie";
                List<HtmlNode> ps = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToFind))?.FirstOrDefault().Descendants("p")?.ToList();
                foreach (HtmlNode htmlNode in ps) {
                    try {
                        if (htmlNode.InnerText.Contains("Cadena: ")) {
                            result.cadenas = htmlNode.InnerText.Replace("Cadena: ", "").Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                        if (htmlNode.InnerText.Contains("País: ")) {
                            result.pais = htmlNode.InnerText.Replace("País: ", "");
                        }
                        if (htmlNode.InnerText.Contains("Productora: ")) {
                            result.productora = htmlNode.InnerText.Replace("Productora: ", "");
                        }
                        if (htmlNode.InnerText.Contains("Género: ")) {
                            result.genero = htmlNode.InnerText.Replace("Género: ", "").Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                        if (htmlNode.InnerText.Contains("Primera emisión: ")) {
                            result.primeraEmision = htmlNode.InnerText.Replace("Primera emisión: ", "");
                        }
                        if (htmlNode.InnerText.Contains("Última emisión: ")) {
                            result.ultimaEmision = htmlNode.InnerText.Replace("Última emisión: ", "");
                        }
                        if (htmlNode.InnerText.Contains("Ranking popularidad: ")) {
                            string ranking = htmlNode.InnerText.Replace("Ranking popularidad: ", "").Split(' ').FirstOrDefault().Replace('.', ',');
                            if (ranking != null) {
                                result.ranking_popularidad = int.Parse(ranking, NumberStyles.Any, CultureInfo.InvariantCulture);
                            }
                        }
                        if (htmlNode.InnerText.Contains("Ranking votos: ")) {
                            string ranking_votes = htmlNode.InnerText.Replace("Ranking votos: ", "").Split(' ').FirstOrDefault().Replace('.', ',');
                            if (ranking_votes != null) {
                                result.ranking_votos = int.Parse(ranking_votes, NumberStyles.Any, CultureInfo.InvariantCulture);
                            }
                        }
                    } catch (Exception ex) {
                        Console.WriteLine("ERROR: " + ex.Message);
                    }
                }
                List<HtmlNode> spans = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToFind))?.FirstOrDefault().Descendants("span")?.ToList();
                try {
                    string nota = htmlDocument?.GetElementbyId("not_med")?.InnerText?.Replace(',', '.');
                    if (nota != null) {
                        result.nota = double.Parse(nota, NumberStyles.Any, CultureInfo.InvariantCulture);
                    }
                } catch  {

                }
                try {
                    string voto = htmlDocument?.GetElementbyId("vot_tot")?.InnerText?.Replace('.', ',');
                    if (voto != null) {
                        result.votos = long.Parse(voto, NumberStyles.Any, CultureInfo.InvariantCulture);
                    }
                } catch  {

                }
                try {
                    result.titulo = htmlDocument?.DocumentNode?.Descendants("h2")?.Where(p => p.InnerText.Contains("Título")).FirstOrDefault()?.InnerText?.Replace("Título: ", "");
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                }

                try {
                    result.sinopsis = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToSinopsis))?.FirstOrDefault()?.Descendants("p")?.ToList()?.FirstOrDefault()?.InnerText;
                } catch  {

                }
                try {
                    result.imageURL = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, posterClass))?.FirstOrDefault()?.Attributes["src"]?.Value;
                } catch {

                }

            } catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            return result;
        }

        public static void FormulaTV_GetAudiences(ref FormulaTV_Title title) {
            try {
                const string audiencias = "audiencias";
                string URL_AUDIENCIAS = string.Format("{0}/{1}", title.formulaTV_URL, audiencias);
                // Create web request to obtain HTML.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL_AUDIENCIAS);
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                // With response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    // Response OK.
                    switch (response.StatusCode) {
                        case HttpStatusCode.OK:
                            // Obtain HTML.
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            if (response.CharacterSet == null) {
                                readStream = new StreamReader(receiveStream);
                            } else {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            // With HTML.
                            using (readStream) {

                                HtmlDocument htmlDocument = new HtmlDocument();
                                htmlDocument.LoadHtml(readStream.ReadToEnd());
                                string classToFind = "line reg";


                                List<HtmlNode> htmlAudiencias = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToFind))?.ToList();
                                if (htmlAudiencias != null) {
                                    foreach (HtmlNode htmlNode in htmlAudiencias) {
                                        try {
                                            string linkNode = htmlNode?.Descendants("a")?.FirstOrDefault()?.Attributes["href"]?.Value;
                                            if (linkNode != null) {
                                                for (int i = 0; i < title.episodes.Count; i++) {
                                                    if (title.episodes[i].url == linkNode) {
                                                        title.episodes[i].fecha_de_emision = htmlNode.Descendants("div")?.ToList()[2].InnerText.Replace("\t", "").Replace("\n", "");
                                                        title.episodes[i].audiencia = int.Parse(htmlNode.Descendants("div")?.ToList()[3].InnerText.Replace("\t", "").Replace("\n", "").Replace('.', ','), NumberStyles.Any, CultureInfo.InvariantCulture);
                                                        title.episodes[i].share = double.Parse(htmlNode.Descendants("div")?.ToList()[4].InnerText.Replace("\t", "").Replace("\n", "").Replace(',', '.').Replace("%", ""), NumberStyles.Any, CultureInfo.InvariantCulture);
                                                    }
                                                }
                                            }

                                        } catch  {

                                        }
                                    }
                                }
                                break;
                            }
                        case HttpStatusCode.Moved:
                        default:
                            break;
                    }
                }
            } catch {
                Console.WriteLine("ERROR Getting Episodes for {0}", title.formulaTV_URL);
            }
        }

        public static void FormulaTV_GetSinopsis(ref FormulaTV_Episode episode) {
            try {
                // Create web request to obtain HTML.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(episode.url);
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    // Response OK.
                    switch (response.StatusCode) {
                        case HttpStatusCode.OK:
                            // Obtain HTML.
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            if (response.CharacterSet == null) {
                                readStream = new StreamReader(receiveStream);
                            } else {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            // With HTML.
                            using (readStream) {
                                HtmlDocument htmlDocument = new HtmlDocument();
                                htmlDocument.LoadHtml(readStream.ReadToEnd());
                                string classToFind = "sintxt";
                                List<HtmlNode> sinopsis = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToFind))?.ToList()?.FirstOrDefault()?.Descendants("p")?.ToList();
                                foreach (HtmlNode node in sinopsis) {
                                    episode.sinopsis += node.InnerHtml;
                                }
                            }
                            break;
                    }
                }
            } catch {

            }
        }

        public static void FormulaTV_GetEpisodes(ref FormulaTV_Title title) {
            try {
                const string capitulos = "capitulos";
                string URL_EPISODES = string.Format("{0}/{1}", title.formulaTV_URL, capitulos);
                // Create web request to obtain HTML.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL_EPISODES);
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                // With response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    // Response OK.
                    switch (response.StatusCode) {
                        case HttpStatusCode.OK:
                            // Obtain HTML.
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            if (response.CharacterSet == null) {
                                readStream = new StreamReader(receiveStream);
                            } else {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            // With HTML.
                            using (readStream) {

                                HtmlDocument htmlDocument = new HtmlDocument();
                                htmlDocument.LoadHtml(readStream.ReadToEnd());
                                string classToFind = "line std";
                                title.episodes = new List<FormulaTV_Episode>();
                                List<HtmlNode> htmlEpisodes = htmlDocument?.DocumentNode?.SelectNodes(string.Format(formatContainsSeries, classToFind))?.ToList();
                                if (htmlEpisodes != null) {
                                    htmlEpisodes?.Reverse();
                                    int temporada = 1;
                                    int capitulo = 1;

                                    foreach (HtmlNode htmlNode in htmlEpisodes) {
                                        try {
                                            FormulaTV_Episode episode = new FormulaTV_Episode();

                                            string numberOfEpisode = htmlNode.Descendants("div").ToList()[0]?.InnerText;
                                            string titleOfEpisode = htmlNode.Descendants("div").ToList()[1]?.InnerText;
                                            string notaOfEpisode = htmlNode.Descendants("div").ToList()[2]?.InnerText.Replace(',', '.');
                                            episode.url = htmlNode.Descendants("div").ToList()[1]?.Descendants("a")?.ToList()?.FirstOrDefault()?.Attributes["href"]?.Value;

                                            if (numberOfEpisode != null) {
                                                char[] firstSeparator = new char[] { '(' };
                                                if (numberOfEpisode.ToLower() != "esp") {
                                                    numberOfEpisode = numberOfEpisode.Split(firstSeparator, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                                                    episode.capitulo = int.Parse(numberOfEpisode.Replace('.', ','), NumberStyles.Any, CultureInfo.InvariantCulture);
                                                }
                                            }
                                            if (titleOfEpisode != null) {
                                                episode.titulo = titleOfEpisode.Replace("\t", "").Replace("\n", "");
                                            }

                                            if (notaOfEpisode != null) {
                                                try {
                                                    if (!string.IsNullOrEmpty(notaOfEpisode)) {
                                                        episode.nota = double.Parse(notaOfEpisode.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
                                                    }
                                                } catch { }
                                            }

                                            if (episode.capitulo < capitulo) {
                                                temporada++;
                                                capitulo = 1;
                                            }
                                            episode.temporada = temporada;
                                            FormulaTV_GetSinopsis(ref episode);
                                            title.episodes.Add(episode);
                                            capitulo++;

                                        } catch {

                                        }
                                    }
                                }
                                break;
                            }
                        case HttpStatusCode.Moved:
                        default:
                            break;
                    }
                }
            } catch {
                Console.WriteLine("ERROR Getting Episodes for {0}", title.formulaTV_URL);
            }
        }


        public static FormulaTV_Title FormulaTV_ByTitle(string titleName) {
            FormulaTV_Title result = null;
            try {
                // Create web request to obtain HTML.
                string titleNameUri = RemoveDiacritics(titleName.ToLower().Replace(' ', '-').Replace("!", "").Replace("¡", ""));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", FORMULA_TV_SERIES, titleNameUri));
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                // With response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    // Response OK.
                    switch (response.StatusCode) {
                        case HttpStatusCode.OK:
                            // Obtain HTML.
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            if (response.CharacterSet == null) {
                                readStream = new StreamReader(receiveStream);
                            } else {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            // With HTML.
                            using (readStream) {
                                result = ParseTitleHtml(readStream.ReadToEnd());
                                result.formulaTV_URL = response.ResponseUri.AbsoluteUri;
                                result.formulaTV_Type = "SERIES";
                                break;
                            }
                        case HttpStatusCode.Moved:
                        default:
                            break;
                    }
                }
            } catch  {
                Console.WriteLine("ERROR Getting {0} in series", titleName);
            }

            if (result == null) {
                try {
                    // Create web request to obtain HTML.

                    string titleNameUri = RemoveDiacritics(titleName.ToLower().Replace(' ', '-').Replace("!", "").Replace("¡", ""));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", FORMULA_TV_PROGRAMAS, titleNameUri));
                    request.AllowAutoRedirect = true;
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                    // With response.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                        // Response OK.
                        switch (response.StatusCode) {
                            case HttpStatusCode.OK:
                                // Obtain HTML.
                                Stream receiveStream = response.GetResponseStream();
                                StreamReader readStream = null;
                                if (response.CharacterSet == null) {
                                    readStream = new StreamReader(receiveStream);
                                } else {
                                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                                }
                                // With HTML.
                                using (readStream) {
                                    result = ParseTitleHtml(readStream.ReadToEnd());
                                    result.formulaTV_URL = response.ResponseUri.AbsoluteUri;
                                    result.formulaTV_Type = "PROGRAM";
                                    break;
                                }
                            case HttpStatusCode.Moved:
                            default:
                                break;
                        }
                    }
                } catch{
                    Console.WriteLine("ERROR Getting {0} in programs", titleName);
                }
            }

            return result;
        }

        static string RemoveDiacritics(string text) {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString) {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark) {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }


        public static FormulaTV_Title FormulaTV_ByUrl(string url) {
            FormulaTV_Title result = null;
            try {
                // Create web request to obtain HTML.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                // With response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    // Response OK.
                    switch (response.StatusCode) {
                        case HttpStatusCode.OK:
                            // Obtain HTML.
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            if (response.CharacterSet == null) {
                                readStream = new StreamReader(receiveStream);
                            } else {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            // With HTML.
                            using (readStream) {
                                result = ParseTitleHtml(readStream.ReadToEnd());
                                result.formulaTV_URL = url;
                                if (url.Contains("programas")) {
                                    result.formulaTV_Type = "PROGRAM";
                                }
                                if (url.Contains("series")) {
                                    result.formulaTV_Type = "SERIES";
                                }

                                break;
                            }
                        case HttpStatusCode.Moved:
                        default:
                            break;
                    }
                }
            } catch {
                Console.WriteLine("ERROR Getting {0} by URL", url);
            }
            return result;
        }

        public static void FormulaTV_GetActors(ref FormulaTV_Title title) {
            try {
                // Create web request to obtain HTML.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(title.formulaTV_URL + "/reparto");
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                // With response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    // Response OK.
                    switch (response.StatusCode) {
                        case HttpStatusCode.OK:
                            // Obtain HTML.
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            if (response.CharacterSet == null) {
                                readStream = new StreamReader(receiveStream);
                            } else {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            // With HTML.
                            using (readStream) {
                                string html = readStream.ReadToEnd();
                                HtmlDocument htmlDocument = new HtmlDocument();
                                htmlDocument.LoadHtml(html);
                                List<HtmlNode> actors = htmlDocument.DocumentNode.Descendants().Where(n => n.GetAttributeValue("class", "").Contains("actor")).ToList();
                                title.actorsUrls = new List<string>();
                                foreach (HtmlNode actor in actors) {
                                    string url = actor?.FirstChild?.Attributes["href"]?.Value;
                                    if (url != null) {
                                        title.actorsUrls.Add(url);
                                    }
                                }
                                break;
                            }
                        case HttpStatusCode.Moved:
                        default:
                            break;
                    }
                }
            } catch  {
                Console.WriteLine("ERROR Getting {0} by URL", title.formulaTV_URL);
            }
        }

        public static FormulaTV_Person FormulaTV_PeopleByUrl(string url) {
            FormulaTV_Person result = null;
            try {
                // Create web request to obtain HTML.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                // With response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    // Response OK.
                    switch (response.StatusCode) {
                        case HttpStatusCode.OK:
                            // Obtain HTML.
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            if (response.CharacterSet == null) {
                                readStream = new StreamReader(receiveStream);
                            } else {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            // With HTML.
                            using (readStream) {
                                result = ParsePeopleHtml(readStream.ReadToEnd());
                                break;
                            }
                        case HttpStatusCode.Moved:
                        default:
                            break;
                    }
                }
            } catch {
                Console.WriteLine("ERROR Getting {0} by URL", url);
            }
            return result;
        }

        //static int querys = 1;
        internal static FormulaTV_Title FormulaTV_BySearch_InGoogle(string titleName) {

            FormulaTV_Title result = null;
            try {

                // Create web request to obtain HTML.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(FORMULA_TV_BUCADOR_GOOGLE, HttpUtility.UrlEncode(titleName.ToLower())));
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                //request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 ";

                // With response.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    // Response OK.
                    switch (response.StatusCode) {
                        case HttpStatusCode.OK:
                            // Obtain HTML.
                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = null;
                            if (response.CharacterSet == null) {
                                readStream = new StreamReader(receiveStream);
                            } else {
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            }
                            using (readStream) {
                                HtmlDocument htmlDocument = new HtmlDocument();
                                htmlDocument.LoadHtml(readStream.ReadToEnd());

                                List<HtmlNode> ps = htmlDocument?.DocumentNode?.Descendants("a")?.ToList();
                                // With HTML.
                                for (int i = 0; i < ps.Count; i++) {
                                    string link = ps[i]?.Attributes["href"]?.Value;
                                    if (link != null && (link.Contains(FORMULA_TV_SERIES) || link.Contains(FORMULA_TV_PROGRAMAS))) {
                                        result = FormulaTV_ByUrl(link);
                                        break;
                                    }
                                }
                            }
                            break;
                        case HttpStatusCode.Moved:
                        default:
                            break;
                    }
                }
               
            } catch { 
                Console.WriteLine("ERROR Getting {0}", titleName);
            }
            return result;
        }

        public static FormulaTV_Title FormulaTV_Search(string search) {
            //Three ways of searching
            FormulaTV_Title result = null;
            // - Direct URL
            result = FormulaTV_ByTitle(search);

            /*if (result == null) {
                // - Search in FormulaTV Funciona una Mierda
                result = FormulaTV_BySearch(search);
            }*/
            // - Search in Google by FormulaTV => API Limit?
            if (result == null) {
                //result = FormulaTV_BySearch_InGoogle(search);
            }
            return result;
        }
    }
}

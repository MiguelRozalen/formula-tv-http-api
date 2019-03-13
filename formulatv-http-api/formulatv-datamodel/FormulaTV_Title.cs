using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;

namespace formulatv_datamodel {
    
    [CollectionProperty(Naming = NamingConvention.ToCamelCase, CollectionName = "formulaTV")]
    public class FormulaTV_Title {

        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string _key { get; set; }

        public string titulo;
        public List<string> cadenas;
        public string pais;
        public string productora;
        public List<string> genero;
        public string primeraEmision;
        public string ultimaEmision;
        public int ranking_popularidad = -1;
        public int ranking_votos = -1;
        public double nota = -1;
        public long votos = -1;
        public string sinopsis;
        public string imageURL;
        public string formulaTV_URL;
        public string formulaTV_Type;

        public List<FormulaTV_Episode> episodes;
        public List<string> actorsUrls;
    }
}

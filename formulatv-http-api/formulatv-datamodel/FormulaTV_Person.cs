using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;

namespace formulatv_datamodel {
    [CollectionProperty(Naming = NamingConvention.ToCamelCase, CollectionName = "formulaTV_people")]
    public class FormulaTV_Person {
        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string _key { get; set; }

        public string aka;
        public string name;
        public string surname;
        public string profesion;
        public string origin;
        public string birthday;
        public int age;

        public string imageURL;
        public string formulaTV_URL;

        public int ranking_popularidad = -1;
        public int ranking_votos = -1;
        public double nota = -1;
        public long votos = -1;
        public string biography;
    }
}

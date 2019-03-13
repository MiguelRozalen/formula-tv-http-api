using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formulatv_datamodel {
    public class FormulaTV_Episode {
        public int temporada;
        public int capitulo;
        public string titulo;
        public string url;
        public double nota = -1;
        public string sinopsis;
        public string fecha_de_emision;
        public long audiencia = -1;
        public double share = -1;
    }
}

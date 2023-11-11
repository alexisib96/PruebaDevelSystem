namespace Encuestas.Models
{
    public class Encuesta
    {
        public int IDEncuesta { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Link { get; set; }
        public int IDUser { get; set; }
        public List<CampoEncuesta> Campos { get; set; }

    }
    public class CampoEncuesta
    {
        public int IDEncuesta { get; set; }
        public string Nombre { get; set; }
        public string Titulo { get; set; }
        public bool Requerido { get; set; }
        public int Tipo { get; set; }
        public int IDCampo { get; set; }
        public string TipoText { get; set; }
        public bool EsRequerido { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;


namespace appWeb_07.Models
{
    public class ProductoModel
    {
        [Display(Name = "Código")]
        public int idproducto { get; set; }
        [Display(Name = "Descripción")]
        public string nombreproducto { get; set; }
        [Display(Name = "Categoría")]
        public string nombrecategoria { get; set; }
        [Display(Name = "Precio")]
        public decimal precio { get; set; }
        [Display(Name = "Unidades Disponibles")]
        public Int32 unidades { get; set; }
        [Display(Name = "Foto")]
        public string ruta { get; set; }
    }
}

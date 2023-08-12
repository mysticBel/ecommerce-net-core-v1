using System.ComponentModel.DataAnnotations;


namespace appWeb_07.Models
{
    public class ItemModel
    {
        [Display(Name = "Código")]
        public int idproducto { get; set; }

        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Categoría")]
        public string categoria { get; set; }

        [Display(Name = "Precio")]
        public decimal precio { get; set; }

        [Display(Name = "Unidades Disponibles")]
        public int unidades { get; set; }

        [Display(Name = "Monto")]
        public decimal monto { get { return precio * unidades; } }

        [Display(Name = "Foto")]
        public string ruta { get; set; }
    }
}

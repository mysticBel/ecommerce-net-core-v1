using Microsoft.AspNetCore.Mvc;
using appWeb_07.Models;
using Microsoft.Data.SqlClient;
//
using Newtonsoft.Json;

namespace appWeb_07.Controllers
{
    public class EcommerceController : Controller
    {
        public readonly IConfiguration _configuration;

        public EcommerceController(IConfiguration IConfig)
        {
            _configuration = IConfig;
        }

        // 1. metodo para listar productos
        IEnumerable<ProductoModel> productos()
        {
            List<ProductoModel> products = new List<ProductoModel>();
            using (SqlConnection cn =
                new SqlConnection(_configuration["ConnectionStrings:cn"]))
            {

                SqlCommand cmd = new SqlCommand("exec sp_listar_productos", cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    products.Add(new ProductoModel()
                    {
                        idproducto = dr.GetInt32(0),
                        nombreproducto = dr.GetString(1),
                        nombrecategoria = dr.GetString(2),
                        precio = dr.GetDecimal(3),
                        unidades = dr.GetInt32(4)
                    });
                }
            }

            return products;
        }


        // 2. DOnde mostraremos el Portal de ventas
        public async Task<IActionResult> Portal() 
        {
            //Haremos todo enuna sesion llamada "Canasta"
            //validaci: Si tenemos Canasta, y no hay 
            //seteamos y serializamos y convertimos un objeto a un json
            // de una lista vacia (item)

            if (HttpContext.Session.GetString("Canasta") == null) {
                HttpContext.Session.SetString("Canasta",
                    JsonConvert.SerializeObject(new List<ItemModel>()));
            }
            return View(await Task.Run( () => productos() ));
        }


        // METODO PARA AGREGAR
        ProductoModel Buscar(int id = 0)
        {
            ProductoModel reg =
                productos().Where(p => p.idproducto == id).FirstOrDefault();
            if (reg == null)
            {
                reg = new ProductoModel();
            }
            return reg;
        }

        public async Task<IActionResult> Agregar(int id = 0)
        {
            return View(await Task.Run(() => Buscar(id)));
        }


        [HttpPost]
        public ActionResult Agregar(int codigo, int cantidad)  //recibe 2 parametros de la vista Agregar
        {
            ProductoModel reg = Buscar(codigo);

            if (cantidad > reg.unidades) {
                ViewBag.mensaje = string.Format("El producto solo disponie " +
                    "de {0} unidades", reg.unidades);
                return View(reg);
            }
            //en caso si hay unidades disponibles, debemos guardar el  producto en el carrito
            //guardaremos la info en la sesion "Canasta" , usando ItemModel
            //a traves de una instancia, le damos los datos para que se pueda inicializar
            ItemModel it = new ItemModel();
            it.idproducto = codigo;
            it.descripcion = reg.nombreproducto;  //reg es el Objeto buscado
            it.categoria = reg.nombrecategoria;
            it.precio = reg.precio;
            it.unidades = cantidad;

            //para guardar la info en la sesion necesitamos una lista
            //verifica si hay info en el carrito, y se va añadiendo
            //proceso de deserializacion e inverso
            List<ItemModel> carrito = JsonConvert.DeserializeObject<List<ItemModel>>(HttpContext.Session.GetString("Canasta"));
            carrito.Add(it);
            HttpContext.Session.SetString("Canasta",
                JsonConvert.SerializeObject(carrito));
            ViewBag.mensaje = "Producto Agregado";  //Producto item añadido al carrito


            return View(reg);   
        }

        // MOSTRAR EN UNA VISTA LOS productos que el usuario tiene en su canas
        public ActionResult Canasta()
        {
            if (HttpContext.Session.GetString("Canasta") == null)
            {
                return RedirectToAction("Portal");
            }
            // cao contrario
            IEnumerable<ItemModel> carrito = JsonConvert.DeserializeObject<List<ItemModel>>(HttpContext.Session.GetString("Canasta"));

            return View(carrito); // retornamos la vista con el carrito
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            List<ItemModel> carrito = JsonConvert.DeserializeObject<List<ItemModel>>(HttpContext.Session.GetString("Canasta"));
            
            ItemModel item = carrito.Where(it => it.idproducto == id).First();
            carrito.Remove(item);

            HttpContext.Session.SetString("Canasta", JsonConvert.SerializeObject(carrito));

            return RedirectToAction("Canasta");
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}

using FORTTHEGAMES.Data;
using FORTTHEGAMES.Data.Cart;
using FORTTHEGAMES.Data.ViewModels;
using FORTTHEGAMES.Models;
using Khipu.Api;
using Khipu.Client;
using Khipu.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FORTTHEGAMES.Controllers
{
    public class PasarelaController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly FORTTHEGAMESContext _context;
        public IConfiguration _configuration;


        public PasarelaController(FORTTHEGAMESContext context, IConfiguration configuration, ShoppingCart shoppingCart)
        {
            _context = context;
            _configuration = configuration;
            _shoppingCart = shoppingCart;
        }
        public ActionResult Pay(string token)
        {

            try
            {
                ViewModelPago viewModelPago = new ViewModelPago();

                //se valida token
                if (ValidateToken(token) != null)
                {

                }
                else
                {
                    return View("cancel");
                }

                //Se lee Token
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                string solicitud_jwt = jwt.Claims.First(c => c.Type == "IdUnique").Value;
                string monto_jwt = jwt.Claims.First(c => c.Type == "Monto").Value;
                string subject_jwt = jwt.Claims.First(c => c.Type == "Subject").Value;
                string body_jwt = jwt.Claims.First(c => c.Type == "Body").Value;
                string shopid = jwt.Claims.First(c => c.Type == "ShoppId").Value; 





                //Se formatea el monto
                //string montoAux = string.Format("{0:0.00}", Convert.ToDecimal(monto));

                string montoAux = monto_jwt.ToString();

                CultureInfo chile = CultureInfo.CreateSpecificCulture("es-CL");
                //double montoFinal = Double.Parse(montoAux, System.Globalization.NumberFormatInfo.InvariantInfo);
                double montoFinal = Double.Parse(montoAux);

                viewModelPago.monto = montoFinal;
                viewModelPago.cliente = "";

                viewModelPago.transaction_id = "";
                viewModelPago.num_solicitud = solicitud_jwt.ToString();
                viewModelPago.fecha = DateTime.Now;
                viewModelPago.subject = subject_jwt;
                viewModelPago.body = body_jwt;
                viewModelPago.token = token;




                return View(viewModelPago);
            }
            catch
            {

                return View("cancel");
            }
        }


        public JsonResult CrearPago(string token, int opcionPago)
        {
            //Se lee el token
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            string solicitud = jwt.Claims.First(c => c.Type == "IdUnique").Value;
            string monto = jwt.Claims.First(c => c.Type == "Monto").Value;
            string subject = jwt.Claims.First(c => c.Type == "Subject").Value;
            string body = jwt.Claims.First(c => c.Type == "Body").Value;
            string shopid = jwt.Claims.First(c => c.Type == "ShoppId").Value;



            //Verificacion si el pago fue creado
            var parseSolicitud = int.Parse(solicitud);
            var pagofind = _context.Payment.FirstOrDefault(p => p.id_unique_service == parseSolicitud);

            if (pagofind != null)
            {
                var pago_detalle = _context.Payment_Detail.FirstOrDefault(p => p.id_payment == pagofind.id);

                string urlPayKiphu = pago_detalle.payment_url;


                return new JsonResult(Ok(new { result = true, urlPago = urlPayKiphu }));

            }
            else
            {
                //creacion del pago
                Payment pago = new Payment();
                pago.amount = Convert.ToDecimal(monto);
                pago.transaction_id = solicitud;
                pago.id_unique_service = int.Parse(solicitud);

                //se dejan productos en 0
                var shopcar = _context.ShoppingCartItems.Where(s => s.ShoppingCartId == shopid).Include(p=>p.Producto);
                string idshopcar = "";
                foreach (var item in shopcar)
                {
                    //Producto producto = item.Producto;
                    //producto.estado = 0;
                    //_context.Producto.Update(producto);
                    idshopcar = item.ShoppingCartId;
                }


                _context.Payment.Add(pago);
                _context.SaveChanges();
                int pago_id = pago.id;

                //se reemplazan puntos
                string montoStr = monto.Replace(".", string.Empty);



                CultureInfo chile = CultureInfo.CreateSpecificCulture("es-CL");
                double montoEnviar = Double.Parse(montoStr, System.Globalization.NumberFormatInfo.InvariantInfo);

                Configuration.ReceiverId = 446068;
                Configuration.Secret = "e33bd04ccee8f3f8383cf65c9c3a45da42580d35";
                PaymentsApi a = new PaymentsApi();


                try
                {


                    DateTime dt = DateTime.Now;
                    dt = dt.AddDays(5);
                    PaymentsCreateResponse response = a.PaymentsPost(
                        subject,//subject
                        "CLP",//currency
                        montoEnviar,//monto
                        transactionId: solicitud,
                        expiresDate: dt,
                        body: body,
                        pictureUrl: "https://hips.hearstapps.com/hmg-prod/images/gh-index-gamingconsoles-052-print-preview-1659705142.jpg",
                        returnUrl: "https://localhost:7133/Pasarela/success/"+ parseSolicitud,
                        cancelUrl: "https://localhost:7133/Pasarela/cancel",
                        notifyUrl: "https://localhost:7133/api/notifypago/1",
                        notifyApiVersion: "1.3"
                     );
                    System.Console.WriteLine(response);

                    //Detalle del pago
                    Payment_Detail pago_Detalle = new Payment_Detail();
                    pago_Detalle.id_payment = pago_id;
                    pago_Detalle.payment_id = response.PaymentId;
                    pago_Detalle.payment_url = response.PaymentUrl;


                    _context.Payment_Detail.Add(pago_Detalle);
                    _context.SaveChanges();

                    string urlPayKiphu = response.PaymentUrl;


                    return new JsonResult(Ok(new { result = true, urlPago = urlPayKiphu }));
                }
                catch (ApiException e)
                {
                    Console.WriteLine(e);
                }
            }



            return new JsonResult(Ok());
        }

        public ActionResult cancel(string hola)
        {
            try
            {
                return View();
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> success(int id)
        {
            List<ViewModelSuccess> ViewModelSuccess = new List<ViewModelSuccess>();
            try
            {

                //var shopcar = _context.ShoppingCartItems.Where(s => s.ShoppingCartId == id).Include(p => p.Producto);
                var items = _shoppingCart.GetShoppingCartItems();
                foreach (var item in items)
                {
                    var orderItem = new OrderItem()
                    {
                        cantidad = item.monto,
                        Producto = item.Producto,
                        id_order = id,
                        price = item.Producto.valor
                    };
                    await _context.OrderItem.AddAsync(orderItem);
                }


                foreach (var item in items)
                {
                    Producto producto = item.Producto;
                    producto.estado = 0;
                    _context.Producto.Update(producto);
                    ViewModelSuccess vms = new ViewModelSuccess();
                    vms.imagen = item.Producto.imagen;
                    vms.valor = item.Producto.valor;
                    vms.nombre = item.Producto.nombre;
                    ViewModelSuccess.Add(vms);
                }
                await _context.SaveChangesAsync();

                await _shoppingCart.ClearShoppingCartAsync();



                return View(ViewModelSuccess);
            }
            catch
            {
                return View();
            }
        }

        public int? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var idUnique = int.Parse(jwtToken.Claims.First(x => x.Type == "IdUnique").Value);

                // return user id from JWT token if validation successful
                return idUnique;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }


        public async Task<IActionResult> PayKhipu(string id)
        {
            //Obtener usuario
            var correo = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var usuario = _context.Usuario.FirstOrDefault(u => u.correo == correo);
            //var item = await _moviesService.GetMovieByIdAsync(id);
            var carro = _context.ShoppingCartItems.Where(c => c.ShoppingCartId == id).Include(p => p.Producto);

            if (carro != null)
            {
                double valorTotal = 0;
                foreach (var item in carro)
                {
                    valorTotal = valorTotal + item.Producto.valor;
                }


                Order order = new Order();
                order.Usuario = usuario;
                order.id_shoppingitem = id;
                order.total = valorTotal;
                order.fecha = DateTime.Now;


                _context.Order.Add(order);
                _context.SaveChanges();


                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                        new Claim("IdUnique", order.id.ToString()),
                        new Claim("Monto", valorTotal.ToString()),
                        new Claim("Body", "Compra de Productos"),
                        new Claim("Subject", "For The Games"),
                        new Claim("ShoppId", id)
                        //new Claim("Cliente", tokenPayInfo.cliente),
                        //new Claim("CuentaCobro", tokenPayInfo.cuenta_cobro),
                        //new Claim("IdTransaction", tokenPayInfo.transaction_id)
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: signIn);

                var token_access = new JwtSecurityTokenHandler().WriteToken(token);

                return RedirectToAction("Pay", new
                {
                    token = token_access
                });




            }

            return RedirectToAction(nameof(Index));
        }
    }
}

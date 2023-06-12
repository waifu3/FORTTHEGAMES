using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FORTTHEGAMES.Data;
using FORTTHEGAMES.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.SqlClient;
using FORTTHEGAMES.Data.ViewModels;

namespace FORTTHEGAMES.Controllers
{
    public class AccesoController : Controller
    {
        private readonly FORTTHEGAMESContext _context;

        public AccesoController(FORTTHEGAMESContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Login()
        {
            ClaimsPrincipal c = HttpContext.User;
            if (c.Identity != null)
            {
                if (c.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
            }
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(Usuario u)
        {
            try
            {
                var user = _context.Usuario.FirstOrDefault(p => p.correo == u.correo && p.password == u.password);

                if (user != null)
                {
                    string rol = "";
                    if (user.rol == 1)
                    {
                        rol = "Admin";
                    }
                    else
                    {
                        rol = "User";
                    }
                    List<Claim> c = new List<Claim>()
                                {
                                    new Claim(ClaimTypes.NameIdentifier, u.correo),
                                    new Claim(ClaimTypes.Role, rol),
                                    new Claim(ClaimTypes.Name, user.nombre)
                                };
                    
                    ClaimsIdentity ci = new(c, CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties p = new();

                    p.AllowRefresh = true;
                    p.IsPersistent = u.MantenerActivo;


                    if (!u.MantenerActivo)
                        p.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60);
                    else
                        p.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(ci), p);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Credenciales incorrectas o cuenta no registrada.";
                }
                return View();
            }
            catch (System.Exception e)
            {
                ViewBag.Error = e.Message;
                return View();
            }
        }




		public async Task<IActionResult> Registrarse()
		{
			
			return View();
		}

        [HttpPost]
        public async Task<IActionResult> Registrarse(Usuario u)
        {
            var mensaje = ValidarUsuario(u.password, u.passwordRepeat, u.correo);
            if (mensaje == "")
            {
                u.rol = 0;
                _context.Usuario.Add(u);
                mensaje = "Usuario creado correctamente";
                _context.SaveChanges();
                ViewBag.Message = mensaje;
            }
            else
            {
                ViewBag.Message = mensaje;
            }
            
            return View();
        }
         
		public async Task<IActionResult> SignUp()
		{

            return RedirectToAction("Registrarse", "Acceso");
		}

        public async Task<IActionResult> RecuperarPassword()
        {
            var response = new ViewModelLogin();


			return View(response);
        }

        [HttpPost]
		public async Task<IActionResult> RecuperarPassword(ViewModelLogin VML)
		{
            var usuario = _context.Usuario.FirstOrDefault(u => u.correo == VML.correo);
            if (usuario != null)
            {
                usuario.password = VML.password;
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Error = "No existe el correo ingresado";
            }


			return View();
		}

		private string ValidarUsuario(string password1, string password2, string correo)
        {
            string mensaje = "";
            if (password1.Equals(password2))
            {
                //Son iguales
            }
            else
            {
                //Password distintas
                mensaje = "Password Distintas";
                return mensaje;
            }

            var correoexiste = _context.Usuario.FirstOrDefault(u => u.correo == correo);
            if (correoexiste != null)
            {
                mensaje = "Correo existente";
                return mensaje;
            }
            else
            {
                return mensaje;
            }
        }


	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FORTTHEGAMES.Data;
using FORTTHEGAMES.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FORTTHEGAMES.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly FORTTHEGAMESContext _context;

        public ProductoController(FORTTHEGAMESContext context)
        {
            _context = context;
        }

        // GET: Producto
        public async Task<IActionResult> Index()
        {
            List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();
            string rol = "";
            foreach (var item in roleClaims)
            {
                rol = item.Value;
            }

            if (rol == "Admin")
            {
                return _context.Producto != null ?
                          View(await _context.Producto.ToListAsync()) :
                          Problem("Entity set 'FORTTHEGAMESContext.Producto'  is null.");
            }
            else
            {
                return NotFound();
            }

            
        }

        // GET: Producto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Producto == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .FirstOrDefaultAsync(m => m.id_producto == id);
            if (producto == null)
            {
                return NotFound();
            }

            List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();
            string rol = "";
            foreach (var item in roleClaims)
            {
                rol = item.Value;
            }

            if (rol != "Admin")
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Producto/Create
        public IActionResult Create()
        {
            List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();
            string rol = "";
            foreach (var item in roleClaims)
            {
                rol = item.Value;
            }

            if (rol != "Admin")
            {
                return NotFound();
            }

            return View();
        }

        // POST: Producto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id_producto,nombre,imagen,descripcion,valor,estado,id_categoria")] Producto producto, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;  //to contain the filename
                if (file != null)  //handle iformfile
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);

                    using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileSrteam);
                    }
                }
                producto.imagen = file.FileName; //fill the image property

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Producto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Producto == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();
            string rol = "";
            foreach (var item in roleClaims)
            {
                rol = item.Value;
            }

            if (rol != "Admin")
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Producto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id_producto,nombre,descripcion,imagen,valor,estado,id_categoria")] Producto producto)
        {
            if (id != producto.id_producto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.id_producto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Producto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Producto == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .FirstOrDefaultAsync(m => m.id_producto == id);
            if (producto == null)
            {
                return NotFound();
            }

            List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();
            string rol = "";
            foreach (var item in roleClaims)
            {
                rol = item.Value;
            }

            if (rol != "Admin")
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Producto == null)
            {
                return Problem("Entity set 'FORTTHEGAMESContext.Producto'  is null.");
            }
            var producto = await _context.Producto.FindAsync(id);
            if (producto != null)
            {
                producto.estado = 0;
                _context.Producto.Update(producto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
          return (_context.Producto?.Any(e => e.id_producto == id)).GetValueOrDefault();
        }
    }
}

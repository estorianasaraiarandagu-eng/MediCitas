using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using MediCitas.Models;
using System.Collections.Generic;

namespace MediCitas.Controllers
{
    public class MedicoController : Controller
    {
        private readonly string _connectionString;

        public MedicoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            var medicos = new List<Medico>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT id_medico, nombre, apellido, especialidad, telefono FROM medico";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            medicos.Add(new Medico
                            {
                                IdMedico = reader.GetInt32("id_medico"),
                                Nombre = reader.GetString("nombre"),
                                Apellido = reader.GetString("apellido"),
                                Especialidad = reader.GetString("especialidad"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? "" : reader.GetString("telefono")
                            });
                        }
                    }
                }
            }
            return View(medicos);
        }

        // GET: Medico/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medico/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Medico medico)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO medico (nombre, apellido, especialidad, telefono) VALUES (@nombre, @apellido, @especialidad, @telefono)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", medico.Nombre);
                        command.Parameters.AddWithValue("@apellido", medico.Apellido);
                        command.Parameters.AddWithValue("@especialidad", medico.Especialidad);
                        command.Parameters.AddWithValue("@telefono", medico.Telefono ?? (object)DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(medico);
        }
    }
}
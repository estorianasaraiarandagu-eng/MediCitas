using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using MediCitas.Models;
using System.Collections.Generic;

namespace MediCitas.Controllers
{
    public class PacienteController : Controller
    {
        private readonly string _connectionString;

        public PacienteController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        // Pantalla 1: Listado de Pacientes
        public IActionResult Index()
        {
            List<Paciente> lista = new List<Paciente>();
            using (var conexion = new MySqlConnection(_connectionString))
            {
                conexion.Open();
                string query = "SELECT id_paciente, nombre, telefono FROM paciente";
                using (var cmd = new MySqlCommand(query, conexion))
                {
                    using (var lector = cmd.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Paciente {
                                IdPaciente = lector.GetInt32("id_paciente"),
                                Nombre = lector.GetString("nombre"),
                                Telefono = lector.GetString("telefono")
                            });
                        }
                    }
                }
            }
            return View(lista);
        }

        // Pantalla 2: Vista del Formulario de Registro (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Procesar Registro (POST)
        [HttpPost]
        public IActionResult Create(string nombre, string telefono)
        {
            using (var conexion = new MySqlConnection(_connectionString))
            {
                conexion.Open();
                string query = "INSERT INTO paciente (nombre, telefono) VALUES (@nombre, @telefono)";
                using (var cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@telefono", telefono);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // Pantalla 3: Ver Detalles del Paciente
        public IActionResult Details(int id)
        {
            Paciente paciente = null;
            using (var conexion = new MySqlConnection(_connectionString))
            {
                conexion.Open();
                string query = "SELECT id_paciente, nombre, telefono FROM paciente WHERE id_paciente = @id";
                using (var cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var lector = cmd.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            paciente = new Paciente {
                                IdPaciente = lector.GetInt32("id_paciente"),
                                Nombre = lector.GetString("nombre"),
                                Telefono = lector.GetString("telefono")
                            };
                        }
                    }
                }
            }
            if (paciente == null) return NotFound();
            return View(paciente);
        }

        // Pantalla 4: Formulario de Edición (GET)
        public IActionResult Edit(int id)
        {
            Paciente paciente = null;
            using (var conexion = new MySqlConnection(_connectionString))
            {
                conexion.Open();
                string query = "SELECT id_paciente, nombre, telefono FROM paciente WHERE id_paciente = @id";
                using (var cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var lector = cmd.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            paciente = new Paciente {
                                IdPaciente = lector.GetInt32("id_paciente"),
                                Nombre = lector.GetString("nombre"),
                                Telefono = lector.GetString("telefono")
                            };
                        }
                    }
                }
            }
            if (paciente == null) return NotFound();
            return View(paciente);
        }

        // Procesar Edición (POST)
        [HttpPost]
        public IActionResult Edit(int id, string nombre, string telefono)
        {
            using (var conexion = new MySqlConnection(_connectionString))
            {
                conexion.Open();
                string query = "UPDATE paciente SET nombre = @nombre, telefono = @telefono WHERE id_paciente = @id";
                using (var cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@telefono", telefono);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // Pantalla 5: Confirmación de Eliminación (GET)
        public IActionResult Delete(int id)
        {
            Paciente paciente = null;
            using (var conexion = new MySqlConnection(_connectionString))
            {
                conexion.Open();
                string query = "SELECT id_paciente, nombre, telefono FROM paciente WHERE id_paciente = @id";
                using (var cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var lector = cmd.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            paciente = new Paciente {
                                IdPaciente = lector.GetInt32("id_paciente"),
                                Nombre = lector.GetString("nombre"),
                                Telefono = lector.GetString("telefono")
                            };
                        }
                    }
                }
            }
            if (paciente == null) return NotFound();
            return View(paciente);
        }

        // Procesar Eliminación (POST)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (var conexion = new MySqlConnection(_connectionString))
            {
                conexion.Open();
                string query = "DELETE FROM paciente WHERE id_paciente = @id";
                using (var cmd = new MySqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
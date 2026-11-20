// ==========================================
// ARCHIVO: Controllers/CitaController.cs
// ==========================================
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using MediCitas.Models;
using System.Data;

namespace MediCitas.Controllers
{
    public class CitaController : Controller
    {
        // Conexión configurada directamente con tu base de datos en Aiven
        private readonly string _connectionString = "Server=mysql-208cce03-unifranz-c0b4.g.aivencloud.com;Port=20872;Database=defaultdb;Uid=avnadmin;Pwd=AQUI_TU_CONTRASENA;SslMode=Required;";

        public IActionResult IndexAgendadas()
        {
            var lista = new List<Cita>();
            try
            {
                using (var conexion = new MySqlConnection(_connectionString))
                {
                    conexion.Open();
                    string query = @"SELECT c.id_cita, c.id_paciente, c.id_medico, c.fecha_hora, c.motivo, c.estado,
                                     CONCAT(p.nombre, ' ', p.apellido) AS nombre_paciente,
                                     CONCAT(m.nombre, ' ', m.apellido, ' (', m.especialidad, ')') AS nombre_medico
                                     FROM cita c
                                     INNER JOIN paciente p ON c.id_paciente = p.id_paciente
                                     INNER JOIN medico m ON c.id_medico = m.id_medico
                                     WHERE c.estado = 'Agendada'
                                     ORDER BY c.fecha_hora ASC";

                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Cita
                                {
                                    IdCita = reader.GetInt32("id_cita"),
                                    IdPaciente = reader.GetInt32("id_paciente"),
                                    IdMedico = reader.GetInt32("id_medico"),
                                    FechaHora = reader.GetDateTime("fecha_hora"),
                                    Motivo = reader.GetString("motivo"),
                                    Estado = reader.GetString("estado"),
                                    NombrePaciente = reader.GetString("nombre_paciente"),
                                    NombreMedico = reader.GetString("nombre_medico")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
            return View(lista);
        }

        public IActionResult IndexCanceladas()
        {
            var lista = new List<Cita>();
            try
            {
                using (var conexion = new MySqlConnection(_connectionString))
                {
                    conexion.Open();
                    string query = @"SELECT c.id_cita, c.id_paciente, c.id_medico, c.fecha_hora, c.motivo, c.estado,
                                     CONCAT(p.nombre, ' ', p.apellido) AS nombre_paciente,
                                     CONCAT(m.nombre, ' ', m.apellido, ' (', m.especialidad, ')') AS nombre_medico
                                     FROM cita c
                                     INNER JOIN paciente p ON c.id_paciente = p.id_paciente
                                     INNER JOIN medico m ON c.id_medico = m.id_medico
                                     WHERE c.estado = 'Cancelada'
                                     ORDER BY c.fecha_hora DESC";

                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Cita
                                {
                                    IdCita = reader.GetInt32("id_cita"),
                                    IdPaciente = reader.GetInt32("id_paciente"),
                                    IdMedico = reader.GetInt32("id_medico"),
                                    FechaHora = reader.GetDateTime("fecha_hora"),
                                    Motivo = reader.GetString("motivo"),
                                    Estado = reader.GetString("estado"),
                                    NombrePaciente = reader.GetString("nombre_paciente"),
                                    NombreMedico = reader.GetString("nombre_medico")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
            return View(lista);
        }

        public IActionResult Create()
        {
            CargarListasDesplegables();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cita cita)
        {
            try
            {
                using (var conexion = new MySqlConnection(_connectionString))
                {
                    conexion.Open();
                    string query = "INSERT INTO cita (id_paciente, id_medico, fecha_hora, motivo, estado) VALUES (@idPaciente, @idMedico, @fechaHora, @motivo, 'Agendada')";
                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@idPaciente", cita.IdPaciente);
                        cmd.Parameters.AddWithValue("@idMedico", cita.IdMedico);
                        cmd.Parameters.AddWithValue("@fechaHora", cita.FechaHora);
                        cmd.Parameters.AddWithValue("@motivo", cita.Motivo ?? string.Empty);
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(IndexAgendadas));
            }
            catch (Exception)
            {
                CargarListasDesplegables();
                return View(cita);
            }
        }

        public IActionResult Cancelar(int id)
        {
            try
            {
                using (var conexion = new MySqlConnection(_connectionString))
                {
                    conexion.Open();
                    string query = "UPDATE cita SET estado = 'Cancelada' WHERE id_cita = @id";
                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception) { }
            return RedirectToAction(nameof(IndexAgendadas));
        }

        public IActionResult AgendaDiaria(string? fecha, int? idMedico)
        {
            var lista = new List<Cita>();
            DateTime fechaFiltro = string.IsNullOrEmpty(fecha) ? DateTime.Today : DateTime.Parse(fecha);

            ViewBag.FechaSeleccionada = fechaFiltro.ToString("yyyy-MM-dd");
            ViewBag.MedicoSeleccionado = idMedico;
            CargarListasDesplegables();

            try
            {
                using (var conexion = new MySqlConnection(_connectionString))
                {
                    conexion.Open();
                    string query = @"SELECT c.id_cita, c.id_paciente, c.id_medico, c.fecha_hora, c.motivo, c.estado,
                                     CONCAT(p.nombre, ' ', p.apellido) AS nombre_paciente,
                                     CONCAT(m.nombre, ' ', m.apellido, ' (', m.especialidad, ')') AS nombre_medico
                                     FROM cita c
                                     INNER JOIN paciente p ON c.id_paciente = p.id_paciente
                                     INNER JOIN medico m ON c.id_medico = m.id_medico
                                     WHERE DATE(c.fecha_hora) = @fecha";

                    if (idMedico.HasValue && idMedico.Value > 0)
                    {
                        query += " AND c.id_medico = @idMedico";
                    }

                    query += " ORDER BY c.fecha_hora ASC";

                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@fecha", fechaFiltro.ToString("yyyy-MM-dd"));
                        if (idMedico.HasValue && idMedico.Value > 0)
                        {
                            cmd.Parameters.AddWithValue("@idMedico", idMedico.Value);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Cita
                                {
                                    IdCita = reader.GetInt32("id_cita"),
                                    IdPaciente = reader.GetInt32("id_paciente"),
                                    IdMedico = reader.GetInt32("id_medico"),
                                    FechaHora = reader.GetDateTime("fecha_hora"),
                                    Motivo = reader.GetString("motivo"),
                                    Estado = reader.GetString("estado"),
                                    NombrePaciente = reader.GetString("nombre_paciente"),
                                    NombreMedico = reader.GetString("nombre_medico")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            return View(lista);
        }

        public IActionResult HistorialClinico(int? idPaciente)
        {
            var lista = new List<ConsultaMedica>();
            CargarListasDesplegables();
            ViewBag.PacienteSeleccionado = idPaciente;

            if (!idPaciente.HasValue || idPaciente.Value == 0)
            {
                return View(lista);
            }

            try
            {
                using (var conexion = new MySqlConnection(_connectionString))
                {
                    conexion.Open();
                    string query = @"SELECT cm.id_consulta, cm.id_cita, cm.diagnostico, cm.receta, cm.observaciones,
                                     c.fecha_hora, CONCAT(m.nombre, ' ', m.apellido, ' (', m.especialidad, ')') AS nombre_medico
                                     FROM consulta_medica cm
                                     INNER JOIN cita c ON cm.id_cita = c.id_cita
                                     INNER JOIN medico m ON c.id_medico = m.id_medico
                                     WHERE c.id_paciente = @idPaciente
                                     ORDER BY c.fecha_hora DESC";

                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@idPaciente", idPaciente.Value);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new ConsultaMedica
                                {
                                    IdConsulta = reader.GetInt32("id_consulta"),
                                    IdCita = reader.GetInt32("id_cita"),
                                    Diagnostico = reader.GetString("diagnostico"),
                                    Receta = reader.GetString("receta"),
                                    Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? string.Empty : reader.GetString("observaciones"),
                                    FechaCita = reader.GetDateTime("fecha_hora"),
                                    NombreMedico = reader.GetString("nombre_medico")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            return View(lista);
        }

        public IActionResult Especialidades()
        {
            var lista = new List<EspecialidadResumen>();
            try
            {
                using (var conexion = new MySqlConnection(_connectionString))
                {
                    conexion.Open();
                    string query = @"SELECT especialidad, COUNT(*) AS cantidad, 
                                     GROUP_CONCAT(CONCAT(nombre, ' ', apellido) SEPARATOR ', ') AS doctores
                                     FROM medico
                                     GROUP BY especialidad";

                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new EspecialidadResumen
                                {
                                    Especialidad = reader.GetString("especialidad"),
                                    CantidadDoctores = reader.GetInt32("cantidad"),
                                    DoctoresNombres = reader.GetString("doctores")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            return View(lista);
        }

        public IActionResult Atender(int id)
        {
            ConsultaMedica modelo = new ConsultaMedica();
            try
            {
                using (var conexion = new MySqlConnection(_connectionString))
                {
                    conexion.Open();
                    string query = @"SELECT c.id_cita, c.fecha_hora, 
                                     CONCAT(p.nombre, ' ', p.apellido) AS nombre_paciente,
                                     CONCAT(m.nombre, ' ', m.apellido, ' (', m.especialidad, ')') AS nombre_medico
                                     FROM cita c
                                     INNER JOIN paciente p ON c.id_paciente = p.id_paciente
                                     INNER JOIN medico m ON c.id_medico = m.id_medico
                                     WHERE c.id_cita = @id";

                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                modelo.IdCita = reader.GetInt32("id_cita");
                                modelo.FechaCita = reader.GetDateTime("fecha_hora");
                                modelo.NombrePaciente = reader.GetString("nombre_paciente");
                                modelo.NombreMedico = reader.GetString("nombre_medico");
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Atender(ConsultaMedica modelo)
        {
            try
            {
                using (var conexion = new MySqlConnection(_connectionString))
                {
                    conexion.Open();
                    string queryConsulta = "INSERT INTO consulta_medica (id_cita, diagnostico, receta, observaciones) VALUES (@idCita, @diagnostico, @receta, @observaciones)";
                    using (var cmd = new MySqlCommand(queryConsulta, conexion))
                    {
                        cmd.Parameters.AddWithValue("@idCita", modelo.IdCita);
                        cmd.Parameters.AddWithValue("@diagnostico", modelo.Diagnostico);
                        cmd.Parameters.AddWithValue("@receta", modelo.Receta);
                        cmd.Parameters.AddWithValue("@observaciones", modelo.Observaciones ?? string.Empty);
                        cmd.ExecuteNonQuery();
                    }

                    string queryCita = "UPDATE cita SET estado = 'Atendida' WHERE id_cita = @idCita";
                    using (var cmd2 = new MySqlCommand(queryCita, conexion))
                    {
                        cmd2.Parameters.AddWithValue("@idCita", modelo.IdCita);
                        cmd2.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(AgendaDiaria));
            }
            catch (Exception)
            {
                return View(modelo);
            }
        }

        private void CargarListasDesplegables()
        {
            var pacientes = new List<Paciente>();
            var medicos = new List<Medico>();

            try
            {
                using (var conexion = new MySqlConnection(_connectionString))
                {
                    conexion.Open();
                    using (var cmd = new MySqlCommand("SELECT id_paciente, CONCAT(nombre, ' ', apellido) AS nombre FROM paciente", conexion))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pacientes.Add(new Paciente { IdPaciente = reader.GetInt32("id_paciente"), Nombre = reader.GetString("nombre") });
                            }
                        }
                    }

                    using (var cmd2 = new MySqlCommand("SELECT id_medico, CONCAT(nombre, ' ', apellido, ' - ', especialidad) AS nombre FROM medico", conexion))
                    {
                        using (var reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                medicos.Add(new Medico { IdMedico = reader.GetInt32("id_medico"), Nombre = reader.GetString("nombre") });
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            ViewBag.Pacientes = pacientes;
            ViewBag.Medicos = medicos;
        }
    }
}
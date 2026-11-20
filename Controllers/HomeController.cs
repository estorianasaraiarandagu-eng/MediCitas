using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace MediCitas.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Estadisticas()
        {
            int totalPacientes = 0;
            int totalMedicos = 0;
            int citasAgendadas = 0;
            int citasCanceladas = 0;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM paciente", connection))
                {
                    totalPacientes = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM medico", connection))
                {
                    totalMedicos = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM cita WHERE estado = 'Agendada'", connection))
                {
                    citasAgendadas = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM cita WHERE estado = 'Cancelada'", connection))
                {
                    citasCanceladas = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            ViewBag.TotalPacientes = totalPacientes;
            ViewBag.TotalMedicos = totalMedicos;
            ViewBag.CitasAgendadas = citasAgendadas;
            ViewBag.CitasCanceladas = citasCanceladas;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
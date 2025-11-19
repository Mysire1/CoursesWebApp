using Microsoft.AspNetCore.Mvc;
using CoursesWebApp.Data;
using Npgsql;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace CoursesWebApp.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class DatabaseConsoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public DatabaseConsoleController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteQuery([FromBody] QueryRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Query))
                {
                    return Json(new 
                    { 
                        success = false, 
                        error = "Запит не може бути порожнім" 
                    });
                }
                
                var queryUpper = request.Query.Trim().ToUpperInvariant();
                if (!queryUpper.StartsWith("SELECT"))
                {
                    return Json(new 
                    { 
                        success = false, 
                        error = "Дозволені тільки SELECT запити. Операції зміни даних (INSERT, UPDATE, DELETE, DROP, ALTER, CREATE, TRUNCATE) заборонені в консолі." 
                    });
                }
                
                var dangerousKeywords = new[] { "DROP", "DELETE", "TRUNCATE", "ALTER", "CREATE", "INSERT", "UPDATE" };
                foreach (var keyword in dangerousKeywords)
                {
                    if (queryUpper.Contains(keyword))
                    {
                        return Json(new 
                        { 
                            success = false, 
                            error = $"Операція {keyword} заборонена в консолі. Дозволені тільки SELECT запити." 
                        });
                    }
                }

                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var results = new List<Dictionary<string, object>>();

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(request.Query, connection))
                    {
                        command.CommandTimeout = 30;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    row[reader.GetName(i)] = value;
                                }
                                results.Add(row);
                            }
                        }
                    }
                }

                return Json(new 
                { 
                    success = true, 
                    data = results, 
                    rowCount = results.Count 
                });
            }
            catch (PostgresException ex)
            {
                return Json(new 
                { 
                    success = false, 
                    error = $"Помилка PostgreSQL: {ex.MessageText}\nКод помилки: {ex.SqlState}" 
                });
            }
            catch (Exception ex)
            {
                return Json(new 
                { 
                    success = false, 
                    error = $"Помилка виконання запиту: {ex.Message}" 
                });
            }
        }
    }

    public class QueryRequest
    {
        public string Query { get; set; } = string.Empty;
    }
}

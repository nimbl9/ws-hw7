using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniCoursesApp.Attributes;
using UniCoursesApp.Models;
using UniCoursesApp.Services;

namespace UniCoursesApp.Controllers;

[ApiController]
[Route("api/courses")]
[BasicAuthentication]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    [MyAuthorize("Admin", "Teacher", "Student")]
    public ActionResult<IEnumerable<Course>> GetAllCourses()
    {
        Console.WriteLine("запрос на просмотр всех учебных курсов");
        var list = _courseService.FetchCourses();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [MyAuthorize("Admin", "Teacher", "Student")]
    public ActionResult<Course> GetCourse(int id)
    {
        Console.WriteLine($"запрос на получение данных курса {id}");
        var course = _courseService.FetchById(id);
        if (course == null)
        {
            Console.WriteLine($"курс {id} не найден");
            return NotFound(new { message = "учебный курс не найден" });
        }

        return Ok(course);
    }

    [HttpPost]
    [MyAuthorize("Admin", "Teacher")]
    public ActionResult<Course> CreateCourse([FromBody] Course course)
    {
        Console.WriteLine($"создание нового учебного курса: {course.Title}");
        var created = _courseService.AddCourse(course);
        return CreatedAtAction(nameof(GetCourse), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [MyAuthorize("Admin", "Teacher")]
    public IActionResult UpdateCourse(int id, [FromBody] Course course)
    {
        Console.WriteLine($"обновление информации о курсе с кодом {id}");
        bool modified = _courseService.ModifyCourse(id, course);
        if (!modified)
        {
            return NotFound(new { message = "курс для обновления не найден" });
        }

        return Ok(new { message = "курс успешно обновлен" });
    }

    [HttpDelete("{id:int}")]
    [MyAuthorize("Admin")]
    public IActionResult DeleteCourse(int id)
    {
        Console.WriteLine($"удаление курса с кодом {id} администратором системы");
        bool deleted = _courseService.RemoveCourse(id);
        if (!deleted)
        {
            return NotFound(new { message = "курс для удаления не найден" });
        }

        return Ok(new { message = "учебный курс успешно удален" });
    }

    [HttpGet("info")]
    [AllowAnonymous]
    public IActionResult GetPublicInfo()
    {
        Console.WriteLine("выдача общедоступной информации об учебных программах университета");
        return Ok(new
        {
            institution = "Казахский национальный исследовательский технический университет",
            department = "Кафедра Программной инженерии",
            courseName = "CSE5032 Разработка веб-сервисов",
            semester = "Осенний семестр 2026",
            openProgramsCount = 5,
            contactEmail = "pi.support@satbayev.kz"
        });
    }
}

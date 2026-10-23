using UniCoursesApp.Models;

namespace UniCoursesApp.Services;

public class CourseService : ICourseService
{
    private readonly List<Course> _courses = new()
    {
        new Course
        {
            Id = 1,
            Title = "Разработка веб-сервисов на ASP.NET Core",
            Description = "Создание защищенных RESTful Web API, микросервисов и базовая аутентификация",
            Teacher = "Дина Касымова",
            Duration = "60 часов"
        },
        new Course
        {
            Id = 2,
            Title = "Алгоритмы и структуры данных",
            Description = "Изучение базовых и продвинутых структур, оценка сложности алгоритмов",
            Teacher = "Ерсултан Сапаров",
            Duration = "72 часа"
        },
        new Course
        {
            Id = 3,
            Title = "Проектирование баз данных и SQL",
            Description = "Реляционные модели, индексация, оптимизация сложных аналитических запросов",
            Teacher = "Аружан Мусина",
            Duration = "54 часа"
        },
        new Course
        {
            Id = 4,
            Title = "Архитектура облачных систем",
            Description = "Контейнеризация, оркестрация и развертывание сервисов в облаке",
            Teacher = "Алихан Смагулов",
            Duration = "48 часов"
        },
        new Course
        {
            Id = 5,
            Title = "Информационная безопасность веб-приложений",
            Description = "Защита API от распространенных уязвимостей, аутентификация и шифрование",
            Teacher = "Нурлан Сериков",
            Duration = "64 часа"
        }
    };

    private int _nextId = 6;

    public IReadOnlyList<Course> FetchCourses()
    {
        Console.WriteLine("получение списка учебных курсов университета");
        return _courses;
    }

    public Course? FetchById(int id)
    {
        Console.WriteLine($"поиск курса с идентификатором {id}");
        return _courses.FirstOrDefault(c => c.Id == id);
    }

    public Course AddCourse(Course course)
    {
        course.Id = _nextId++;
        _courses.Add(course);
        Console.WriteLine($"добавлен новый курс: {course.Title}, преподаватель {course.Teacher}");
        return course;
    }

    public bool ModifyCourse(int id, Course course)
    {
        var existing = _courses.FirstOrDefault(c => c.Id == id);
        if (existing == null)
        {
            Console.WriteLine($"курс {id} для изменения не найден");
            return false;
        }

        existing.Title = course.Title;
        existing.Description = course.Description;
        existing.Teacher = course.Teacher;
        existing.Duration = course.Duration;

        Console.WriteLine($"данные курса {id} успешно обновлены");
        return true;
    }

    public bool RemoveCourse(int id)
    {
        var existing = _courses.FirstOrDefault(c => c.Id == id);
        if (existing == null)
        {
            Console.WriteLine($"курс {id} для удаления не найден");
            return false;
        }

        _courses.Remove(existing);
        Console.WriteLine($"курс {id} удален из каталога");
        return true;
    }
}

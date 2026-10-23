using UniCoursesApp.Models;

namespace UniCoursesApp.Services;

public interface ICourseService
{
    IReadOnlyList<Course> FetchCourses();
    Course? FetchById(int id);
    Course AddCourse(Course course);
    bool ModifyCourse(int id, Course course);
    bool RemoveCourse(int id);
}

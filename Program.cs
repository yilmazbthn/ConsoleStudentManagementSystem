using Microsoft.EntityFrameworkCore;
using StudentManagementSystem;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using AppDbContext = StudentManagementSystem.Data.AppDbContext;
using DateTime = System.DateTime;
using StudentHomeworkGrade = StudentManagementSystem.Models.StudentHomeworkGrade;

class Program
{
    
    private static AppDbContext _context = new AppDbContext();
    private static User? _loggedInUser;

    private static ConsoleMenu _StudentMenu = new("Öğrenci Menüsü");
    private static ConsoleMenu _InstructorMenu = new("Eğitmen Menüsü");
    private static ConsoleMenu _AdminMenu = new("Yönetici Menüsü");

    static void Main(string[] args)
    {
        _StudentMenu
            .AddMenu("Derslerim", MyLessons)
            .AddMenu("Ödevlerim", MyHomeworks)
            .AddMenu("Notlarım", MyGrades);

        _InstructorMenu
            .AddMenu("Derslerim", InstructorLessons)
            .AddMenu("Ödev İşlemleri", HomeworkMenu)
            .AddMenu("Notlandırma", GradeMenu);

        _AdminMenu
            .AddMenu("Kullanıcı Yönetimi", UserMenu)
            .AddMenu("Sınıf Yönetimi", ClassroomMenu)
            .AddMenu("Ders Yönetimi", LessonMenu)
            .AddMenu("Ödev Yönetimi", HomeworkMenu)
            .AddMenu("Not Yönetimi", GradeMenu)
            .AddMenu("Atama İşlemleri", AssignmentMenu);

        var mainMenu = new ConsoleMenu("Öğrenci Yönetim Sistemi");
        mainMenu
            .AddMenu("Giriş Yap", LoginUser)
            .AddMenu("Kayıt Ol", RegisterUser);

        mainMenu.Show(isRoot: true);
    }
    
    static void LoginUser()
    {
        var username = Helper.Ask("Kullanıcı Adı", true);
        var password = Helper.AskPassword("Şifre");
        var loginStatus = Auth.Login(username, password, out var user);
        switch (loginStatus)
        {
            case Auth.LoginStatus.LoggedIn:
                _loggedInUser = user;
                if (user is Student) _StudentMenu.Show();
                else if (user is Instructor) _InstructorMenu.Show();
                else if (user is Admin) _AdminMenu.Show();
                break;
            case Auth.LoginStatus.UserNotFound:
                Helper.ShowErrorMsg("Kullanıcı Bulunamadı.");
                Thread.Sleep(1000);
                break;
            case Auth.LoginStatus.Wrong:
                Helper.ShowErrorMsg("Kullanıcı adı veya şifre hatalı.");
                Thread.Sleep(1000);
                break;
        }
    }

    static void RegisterUser()
    {
        Console.WriteLine("1-) Öğrenci");
        Console.WriteLine("2-) Eğitmen");
        Console.WriteLine("3-) Yönetici");
        var statusRole = Console.ReadLine();

        UserRoles role = statusRole switch
        {
            "1" => UserRoles.Student,
            "2" => UserRoles.Instructor,
            "3" => UserRoles.Admin,
            _ => UserRoles.InvalidRole
        };
        if (role == UserRoles.InvalidRole)
        {
            Helper.ShowErrorMsg("Geçersiz rol seçimi!");
            return;
        }

        string? inputCourse = null;
        var inputName = Helper.Ask("Ad", true);
        var inputLastName = Helper.Ask("Soyad", true);
        var inputUsername = Helper.Ask("Kullanıcı adı", true);
        if (role == UserRoles.Instructor)
            inputCourse = Helper.Ask("Branşı", true);
        var inputPassword = Helper.AskPassword("Şifre");
        var inputBirthdayStr = Helper.Ask("Doğum Tarihi (yyyy-MM-dd):", true);

        if (!DateOnly.TryParse(inputBirthdayStr, out DateOnly inputBirthday))
        {
            Helper.ShowErrorMsg("Geçersiz tarih formatı!");
            return;
        }

        var registerStatus = Auth.Register(
            inputName,
            inputLastName,
            inputUsername,
            inputBirthday,
            inputPassword,
            role,
            out User? user,
            inputCourse);

        if (registerStatus == Auth.RegisterStatus.UsernameExists)
        {
            Helper.ShowErrorMsg("Bu kullanıcı zaten var!");
            Thread.Sleep(1000);
            return;
        }

        if (user != null)
        {
            if (role == UserRoles.Student)
            {
                Console.WriteLine("Sistemdeki sınıflar:");
                var classes = _context.Classrooms.ToList();
                foreach (var c in classes)
                    Console.WriteLine($"ID: {c.Id} | Ad: {c.ClassroomName}");

                Console.Write("Katılmak istediğiniz sınıfın ID'sini girin: ");
                if (int.TryParse(Console.ReadLine(), out int classId))
                {
                    var selectedClass = _context.Classrooms.FirstOrDefault(c => c.Id == classId);
                    if (selectedClass != null && user is Student student)
                    {
                        selectedClass.Students.Add(student);
                        _context.SaveChanges();
                        Console.WriteLine("Seçtiğiniz sınıfa başarıyla eklendiniz!");
                    }
                    else
                    {
                        Console.WriteLine("Sınıf bulunamadı.");
                    }
                }
                else
                {
                    Console.WriteLine("Geçersiz ID!");
                }

                Thread.Sleep(1000);
            }
            else if (role == UserRoles.Instructor)
            {
                Console.WriteLine("Sistemdeki dersler:");
                var lessons = _context.Lessons.ToList();
                foreach (var l in lessons)
                    Console.WriteLine($"ID: {l.Id} | Ad: {l.Name}");

                Console.Write("Eğitmeni olacağınız dersin ID'sini girin: ");
                if (int.TryParse(Console.ReadLine(), out int lessonId))
                {
                    var selectedLesson = _context.Lessons.FirstOrDefault(l => l.Id == lessonId);
                    if (selectedLesson != null && user is Instructor instructor)
                    {
                        selectedLesson.Instructors.Add(instructor);
                        _context.SaveChanges();
                        Console.WriteLine("Seçtiğiniz derse eğitmen olarak eklendiniz!");
                    }
                    else
                    {
                        Console.WriteLine("Ders bulunamadı.");
                    }
                }
                else
                {
                    Console.WriteLine("Geçersiz ID!");
                }

                Thread.Sleep(1000);
            }

            Console.WriteLine("Kayıt başarılı! Giriş yapabilirsiniz.");
            Thread.Sleep(1000);
        }
    }
        static void UserMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Kullanıcı İşlemleri ===");
                Console.WriteLine("1. Kullanıcıları Listele");
                Console.WriteLine("2. Kullanıcı Sil");
                Console.WriteLine("0. Geri Dön");
                Console.Write("Seçiminiz: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": ListUsers(); break;
                    case "2": DeleteUser(); break;
                    case "0": return;
                    default: Helper.ShowErrorMsg("Geçersiz seçim."); break;
                }
            }
        }

        static void ListUsers()
        {
            Console.Clear();
            var users = _context.Users.ToList();
            Console.WriteLine(">> Kullanıcı Listesi");
            foreach (var u in users)
                Console.WriteLine($"ID: {u.Id} | Ad: {u.FirstName} {u.LastName} | Kullanıcı Adı: {u.Username} | Rol: {u.Role}");
            Helper.ShowInfoMsg("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        static void DeleteUser()
        {
            Console.Clear();
            ListUsers();
            var idStr = Helper.Ask("Silinecek Kullanıcı ID");
            if (int.TryParse(idStr, out int id))
            {
                var user = _context.Users.Find(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    Helper.ShowSuccessMsg("Kullanıcı silindi.");
                }
                else Helper.ShowErrorMsg("Kullanıcı bulunamadı.");
            }
            else Helper.ShowErrorMsg("Geçersiz ID.");
            Console.ReadKey();
        }


        static void ClassroomMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Sınıf İşlemleri ===");
                Console.WriteLine("1. Sınıfları Listele");
                Console.WriteLine("2. Yeni Sınıf Ekle");
                Console.WriteLine("3. Sınıfı Düzenle");
                Console.WriteLine("4. Sınıfı Sil");
                Console.WriteLine("0. Geri Dön");
                Console.Write("Seçiminiz: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": ListClassrooms(); break;
                    case "2": AddClassroom(); break;
                    case "3": EditClassroom(); break;
                    case "4": DeleteClassroom(); break;
                    case "0": return;
                    default: Helper.ShowErrorMsg("Geçersiz seçim."); break;
                }
            }
        }

        static void ListClassrooms()
        {
            Console.Clear();
            var classes = _context.Classrooms.Include(c => c.Students).Include(c => c.Instructors).ToList();
            Console.WriteLine(">> Sınıf Listesi");
            foreach (var c in classes)
                Console.WriteLine($"ID: {c.Id} | Ad: {c.ClassroomName} | Öğrenci: {c.Students.Count} | Eğitmen: {c.Instructors.Count}");
            Helper.ShowInfoMsg("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        static void AddClassroom()
        {
            Console.Clear();
            var name = Helper.Ask("Sınıf adı", true);
            _context.Classrooms.Add(new Classroom { ClassroomName = name });
            _context.SaveChanges();
            Helper.ShowSuccessMsg("Sınıf eklendi.");
            Console.ReadKey();
        }

        static void EditClassroom()
        {
            Console.Clear();
            ListClassrooms();
            var idStr = Helper.Ask("Düzenlenecek Sınıf ID");
            if (int.TryParse(idStr, out int id))
            {
                var c = _context.Classrooms.Find(id);
                if (c != null)
                {
                    var name = Helper.Ask("Yeni sınıf adı (boşsa değişmez)");
                    if (!string.IsNullOrWhiteSpace(name)) c.ClassroomName = name;
                    _context.SaveChanges();
                    Helper.ShowSuccessMsg("Sınıf güncellendi.");
                }
                else Helper.ShowErrorMsg("Sınıf bulunamadı.");
            }
            else Helper.ShowErrorMsg("Geçersiz ID.");
            Console.ReadKey();
        }

        static void DeleteClassroom()
        {
            Console.Clear();
            ListClassrooms();
            var idStr = Helper.Ask("Silinecek Sınıf ID");
            if (int.TryParse(idStr, out int id))
            {
                var c = _context.Classrooms.Find(id);
                if (c != null)
                {
                    _context.Classrooms.Remove(c);
                    _context.SaveChanges();
                    Helper.ShowSuccessMsg("Sınıf silindi.");
                }
                else Helper.ShowErrorMsg("Sınıf bulunamadı.");
            }
            else Helper.ShowErrorMsg("Geçersiz ID.");
            Console.ReadKey();
        }
        
        static void LessonMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Ders İşlemleri ===");
                Console.WriteLine("1. Dersleri Listele");
                Console.WriteLine("2. Yeni Ders Ekle");
                Console.WriteLine("3. Ders Düzenle");
                Console.WriteLine("4. Ders Sil");
                Console.WriteLine("0. Geri Dön");
                Console.Write("Seçiminiz: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": ListLessons(); break;
                    case "2": AddLesson(); break;
                    case "3": EditLesson(); break;
                    case "4": DeleteLesson(); break;
                    case "0": return;
                    default: Helper.ShowErrorMsg("Geçersiz seçim."); break;
                }
            }
        }

        static void ListLessons()
        {
            Console.Clear();
            var lessons = _context.Lessons.Include(l => l.Classrooms).Include(l => l.Instructors).ToList();
            Console.WriteLine(">> Ders Listesi");
            foreach (var lesson in lessons)
                Console.WriteLine($"ID: {lesson.Id} | Ad: {lesson.Name} | Program: {lesson.Schedule} | Sınıflar: {lesson.Classrooms.Count} | Eğitmenler: {lesson.Instructors.Count}");
            Helper.ShowInfoMsg("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        static void AddLesson()
        {
            Console.Clear();
            var name = Helper.Ask("Ders adı", true);
            var schedule = Helper.Ask("Ders programı", true);
            _context.Lessons.Add(new Lesson { Name = name, Schedule = schedule });
            _context.SaveChanges();
            Helper.ShowSuccessMsg("Ders başarıyla eklendi.");
            Console.ReadKey();
        }

        static void EditLesson()
        {
            Console.Clear();
            ListLessons();
            var idStr = Helper.Ask("Düzenlenecek Ders ID");
            if (int.TryParse(idStr, out int id))
            {
                var lesson = _context.Lessons.Find(id);
                if (lesson != null)
                {
                    var name = Helper.Ask("Yeni ders adı (boşsa değişmez)");
                    if (!string.IsNullOrWhiteSpace(name)) lesson.Name = name;
                    var schedule = Helper.Ask("Yeni ders programı (boşsa değişmez)");
                    if (!string.IsNullOrWhiteSpace(schedule)) lesson.Schedule = schedule;
                    _context.SaveChanges();
                    Helper.ShowSuccessMsg("Ders güncellendi.");
                }
                else Helper.ShowErrorMsg("Ders bulunamadı.");
            }
            else Helper.ShowErrorMsg("Geçersiz ID.");
            Console.ReadKey();
        }

        static void DeleteLesson()
        {
            Console.Clear();
            ListLessons();
            var idStr = Helper.Ask("Silinecek Ders ID");
            if (int.TryParse(idStr, out int id))
            {
                var lesson = _context.Lessons.Find(id);
                if (lesson != null)
                {
                    _context.Lessons.Remove(lesson);
                    _context.SaveChanges();
                    Helper.ShowSuccessMsg("Ders silindi.");
                }
                else Helper.ShowErrorMsg("Ders bulunamadı.");
            }
            else Helper.ShowErrorMsg("Geçersiz ID.");
            Console.ReadKey();
        }
        
        static void HomeworkMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Ödev İşlemleri ===");
                Console.WriteLine("1. Ödevleri Listele");
                Console.WriteLine("2. Yeni Ödev Ekle");
                Console.WriteLine("3. Ödev Düzenle");
                Console.WriteLine("4. Ödev Sil");
                Console.WriteLine("0. Geri Dön");
                Console.Write("Seçiminiz: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": ListHomeworks(); break;
                    case "2": AddHomework(); break;
                    case "3": EditHomework(); break;
                    case "4": DeleteHomework(); break;
                    case "0": return;
                    default: Helper.ShowErrorMsg("Geçersiz seçim."); break;
                }
            }
        }

        static void ListHomeworks()
        {
            Console.Clear();
            var homeworks = _context.Homeworks.Include(h => h.Lesson).ToList();
            Console.WriteLine(">> Ödev Listesi");
            foreach (var hw in homeworks)
                Console.WriteLine($"ID: {hw.Id} | Başlık: {hw.Title} | Ders: {hw.Lesson?.Name} | Son Tarih: {hw.DueDate:yyyy-MM-dd}");
            Helper.ShowInfoMsg("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        static void AddHomework()
        {
            Console.Clear();
            ListLessons();
            var lessonIdStr = Helper.Ask("Bağlı Ders ID", true);
            if (!int.TryParse(lessonIdStr, out int lessonId))
            {
                Helper.ShowErrorMsg("Geçersiz ders ID.");
                Console.ReadKey();
                return;
            }
            var title = Helper.Ask("Başlık", true);
            var desc = Helper.Ask("Açıklama", true);
            var dueDateStr = Helper.Ask("Son Teslim Tarihi (yyyy-MM-dd)", true);
            if (!DateTime.TryParse(dueDateStr, out DateTime dueDate))
            {
                Helper.ShowErrorMsg("Geçersiz tarih.");
                Console.ReadKey();
                return;
            }
            _context.Homeworks.Add(new Homework { LessonId = lessonId, Title = title, Description = desc, DueDate = dueDate });
            _context.SaveChanges();
            Helper.ShowSuccessMsg("Ödev başarıyla eklendi.");
            Console.ReadKey();
        }

        static void EditHomework()
        {
            Console.Clear();
            ListHomeworks();
            var idStr = Helper.Ask("Düzenlenecek Ödev ID");
            if (int.TryParse(idStr, out int id))
            {
                var hw = _context.Homeworks.Find(id);
                if (hw != null)
                {
                    var title = Helper.Ask("Yeni başlık (boşsa değişmez)");
                    if (!string.IsNullOrWhiteSpace(title)) hw.Title = title;
                    var desc = Helper.Ask("Yeni açıklama (boşsa değişmez)");
                    if (!string.IsNullOrWhiteSpace(desc)) hw.Description = desc;
                    var dateStr = Helper.Ask("Yeni teslim tarihi (yyyy-MM-dd, boşsa değişmez)");
                    if (!string.IsNullOrWhiteSpace(dateStr) && DateTime.TryParse(dateStr, out var dd)) hw.DueDate = dd;
                    _context.SaveChanges();
                    Helper.ShowSuccessMsg("Ödev güncellendi.");
                }
                else Helper.ShowErrorMsg("Ödev bulunamadı.");
            }
            else Helper.ShowErrorMsg("Geçersiz ID.");
            Console.ReadKey();
        }

        static void DeleteHomework()
        {
            Console.Clear();
            ListHomeworks();
            var idStr = Helper.Ask("Silinecek Ödev ID");
            if (int.TryParse(idStr, out int id))
            {
                var hw = _context.Homeworks.Find(id);
                if (hw != null)
                {
                    _context.Homeworks.Remove(hw);
                    _context.SaveChanges();
                    Helper.ShowSuccessMsg("Ödev silindi.");
                }
                else Helper.ShowErrorMsg("Ödev bulunamadı.");
            }
            else Helper.ShowErrorMsg("Geçersiz ID.");
            Console.ReadKey();
        }


        static void GradeMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Not İşlemleri ===");
                Console.WriteLine("1. Notları Listele");
                Console.WriteLine("2. Not Ekle/Güncelle");
                Console.WriteLine("3. Not Sil");
                Console.WriteLine("0. Geri Dön");
                Console.Write("Seçiminiz: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": ListGrades(); break;
                    case "2": AddOrUpdateGrade(); break;
                    case "3": DeleteGrade(); break;
                    case "0": return;
                    default: Helper.ShowErrorMsg("Geçersiz seçim."); break;
                }
            }
        }

        static void ListGrades()
        {
            Console.Clear();
            var grades = _context.StudentHomeworkGrades
                .Include(g => g.Student)
                .Include(g => g.Homework)
                .ThenInclude(h => h.Lesson)
                .ToList();
            Console.WriteLine(">> Not Listesi");
            foreach (var g in grades)
                Console.WriteLine($"ID: {g.Id} | Öğrenci: {g.Student?.FirstName} {g.Student?.LastName} | Ödev: {g.Homework?.Title} | Ders: {g.Homework?.Lesson?.Name} | Not: {g.Grade} | Geri Bildirim: {g.Feedback}");
            Helper.ShowInfoMsg("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        static void AddOrUpdateGrade()
        {
            Console.Clear();
            ListHomeworks();
            var hwIdStr = Helper.Ask("Not verilecek Ödev ID", true);
            if (!int.TryParse(hwIdStr, out int hwId))
            {
                Helper.ShowErrorMsg("Geçersiz ödev ID.");
                Console.ReadKey();
                return;
            }
            var studentIdStr = Helper.Ask("Öğrenci ID", true);
            if (!int.TryParse(studentIdStr, out int studentId))
            {
                Helper.ShowErrorMsg("Geçersiz öğrenci ID.");
                Console.ReadKey();
                return;
            }
            var gradeStr = Helper.Ask("Not (0-100)", true);
            if (!int.TryParse(gradeStr, out int grade) || grade < 0 || grade > 100)
            {
                Helper.ShowErrorMsg("Geçersiz not.");
                Console.ReadKey();
                return;
            }
            var feedback = Helper.Ask("Geri Bildirim");
            var gradeEntry = _context.StudentHomeworkGrades
                .FirstOrDefault(g => g.HomeworkId == hwId && g.StudentId == studentId);
            if (gradeEntry == null)
            {
                _context.StudentHomeworkGrades.Add(new StudentHomeworkGrade
                {
                    HomeworkId = hwId,
                    StudentId = studentId,
                    Grade = grade,
                    Feedback = feedback
                });
            }
            else
            {
                gradeEntry.Grade = grade;
                gradeEntry.Feedback = feedback;
            }
            _context.SaveChanges();
            Helper.ShowSuccessMsg("Not kaydedildi.");
            Console.ReadKey();
        }

        static void DeleteGrade()
        {
            Console.Clear();
            ListGrades();
            var idStr = Helper.Ask("Silinecek Not Kayıt ID");
            if (int.TryParse(idStr, out int id))
            {
                var grade = _context.StudentHomeworkGrades.Find(id);
                if (grade != null)
                {
                    _context.StudentHomeworkGrades.Remove(grade);
                    _context.SaveChanges();
                    Helper.ShowSuccessMsg("Not silindi.");
                }
                else Helper.ShowErrorMsg("Not bulunamadı.");
            }
            else Helper.ShowErrorMsg("Geçersiz ID.");
            Console.ReadKey();
        }
        
        static void AssignmentMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Atama İşlemleri ===");
                Console.WriteLine("1. Sınıfa Öğrenci Atama");
                Console.WriteLine("2. Sınıfa Eğitmen Atama");
                Console.WriteLine("3. Derse Eğitmen Atama");
                Console.WriteLine("0. Geri Dön");
                Console.Write("Seçiminiz: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AssignStudentToClass(); break;
                    case "2": AssignInstructorToClass(); break;
                    case "3": AssignInstructorToLesson(); break;
                    case "0": return;
                    default: Helper.ShowErrorMsg("Geçersiz seçim."); break;
                }
            }
        }

        static void AssignStudentToClass()
        {
            Console.Clear();
            var students = _context.Students.ToList();
            var classes = _context.Classrooms.Include(c => c.Students).ToList();
            if (students.Count == 0 || classes.Count == 0)
            {
                Helper.ShowErrorMsg("Öğrenci veya sınıf yok!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Öğrenciler:");
            foreach (var s in students)
                Console.WriteLine($"ID: {s.Id} | {s.FirstName} {s.LastName}");

            var studentId = Helper.Ask("Atanacak öğrenci ID", true);
            var sId = int.Parse(studentId);

            Console.WriteLine("Sınıflar:");
            foreach (var c in classes)
                Console.WriteLine($"ID: {c.Id} | {c.ClassroomName}");

            var classId = Helper.Ask("Atanacağı Sınıf ID", true);
            var cId = int.Parse(classId);

            var student = _context.Students.Include(s => s.Classrooms).First(x => x.Id == sId);
            var classroom = _context.Classrooms.Include(c => c.Students).First(x => x.Id == cId);

            if (!classroom.Students.Contains(student))
                classroom.Students.Add(student);

            _context.SaveChanges();
            Helper.ShowSuccessMsg("Atama tamamlandı.");
            Console.ReadKey();
        }

        static void AssignInstructorToClass()
        {
            Console.Clear();
            var instructors = _context.Instructors.ToList();
            var classes = _context.Classrooms.Include(c => c.Instructors).ToList();
            if (instructors.Count == 0 || classes.Count == 0)
            {
                Helper.ShowErrorMsg("Eğitmen veya sınıf yok!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Eğitmenler:");
            foreach (var i in instructors)
                Console.WriteLine($"ID: {i.Id} | {i.FirstName} {i.LastName}");

            var instructorId = Helper.Ask("Atanacak eğitmen ID", true);
            var iId = int.Parse(instructorId);

            Console.WriteLine("Sınıflar:");
            foreach (var c in classes)
                Console.WriteLine($"ID: {c.Id} | {c.ClassroomName}");

            var classId = Helper.Ask("Atanacağı Sınıf ID", true);
            var cId = int.Parse(classId);

            var instructor = _context.Instructors.Include(i => i.Classrooms).First(x => x.Id == iId);
            var classroom = _context.Classrooms.Include(c => c.Instructors).First(x => x.Id == cId);

            if (!classroom.Instructors.Contains(instructor))
                classroom.Instructors.Add(instructor);

            _context.SaveChanges();
            Helper.ShowSuccessMsg("Atama tamamlandı.");
            Console.ReadKey();
        }

        static void AssignInstructorToLesson()
        {
            Console.Clear();
            var instructors = _context.Instructors.ToList();
            var lessons = _context.Lessons.Include(l => l.Instructors).ToList();
            if (instructors.Count == 0 || lessons.Count == 0)
            {
                Helper.ShowErrorMsg("Eğitmen veya ders yok!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Eğitmenler:");
            foreach (var i in instructors)
                Console.WriteLine($"ID: {i.Id} | {i.FirstName} {i.LastName}");

            var instructorId = Helper.Ask("Atanacak eğitmen ID", true);
            var iId = int.Parse(instructorId);

            Console.WriteLine("Dersler:");
            foreach (var l in lessons)
                Console.WriteLine($"ID: {l.Id} | {l.Name}");

            var lessonId = Helper.Ask("Atanacağı Ders ID", true);
            var lId = int.Parse(lessonId);

            var instructor = _context.Instructors.Include(i => i.Lessons).First(x => x.Id == iId);
            var lesson = _context.Lessons.Include(l => l.Instructors).First(x => x.Id == lId);

            if (!lesson.Instructors.Contains(instructor))
                lesson.Instructors.Add(instructor);

            _context.SaveChanges();
            Helper.ShowSuccessMsg("Atama tamamlandı.");
            Console.ReadKey();
        }
        
        static void MyLessons()
        {
            Console.Clear();
            var student = _loggedInUser as Student;
            var classrooms = _context.Students
                .Include(s => s.Classrooms)
                .ThenInclude(c => c.Lessons)
                .FirstOrDefault(s => s.Id == student.Id)?
                .Classrooms ?? new List<Classroom>();

            Console.WriteLine("Dersleriniz:");
            var lessons = classrooms.SelectMany(c => c.Lessons).Distinct();
            foreach (var l in lessons)
                Console.WriteLine($"ID: {l.Id} | {l.Name} | {l.Schedule}");

            Helper.ShowInfoMsg("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        static void MyHomeworks()
        {
            Console.Clear();
            var student = _loggedInUser as Student;
            var homeworks = _context.StudentHomeworkGrades
                .Include(g => g.Homework)
                .Where(g => g.StudentId == student.Id)
                .Select(g => g.Homework)
                .Distinct()
                .ToList();

            Console.WriteLine("Ödevleriniz:");
            foreach (var hw in homeworks)
                Console.WriteLine($"ID: {hw.Id} | {hw.Title} | Son Tarih: {hw.DueDate:yyyy-MM-dd}");

            Helper.ShowInfoMsg("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        static void MyGrades()
        {
            Console.Clear();
            var student = _loggedInUser as Student;
            var grades = _context.StudentHomeworkGrades
                .Include(g => g.Homework)
                .ThenInclude(hw => hw.Lesson) 
                .Where(g => g.StudentId == student.Id)
                .ToList();

            Console.WriteLine("Notlarınız:");
            foreach (var g in grades)
                Console.WriteLine($"Ödev: {g.Homework?.Title} | Ders: {g.Homework?.Lesson?.Name} | Not: {g.Grade} | Geri Bildirim: {g.Feedback}");

            Helper.ShowInfoMsg("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }
        
        static void InstructorLessons()
        {
            Console.Clear();
            var instructor = _loggedInUser as Instructor;
            var lessons = _context.Lessons
                .Include(l => l.Instructors)
                .Where(l => l.Instructors.Any(i => i.Id == instructor.Id))
                .ToList();

            Console.WriteLine("Dersleriniz:");
            foreach (var l in lessons)
                Console.WriteLine($"ID: {l.Id} | {l.Name} | {l.Schedule}");

            Helper.ShowInfoMsg("Devam etmek için bir tuşa basın...");
            Console.ReadKey();
        }
}

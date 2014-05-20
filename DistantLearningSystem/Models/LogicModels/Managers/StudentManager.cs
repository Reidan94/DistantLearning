using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DistantLearningSystem.Models.DataModels;
using DistantLearningSystem.Models.LogicModels.ViewModels;
using DistantLearningSystem.Models.LogicModels.Services;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class StudentManager : Manager
    {
        public ProcessResult RegistrateStudent(
            HttpContextBase context,
            Student student,
            HttpServerUtilityBase server,
            HttpPostedFileBase imageUpload)
        {
            Func<Student, bool> func = x => x.Email == student.Email;
            var exists = GetStudent(func);
            student.Password = Security.GetHashString(student.Password);
            student.Activation = (int)UserStatus.Unconfirmed;
            if (exists != null)
                return ProcessResults.UserAlreadyExists;

            if (imageUpload != null)
            {
                if (imageUpload.ContentLength <= 0 || !Security.IsImage(imageUpload))
                    return ProcessResults.InvalidImageFormat;

                student.ImgSrc = SaveImage(student.Id, 
                    StaticSettings.AvatarsUploadFolderPath, 
                    imageUpload, 
                    server);
                SaveChanges();
            }

            if (!SendConfirmationMail(context, student.Email, student.Password, UserType.Student.ToString()))
                return ProcessResults.ErrorOccured;

            var st = entities.Students.Add(student);
            SaveChanges();
            return ProcessResults.RegistrationCompleted;
        }

        public void SetLastVisitDate(int studentId, DateTime date)
        {
            var student = GetStudent(x => x.Id == studentId);
            student.LastVisitDate = date;
            SaveChanges();
        }

        public Student LogInStudent(LoginModel model)
        {
            var find = entities.Students.FirstOrDefault(x =>
                (x.Login == model.LoginOrEmail ||
                x.Email == model.LoginOrEmail) &&
                Security.GetHashString(model.Password) == x.Password);

            return find;
        }


        public Student GetStudent(Func<Student, bool> predicate, bool confirmedOnly = true)
        {
            foreach (var student in entities.Students)
            {
                if (confirmedOnly)
                {
                    if (predicate(student) && (UserStatus)student.Activation == UserStatus.Confirmed)
                        return student;
                }
                else 
                {
                    if (predicate(student))
                        return student;
                }
            }

            return null;
        }

        public bool RemoveStudent(int studentId)
        {
            var studentToRemove = entities.Students.FirstOrDefault(x => x.Id == studentId);
            if (studentToRemove == null)
                return false;

            entities.Students.Remove(studentToRemove);
            SaveChanges();
            return true;
        }

        public IEnumerable<Student> GetStudents() 
        {
            return entities.Students;
        }

        public bool EditStudent(Student newStudent)
        {
            var studentToEdit = entities.Students.FirstOrDefault(x => x.Id == newStudent.Id);
            if (studentToEdit == null)
                return false;
            studentToEdit.Login = newStudent.Login;
            studentToEdit.Name = newStudent.Name;
            studentToEdit.Password = newStudent.Password;
            studentToEdit.Email = newStudent.Email;
            studentToEdit.GroupId = newStudent.GroupId;
            studentToEdit.Email = newStudent.Email;
            SaveChanges();
            return true;
        }

        public StudentConnection AddStudentConnection(int studentId, int connectionId)
        {
            var studConnection = entities.StudentConnections.Add(new StudentConnection()
            {
                AddedDate = DateTime.Now,
                ConnectionId = connectionId,
                StudentId = studentId
            });

            SaveChanges();
            return studConnection;
        }

        public bool RemoveStudentConnection(int studentConnectionId)
        {
            var connectionToRemove = entities.StudentConnections.FirstOrDefault(x => x.Id == studentConnectionId);
            if (connectionToRemove == null)
                return false;

            entities.StudentConnections.Remove(connectionToRemove);
            SaveChanges();
            return true;
        }
    }
}
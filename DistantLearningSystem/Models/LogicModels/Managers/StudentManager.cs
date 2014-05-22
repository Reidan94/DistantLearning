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
            Func<Student, bool> func = x => x.Email == student.Email && x.Login == student.Login;
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
                    student.Email,
                    imageUpload,
                    server);
            }

            if (!SendConfirmationMail(context, student.Email, student.Password, UserType.Student.ToString()))
                return ProcessResults.ErrorOccured;
            student.LastVisitDate = DateTime.Now;
            student.RegDate = DateTime.Now;
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
            var find = entities.Students.ToList().FirstOrDefault(x =>
                (x.Login == model.LoginOrEmail ||
                x.Email == model.LoginOrEmail) &&
                model.Password == x.Password);

            if (find == null)
                return null;

            UpdateLastVisitDate(find);
            SaveChanges();
            return find;
        }

        public void UpdateLastVisitDate(int id)
        {
            var std = GetStudent(id);
            if (std == null)
                return;
            std.LastVisitDate = DateTime.Now;
            SaveChanges();
        }

        public void UpdateLastVisitDate(Student student)
        {
            student.LastVisitDate = DateTime.Now;
            SaveChanges();
        }

        public Student GetStudent(int id)
        {
            return entities.Students.FirstOrDefault(x => x.Id == id);
        }

        public Student GetStudent(Func<Student, bool> predicate, bool confirmedOnly = false)
        {
            foreach (var student in entities.Students.ToList())
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
            return entities.Students.ToList();
        }

        public ProcessResult EditStudent(Student newStudent,
            HttpServerUtilityBase server,
            HttpPostedFileBase imageUpload)
        {
            var studentToEdit = entities.Students.FirstOrDefault(x => x.Id == newStudent.Id);
            if (studentToEdit == null)
                return ProcessResults.ErrorOccured;

            studentToEdit.Login = newStudent.Login;
            studentToEdit.Name = newStudent.Name;
            if (!String.IsNullOrEmpty(newStudent.Password))
                studentToEdit.Password = Security.GetHashString(newStudent.Password);
            studentToEdit.Email = newStudent.Email;
            studentToEdit.LastName = newStudent.LastName;
            if (imageUpload != null)
            {
                if (imageUpload.ContentLength <= 0 || !Security.IsImage(imageUpload))
                    return ProcessResults.InvalidImageFormat;

                if (studentToEdit.ImgSrc != null)
                    DeleteImage(studentToEdit.ImgSrc, server);
                studentToEdit.ImgSrc = SaveImage(studentToEdit.Id,
                    StaticSettings.AvatarsUploadFolderPath,
                    studentToEdit.Email,
                    imageUpload,
                    server);
            }
            else if (!String.IsNullOrEmpty(studentToEdit.ImgSrc)) 
            {
                DeleteImage(studentToEdit.ImgSrc, server);
                studentToEdit.ImgSrc = null;
            }

            SaveChanges();
            return ProcessResults.EditedSuccessfully;
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
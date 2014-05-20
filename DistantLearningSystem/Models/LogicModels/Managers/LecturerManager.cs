using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DistantLearningSystem.Models.DataModels;
using DistantLearningSystem.Models.LogicModels.ViewModels;
using DistantLearningSystem.Models.LogicModels.Services;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class LecturerManager : Manager
    {
        public ProcessResult RegistrateLecturer(
            HttpContextBase context,
            Lecturer lecturer,
            HttpServerUtilityBase server,
            HttpPostedFileBase imageUpload)
        {
            Func<Lecturer, bool> func = x => x.Email == lecturer.Email;
            var exists = GetLecturer(func);
            lecturer.Password = Security.GetHashString(lecturer.Password);
            lecturer.Activation = (int)UserStatus.Unconfirmed;
            if (exists != null)
                return ProcessResults.UserAlreadyExists;

            if (imageUpload != null)
            {
                if (imageUpload.ContentLength <= 0 || !Security.IsImage(imageUpload))
                    return ProcessResults.InvalidImageFormat;

                lecturer.ImgSrc = SaveImage(lecturer.Id,
                    StaticSettings.AvatarsUploadFolderPath,
                    imageUpload,
                    server);
                SaveChanges();
            }

            if (!SendConfirmationMail(context, 
                lecturer.Email, 
                lecturer.Password, 
                UserType.Lecturer.ToString()))
                return ProcessResults.ErrorOccured;

            var st = entities.Lecturers.Add(lecturer);
            SaveChanges();
            return ProcessResults.RegistrationCompleted;
        }

        public Lecturer LogInLecturer(LoginModel model)
        {
            var find = entities.Lecturers.FirstOrDefault(x =>
                (x.Login == model.LoginOrEmail ||
                x.Email == model.LoginOrEmail) &&
                Security.GetHashString(model.Password) == x.Password);

            return find;
        }

        public void SetLastVisitDate(int lecturerId, DateTime date)
        {
            var lecturer = GetLecturer(x => x.Id == lecturerId);
            lecturer.LastVisitDate = date;
            SaveChanges();
        }

        public IEnumerable<Lecturer> GetLectures() 
        {
            return entities.Lecturers;
        }

        public bool RemoveLecturer(int lecturerId)
        {
            var lecturerToRemove = entities.Lecturers.FirstOrDefault(x => x.Id == lecturerId);
            if (lecturerToRemove == null)
                return false;

            entities.Lecturers.Remove(lecturerToRemove);
            SaveChanges();
            return true;
        }

        public Lecturer GetLecturer(Func<Lecturer, bool> predicate, bool confirmedOnly = true)
        {
            return entities.Lecturers.FirstOrDefault(x =>
                predicate(x) &&
                (confirmedOnly ? x.Activation == 1 :
                true));
        }

        public bool EditLecturer(Lecturer newLecturer)
        {
            var lecturerToEdit = entities.Lecturers.FirstOrDefault(x => x.Id == newLecturer.Id);
            if (lecturerToEdit == null)
                return false;
            lecturerToEdit.Login = newLecturer.Login;
            lecturerToEdit.Name = newLecturer.Name;
            lecturerToEdit.Password = newLecturer.Password;
            lecturerToEdit.Position = newLecturer.Position;
            lecturerToEdit.Subject = newLecturer.Subject;
            lecturerToEdit.Department = newLecturer.Department;
            lecturerToEdit.Email = newLecturer.Email;
            SaveChanges();
            return true;
        }
    }
}
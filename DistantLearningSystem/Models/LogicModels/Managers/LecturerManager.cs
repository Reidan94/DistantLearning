﻿using System;
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
                    lecturer.Email,
                    imageUpload,
                    server);
            }

            if (!SendConfirmationMail(context,
                lecturer.Email,
                lecturer.Password,
                UserType.Lecturer.ToString()))
                return ProcessResults.ErrorOccured;

            lecturer.LastVisitDate = DateTime.Now;

            var st = entities.Lecturers.Add(lecturer);
            SaveChanges();
            return ProcessResults.RegistrationCompleted;
        }

        public Lecturer LogInLecturer(LoginModel model)
        {
            var find = entities.Lecturers.ToList().FirstOrDefault(x =>
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
            var lecturer = GetLecturer(id);
            if (lecturer == null)
                return;
            lecturer.LastVisitDate = DateTime.Now;
            SaveChanges();
        }

        public void UpdateLastVisitDate(Lecturer lecturer)
        {
            if (lecturer == null)
                return;
            lecturer.LastVisitDate = DateTime.Now;
            SaveChanges();
        }

        public Lecturer GetLecturer(int id)
        {
            return entities.Lecturers.FirstOrDefault(x => x.Id == id);
        }

        public void UpdateLastVisitDate(Student student)
        {
            student.LastVisitDate = DateTime.Now;
            SaveChanges();
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
            foreach (var lecturer in entities.Lecturers.ToList())
            {
                if (confirmedOnly)
                {
                    if (predicate(lecturer) && (UserStatus)lecturer.Activation == UserStatus.Confirmed)
                        return lecturer;
                }
                else
                {
                    if (predicate(lecturer))
                        return lecturer;
                }
            }

            return null;

        }

        public ProcessResult EditLecturer(Lecturer newLecturer,
            HttpServerUtilityBase server,
            HttpPostedFileBase imageUpload)
        {
            var lecturerToEdit = entities.Lecturers.FirstOrDefault(x => x.Id == newLecturer.Id);
            if (lecturerToEdit == null)
                return ProcessResults.ErrorOccured;

            lecturerToEdit.Login = newLecturer.Login;
            lecturerToEdit.Name = newLecturer.Name;
            if (!String.IsNullOrEmpty(newLecturer.Password))
                lecturerToEdit.Password = Security.GetHashString(newLecturer.Password);
            lecturerToEdit.Email = newLecturer.Email;
            lecturerToEdit.LastName = newLecturer.LastName;
            if (imageUpload != null)
            {
                if (imageUpload.ContentLength <= 0 || !Security.IsImage(imageUpload))
                    return ProcessResults.InvalidImageFormat;

                if (lecturerToEdit.ImgSrc != null)
                    DeleteImage(lecturerToEdit.ImgSrc, server);
                lecturerToEdit.ImgSrc = SaveImage(lecturerToEdit.Id,
                    StaticSettings.AvatarsUploadFolderPath,
                    lecturerToEdit.Email,
                    imageUpload,
                    server);
            }
            else if(!String.IsNullOrEmpty(lecturerToEdit.ImgSrc))
            {
                DeleteImage(lecturerToEdit.ImgSrc, server);
                lecturerToEdit.ImgSrc = null;
            }

            SaveChanges();
            return ProcessResults.EditedSuccessfully;
        }
    }
}
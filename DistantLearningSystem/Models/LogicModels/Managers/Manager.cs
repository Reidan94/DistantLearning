using System;
using System.Collections.Generic;
using System.Linq;
using DistantLearningSystem.Models.DataModels;
using System.Web;
using System.IO;
using DistantLearningSystem.Models.LogicModels.Services;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public abstract class Manager
    {
        protected CourseDataBaseEntities entities;

        public Manager()
        {
            entities = new CourseDataBaseEntities();
        }

        protected string SaveImage(int id, string folder, HttpPostedFileBase imageUpload, HttpServerUtilityBase server)
        {
            string relativePath = folder + id + Path.GetExtension(imageUpload.FileName);
            imageUpload.SaveAs(server.MapPath("~") + relativePath);
            return relativePath;
        }

        protected void DeleteImage(string virtualPath, HttpServerUtilityBase server)
        {
            if (virtualPath != null)
            {
                string path = server.MapPath("~") + virtualPath;
                var file = new FileInfo(path);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
        }

        protected bool SendConfirmationMail(HttpContextBase context, string email, string password, string type)
        {
            var confirmationMessageSender = new ConfirmationMailSender();
            string token = Security.GetHashString(email + password + type);
            
            if (context.Request.Url != null)
            {
                string path = context.Request.Url.GetLeftPart(UriPartial.Authority) + "/User/Confirm?hash=" + token;
                string message = String.Format(StaticSettings.ConfirmationMessage + "{0}", path);
                return confirmationMessageSender.Send(StaticSettings.ConfirmationTitle, message, email);
            }

            return false;
        }

        protected void SaveChanges()
        {
            entities.SaveChanges();
        }
    }
}
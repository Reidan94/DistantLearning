using DistantLearningSystem.Models.DataModels;
using DistantLearningSystem.Models.LogicModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class AuthentificationManager : Manager
    {
        public ProcessResult LogInUser(LoginModel model, out UserModel userModel)
        {
            if (model.UserType == UserType.Student)
                userModel = (UserModel)DataManager.Lecturer.LogInLecturer(model);
            else userModel = (UserModel)DataManager.Student.LogInStudent(model);

            if (userModel == null)
                return ProcessResults.InvalidEmailOrPassword;

            return ProcessResults.LoggedInSuccessfull;
        }
    }
}
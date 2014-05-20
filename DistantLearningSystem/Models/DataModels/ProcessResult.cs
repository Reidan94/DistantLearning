using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistantLearningSystem.Models.DataModels
{
    //Класс будет использоваться для оповещения пользователя 
    //при выполнении каких-то действий на сайте
    //Не смотрите на излишнюю избыточность, я его взял из туриста, если что уберем ненужное
    public class ProcessResult
    {
        public int Id { get; private set; }

        public bool Succeeded { get; private set; }

        public string Message { get; set; }

        public bool IsEmpty
        {
            get { return Succeeded == false && Message == null; }
        }

        public ProcessResult(int id, bool succeeded, string message)
        {
            Id = id;
            Succeeded = succeeded;

        }
    }

    public class ProcessResults
    {
        static readonly ProcessResult[] Results =
        {
            new ProcessResult(0, false, "Неверный email или пароль!"), 
            new ProcessResult(1, false, "Пользователь с такими данными уже существует!"),
            new ProcessResult(2, true, "Вы успешно прошли регестрацию. На Ваш почтовый ящик выслано письмо с подтверждением регистрации!"),
            new ProcessResult(3, true, "Регистраци подтверждена! Теперь Вы можете пользоваться своим личным кабинетом!"),
            new ProcessResult(4, true, "Вы вошли в личный кабинет!"),
            new ProcessResult(5, false, "Неправильный фомат картинки!"),
            new ProcessResult(6, false, "Произошла ошибка!")
        };

        public static ProcessResult GetById(int id = -1)
        {
            if (id < 0 || id > Results.Length) return null;
            return Results[id];
        }

        public static ProcessResult InvalidEmailOrPassword
        {
            get { return Results[0]; }
        }

        public static ProcessResult UserAlreadyExists
        {
            get { return Results[1]; }
        }

        public static ProcessResult RegistrationCompleted
        {
            get { return Results[2]; }
        }

        public static ProcessResult RegistrationConfirmed
        {
            get { return Results[3]; }
        }

        public static ProcessResult LoggedInSuccessfull
        {
            get { return Results[4]; }
        }

        public static ProcessResult InvalidImageFormat
        {
            get { return Results[5]; }
        }

        public static ProcessResult ErrorOccured
        {
            get { return Results[6]; }
        }
    }
}

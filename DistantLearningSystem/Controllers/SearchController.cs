using DistantLearningSystem.Models.LogicModels.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DistantLearningSystem.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        public ActionResult Concept(int id) 
        {
            var concept = DataManager.Concept.GetConcept(id);
            return View(concept);
        }

        public ActionResult GetGroups()
        {
            return View(DataManager.University.GetGroups());
        }

        public ActionResult GetConcepts() 
        {
            return View(DataManager.Concept.GetConcepts());
        }

        public ActionResult GetLectures() 
        {
            return View(DataManager.Lecturer.GetLectures());
        }

        public ActionResult GetStudents() 
        {
            var students = DataManager.Student.GetStudents();
            return View(students);
        }

        public ActionResult GetGroup(int Id) 
        {
            var group = DataManager.University.GetGroup(Id);
            return View(group);
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}
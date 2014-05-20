using DistantLearningSystem.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class ClassificationManager : Manager
    {
        public ClassificationType AddClassificationType(ClassificationType newType)
        {
            var exists = entities.ClassificationTypes.FirstOrDefault(x => x.Name.ToLower() == newType.Name.ToLower());
            if (exists != null)
                return null;

            var cstypes = entities.ClassificationTypes.Add(newType);
            SaveChanges();
            return cstypes;
        }

        public Classification Add(Classification classification)
        {
            var exists = entities.Classifications.FirstOrDefault(x =>
                x.ClassificationTypeId == classification.ClassificationTypeId &&
                x.Base.ToLower() == classification.Base.ToLower());

            if (exists != null)
                return null;

            var cs = entities.Classifications.Add(classification);
            SaveChanges();

            return cs;
        }

        public IEnumerable<Classification> GetAll()
        {
            return entities.Classifications;
        }

        public Classification GetClassification(int classificationId)
        {
            return entities.Classifications.FirstOrDefault(x => x.Id == classificationId);
        }

        public void EditClassification(Classification newClassification)
        {
            var oldClassification = GetClassification(newClassification.Id);
            oldClassification.Rating = newClassification.Rating;
            oldClassification.Status = newClassification.Status;
            oldClassification.StudentId = newClassification.StudentId;
            oldClassification.Base = newClassification.Base;
            oldClassification.ClassificationTypeId = newClassification.ClassificationTypeId;
            SaveChanges();
        }

        public void SetRating(int classificationId, int rating)
        {
            var classsification = GetClassification(classificationId);
            classsification.Rating = rating;
            SaveChanges();
        }

        public void SetCheckStatus(int classificationId, CheckStatus check)
        {
            var classification = GetClassification(classificationId);
            classification.Status = (int)check;
            SaveChanges();
        }

        public bool Remove(int classificationId)
        {
            var classification = GetClassification(classificationId);
            if (classification == null)
                return false;
            entities.Classifications.Remove(classification);
            SaveChanges();
            return true;
        }
    }
}
using DistantLearningSystem.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class SourceManager : Manager
    {
        public Source AddSource(Source source)
        {
            var src = entities.Sources.Add(source);
            SaveChanges();
            return src;
        }

        public Source GetSource(int sourceId)
        {
            return entities.Sources.FirstOrDefault(x => x.Id == sourceId);
        }

        public bool EditSsource(Source editedSource) 
        {
            var src = GetSource(editedSource.Id);
            if (src == null)
                return false;
            src.Type = editedSource.Type;
            src.Author = editedSource.Author;
            src.FullTitle = editedSource.FullTitle;
            src.PublicationYear = editedSource.PublicationYear;
            SaveChanges();
            return true;
        }

        public bool DeleteSource(int sourceId)
        {
            var source = GetSource(sourceId);
            if (source == null)
                return false;

            entities.Sources.Remove(source);
            SaveChanges();
            return true;
        }
    }
}
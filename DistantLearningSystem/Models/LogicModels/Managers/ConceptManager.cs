using System;
using System.Collections.Generic;
using System.Linq;
using DistantLearningSystem.Models.DataModels;

namespace DistantLearningSystem.Models.LogicModels.Managers
{
    public class ConceptManager : Manager
    {
        public Concept Add(Concept concept)
        {
            var exists = entities.Concepts.FirstOrDefault(x => x.Name.ToLower()
                == concept.Name.ToLower());

            if (exists != null)
                return null;

            Concept cn = entities.Concepts.Add(concept);
            SaveChanges();
            return cn;
        }

        public Connection AddConnection(int parentConceptId, int childConceptId, int classificationId)
        {
            var parentConcept = GetConcept(parentConceptId);
            var childConcept = GetConcept(childConceptId);
            var classification = entities.Classifications.FirstOrDefault(x => x.Id == classificationId);
            if (parentConcept == null || childConcept == null || classificationId == null)
                return null;

            var connection = entities.Connections.Add(new Connection()
            {
                ClassificationId = classificationId,
                ChildConceptId = childConceptId,
                ParentConceptId = parentConceptId
            });

            SaveChanges();
            return connection;
        }

        public Connection GetConnection(int connectionId)
        {
            return entities.Connections.FirstOrDefault(x => x.Id == connectionId);
        }

        public bool RemoveConnection(int connectionId)
        {
            var connectionToRemove = GetConnection(connectionId);
            if (connectionToRemove == null)
                return false;
            entities.Connections.Remove(connectionToRemove);
            SaveChanges();
            return true;
        }

        public Formulation GetFormulationOfTheDefinition(int formulationId)
        {
            return entities.Formulations.FirstOrDefault(x => x.Id == formulationId); ;
        }

        public bool RemoveFormulationOfDefinition(int formulationId)
        {
            var formulation = GetFormulationOfTheDefinition(formulationId);
            if (formulation == null)
                return false;
            entities.Formulations.Remove(formulation);

            SaveChanges();
            return true;
        }

        public Formulation AddFormulationForDefinition(int definitionId, Formulation formulation)
        {
            formulation.DefinitionId = definitionId;
            Formulation fm = entities.Formulations.Add(formulation);
            SaveChanges();
            return fm;
        }

        public IEnumerable<Concept> GetConcepts()
        {
            return entities.Concepts;
        }

        public Concept GetConcept(int conceptId)
        {
            return entities.Concepts.FirstOrDefault(x => x.Id == conceptId);
        }

        public bool RemoveConcept(int conceptId)
        {
            var conceptTodelete = GetConcept(conceptId);
            if (conceptTodelete == null)
                return false;

            entities.Concepts.Remove(conceptTodelete);
            SaveChanges();
            return true;
        }

        public Definition AddDefinition(int conceptId, Definition definition)
        {
            definition.ConceptId = conceptId;
            var def = entities.Definitions.Add(definition);
            SaveChanges();
            return def;
        }

        public bool RemoveDefinition(int conceptId, int definitionId)
        {
            var defToRemove = entities.Definitions.FirstOrDefault(x => x.Id == definitionId && x.ConceptId == conceptId);
            if (defToRemove == null)
                return false;
            entities.Definitions.Remove(defToRemove);
            SaveChanges();
            return true;
        }

        public IEnumerable<Definition> GetDefinitions(int conceptId)
        {
            var concept = entities.Concepts.FirstOrDefault(x => x.Id == conceptId);
            if (concept != null)
                return concept.Definitions;
            else throw new ArgumentException("Nos such concept in the database.");
        }
    }
}
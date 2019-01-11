using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinServicesObjects;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System.Dynamic;

namespace RMADal
{
    
    public class BasicCRUD
    {

        static MongoDBContext context = new MongoDBContext();

        static async void insertManyDocumentsIntoCollection(List<SPdataObj> ProjectDocuments)
        {
            await context.Projects.InsertManyAsync(ProjectDocuments);
        }

        public async void InsertMany<T>(List<T> documents, Enums.Collection collectionName)
        {
            await context.GetCollection<T>(collectionName).InsertManyAsync(documents);
        }

        public SPdataObj Details(int? id)
        {
            if (id == null)
            {
                return null;
            }

          return (SPdataObj)context.Projects.Find(Builders<SPdataObj>.Filter.Where(p => p.ProjectId == id));

            // od null check
        }

        public async void CreateDocument(SPdataObj Document)
        {
            await context.Projects.InsertOneAsync(Document);

        }

        public async void InsertOne<T>(T document, Enums.Collection collectionName)
        {
            await context.GetCollection<T>(collectionName).InsertOneAsync(document);
        }

        public async Task<List<SPdataObj>> GetAllProjects()
        {            

            return await context.Projects.Find(new BsonDocument()).ToListAsync();            
        }

        public void Update(SPdataObj projectDocument)
        {
            //To do 
            
        }
    }
}

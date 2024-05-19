using Convey.Persistence.MongoDB;
using MiniSpace.Services.Friends.Core.Entities;
using MiniSpace.Services.Friends.Core.Repositories;
using MiniSpace.Services.Friends.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniSpace.Services.Friends.Infrastructure.Mongo.Repositories
{
    public class StudentRequestsMongoRepository : IStudentRequestsRepository
    {
        private readonly IMongoRepository<StudentRequestsDocument, Guid> _repository;

        public StudentRequestsMongoRepository(IMongoRepository<StudentRequestsDocument, Guid> repository)
        {
            _repository = repository;
        }
     
    public async Task<StudentRequests> GetAsync(Guid studentId)
        {
            Console.WriteLine($"{studentId}");
            var document = await _repository.FindAsync(doc => doc.StudentId == studentId);
            var studentRequestDocument = document.SingleOrDefault();
            if (studentRequestDocument == null)
            {
                return null;
            }

            var entity = studentRequestDocument.AsEntity();
            var json = JsonSerializer.Serialize(entity, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(json);

            return entity;
        }


        public async Task<IEnumerable<StudentRequests>> GetAllAsync()
        {
            var documents = await _repository.FindAsync(_ => true);
            return documents.Select(doc => doc.AsEntity()); 
        }

        public async Task AddAsync(StudentRequests studentRequests)
        {
            var document = studentRequests.AsDocument(); 
            await _repository.AddAsync(document);
        }

        public async Task UpdateAsync(StudentRequests studentRequests)
        {
            var document = studentRequests.AsDocument();
            await _repository.UpdateAsync(document);
        }

         public async Task DeleteAsync(Guid studentId)
        {
            var documents = await _repository.FindAsync(doc => doc.StudentId == studentId);
            var document = documents.SingleOrDefault();
            if (document != null)
            {
                await _repository.DeleteAsync(document.Id);
            }
        }
    }
}

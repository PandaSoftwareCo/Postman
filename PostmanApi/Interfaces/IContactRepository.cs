using System.Collections.Generic;
using System.Threading.Tasks;
using PostmanApi.Models;

namespace PostmanApi.Interfaces
{
    public interface IContactRepository : IRepository
    {
        Task<IEnumerable<Contact>> GetContacts();
        Task<Contact> GetContact(long id);
        Task<IEnumerable<Contact>> GetContacts(IEnumerable<long> ids);
        //Task<Contact> AddAsync(Contact contact);
        Contact Add(Contact contact);
        void Update(Contact contact);
        void Delete(Contact contact);
        Task<bool> ContactExistsAsync(long id);
    }
}

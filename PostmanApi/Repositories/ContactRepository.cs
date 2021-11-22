using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostmanApi.Data;
using PostmanApi.Interfaces;
using PostmanApi.Models;

namespace PostmanApi.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly DataContext _context;
        private ILogger<ContactRepository> _logger;

        public IUnitOfWork UnitOfWork => _context;

        public ContactRepository(DataContext context, ILogger<ContactRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Contact>> GetContacts()
        {
            //_context.Contacts.FromSqlRaw<Contact>("SELECT * FROM Contacts");
            //_context.Database.ExecuteSqlRawAsync(@"");
            //_context.Contacts.FromSqlRaw(@"");
            //_context.Database.ExecuteSqlInterpolated(@"SELECT * FROM [Contacts].[Contacts]({})");
            return await _context.Contacts.AsNoTracking().ToListAsync();
        }

        public async Task<Contact> GetContact(long id)
        {
            //return await _context.Contacts.FindAsync(id);
            return await _context.Contacts.AsNoTracking().Where(i => i.ContactId == id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Contact>> GetContacts(IEnumerable<long> ids)
        {
            return await _context.Contacts.AsNoTracking().Where(i => ids.Contains(i.ContactId)).ToListAsync();
        }

        //public async Task<Contact> AddAsync(Contact contact)
        //{
        //    await _context.Contacts.AddAsync(contact);
        //    return contact;
        //}

        public Contact Add(Contact contact)
        {
            return _context.Contacts.Add(contact).Entity;
        }

        public async void Update(Contact contact)
        {
            _context.Entry(contact).State = EntityState.Modified;
        }

        public void Delete(Contact contact)
        {
            _context.Contacts.Remove(contact);
        }

        public async Task<bool> ContactExistsAsync(long id)
        {
            return await _context.Contacts.AnyAsync(e => e.ContactId == id);
        }
    }
}

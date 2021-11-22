using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostmanApi.Data;
using PostmanApi.Interfaces;
using PostmanApi.Models;

namespace PostmanApi.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<ContactsController> _logger;
        private readonly IMapper _mapper;

        public ContactsController(IContactRepository contactRepository, ILogger<ContactsController> logger, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/Contacts
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [ProducesResponseType(typeof(IEnumerable<ContactModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ContactModel>>> GetContacts()
        {
            var contacts = await _contactRepository.GetContacts();
            foreach (var contact in contacts)
            {
                RemoveImage(contact);
            }
            return Ok(_mapper.Map<IEnumerable<ContactModel>>(contacts));
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        public async Task<ActionResult<ContactModel>> GetContact(long id)
        {
            var contact = await _contactRepository.GetContact(id);

            if (contact == null)
            {
                return NotFound();
            }
            RemoveImage(contact);

            return _mapper.Map<ContactModel>(contact);
        }

        // GET: api/Contacts/GetRange
        [HttpPost("[action]")]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContactModel>>> GetRange(IEnumerable<long> ids)
        {
            var contacts = await _contactRepository.GetContacts(ids);
            foreach (var contact in contacts)
            {
                RemoveImage(contact);
            }

            return Ok(_mapper.Map<IEnumerable<ContactModel>>(contacts));
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutContact(long id, ContactModel contactModel)
        {
            if (id != contactModel.ContactId)
            {
                return BadRequest();
            }

            var contact = _mapper.Map<Contact>(contactModel);
            _contactRepository.Update(contact);

            try
            {
                await _contactRepository.UnitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await ContactExistsAsync(id)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PATCH: api/Contacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        public async Task<IActionResult> PatchContact(long id, JsonPatchDocument<ContactModel> requestOp)
        {
            var contactInDb = await _contactRepository.GetContact(id);
            if (contactInDb == null)
            {
                return NotFound();
            }

            var contact = _mapper.Map<ContactModel>(contactInDb);
            requestOp.ApplyTo(contact, ModelState);

            if (!TryValidateModel(contact))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(contact, contactInDb);

            _contactRepository.Update(contactInDb);

            try
            {
                await _contactRepository.UnitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await ContactExistsAsync(id)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Contacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactModel>> PostContact(ContactModel contactModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var contact = _mapper.Map<Contact>(contactModel);
            _contactRepository.Add(contact);
            await _contactRepository.UnitOfWork.SaveChangesAsync();
            contactModel = _mapper.Map<ContactModel>(contact);
            return CreatedAtAction("GetContact", new { id = contact.ContactId }, contact);
        }

        // POST: api/Contacts/AddRange
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("[action]")]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        public async Task<ActionResult<List<Contact>>> AddRange(IEnumerable<Contact> contacts)
        {
            foreach (var contact in contacts)
            {
                _contactRepository.Add(contact);
            }

            await _contactRepository.UnitOfWork.SaveChangesAsync();

            return CreatedAtAction("GetContacts", contacts);
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        public async Task<IActionResult> DeleteContact(long id)
        {
            var contact = await _contactRepository.GetContact(id);
            if (contact == null)
            {
                return NotFound();
            }

            _contactRepository.Delete(contact);
            await _contactRepository.UnitOfWork.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> ContactExistsAsync(long id)
        {
            return await _contactRepository.ContactExistsAsync(id);
        }

        private void RemoveImage(Contact contact)
        {
            contact.Image = null;
            contact.ImageName = null;
            contact.ImageHeight = null;
            contact.ImageWidth = null;
            contact.Size = null;
            contact.ContentType = null;
        }
    }
}

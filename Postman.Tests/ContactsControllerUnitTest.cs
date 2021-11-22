using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PostmanApi.Controllers;
using PostmanApi.Interfaces;
using PostmanApi.Models;
using PostmanApi.Profiles;
using Xunit;

namespace Postman.Tests
{
    public class ContactsControllerUnitTest
    {
        private static IMapper _mapper;
        private static Mock<ILogger<ContactsController>> _loggerMocked;

        static ContactsControllerUnitTest()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new ContactProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }

            _loggerMocked = new Mock<ILogger<ContactsController>>(MockBehavior.Strict);
            //_loggerMocked.Setup(m => m.LogInformation(It.IsAny<string>()))
            //    .Returns(actionResult);
        }

        [Fact]
        public async Task TestGetContacts()
        {
            // ARRANGE
            var repositoryMocked = new Mock<IContactRepository>(MockBehavior.Strict);
            repositoryMocked.Setup(m => m.GetContacts())
                .ReturnsAsync(new List<Contact>
                {
                    new Contact
                    {
                        ContactId = 1,
                        Name = "James Bond",
                        Phone = "604-555-5555",
                        Email = "007@mi6.gov.uk"
                    }
                });

            var contactsController = new ContactsController(repositoryMocked.Object, _loggerMocked.Object, _mapper);

            // ACT
            var contactResult = await contactsController.GetContacts();
            var result = contactResult.Result as OkObjectResult;
            var contacts = result.Value as List<ContactModel>;

            // ASSERT
            Assert.NotNull(contacts);
            Assert.IsType<List<ContactModel>>(result.Value);
            Assert.NotEmpty(contacts);
            repositoryMocked.Verify(i => i.GetContacts(), Times.Once);
        }

        [Fact]
        public async Task TestGetContact()
        {
            // ARRANGE
            var contact = new Contact
            {
                ContactId = 1,
                Name = "James Bond",
                Phone = "604-555-5555",
                Email = "007@mi6.gov.uk"
            };
            var repositoryMocked = new Mock<IContactRepository>(MockBehavior.Strict);
            repositoryMocked.Setup(m => m.GetContact(It.IsAny<long>()))
                .ReturnsAsync(contact);

            var contactsController = new ContactsController(repositoryMocked.Object, _loggerMocked.Object, _mapper);

            // ACT
            var contactResult = await contactsController.GetContact(1L);
            var contactModel = contactResult.Value as ContactModel;

            // ASSERT
            Assert.NotNull(contactModel);
            Assert.IsType<ContactModel>(contactResult.Value);
            repositoryMocked.Verify(i => i.GetContact(It.IsAny<long>()), Times.Once);
        }
    }
}

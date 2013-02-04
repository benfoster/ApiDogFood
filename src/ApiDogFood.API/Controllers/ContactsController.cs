using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiDogFood.API.Controllers
{
    public class ContactsController : ApiController
    {
        private static List<Contact> contacts = new List<Contact>()
        {
            new Contact { Id = 1, FirstName = "John", LastName = "Smith", Email = "john.smith@foobar.com" },
            new Contact { Id = 2, FirstName = "Peter", LastName = "Piper", Email = "peter.piper@foobar.com" },
            new Contact { Id = 3, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@foobar.com" }
        };

        public IEnumerable<Contact> Get()
        {
            return contacts;
        }

        public Contact Get(int id)
        {
            return GetContact(id);
        }

        public HttpResponseMessage Post(AddContactCommand command)
        {
            var newId = contacts.Max(x => x.Id) + 1;
            var contact = new Contact { 
                Id = newId, 
                FirstName = command.FirstName, 
                LastName = command.LastName, 
                Email = command.Email };

            contacts.Add(contact);

            var response = Request.CreateResponse(HttpStatusCode.Created, contact);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { controller = "contacts", id = contact.Id }));

            return response;
        }

        public void Put(int id, UpdateContactCommand command)
        {
            var contact = GetContact(id);
            contact.FirstName = command.FirstName;
            contact.LastName = command.LastName;
        }

        public void Delete(int id)
        {
            var contact = GetContact(id);
            contacts.Remove(contact);
        }

        private Contact GetContact(int id)
        {
            var contact = contacts.SingleOrDefault(c => c.Id == id);

            if (contact == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return contact;
        }
    }

    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class AddContactCommand
    {
        [Required]public string FirstName { get; set; }
        [Required]public string LastName { get; set; }
        [Required]public string Email { get; set; }
    }

    public class UpdateContactCommand
    {
        [Required]public string FirstName { get; set; }
        [Required]public string LastName { get; set; }
    }
}

﻿using CBDesktopUI.Library.DataAccess;
using CBDesktopUI.Library.Models;
using CBDesktopUI.ViewAbstraction;
using System;
using System.Linq;

namespace CBDesktopUI.Presenter
{
    public class ContactPresenter
    {
        private readonly IContactView _view;
        private readonly IPersonData _personData;
        private readonly IComboBoxData _boxData;
        public PersonDetailModel ContactDetails { get; set; }
        public MainPresenter MainPresenter { get; set; }

        public ContactPresenter(IContactView view, IPersonData personData, IComboBoxData boxData)
        {
            _view = view;
            _personData = personData;
            _boxData = boxData;

            ContactDetails = new PersonDetailModel();

            LoadTypes();

            _view.CreateContact += OnCreateContact;
            _view.EditContact += OnEditContact;
            _view.AddPhone += OnAddPhone;
            _view.AddAddress += OnAddAddress;
            _view.PhoneTypeSelected += OnPhoneTypeSelected;
            _view.AddressTypeSelected += OnAddressTypeSelected;
            _view.EditAddress += OnEditAddress;
            _view.EditPhone += OnEditPhone;
            _view.DeleteAddress += OnDeleteAddress;
            _view.DeletePhone += OnDeletePhone;
        }

        private void OnCreateContact(object sender, EventArgs e)
        {
            var contact = GetContactFromView();

            _personData.SaveContact(contact, ContactDetails);

            MainPresenter?.LoadContacts();
        }

        public void OnEditContact(object sender, EventArgs e)
        {
            var editedContact = GetContactFromView();

            _personData.EditContact(editedContact, ContactDetails);

            MainPresenter?.LoadContacts();
        }

        private void OnAddPhone(object sender, EventArgs e)
        {
            ContactDetails.Phones.Add(GetPhoneFromView());

            ClearTextBox();
        }

        private void OnEditPhone(object sender, EventArgs e)
        {
            ContactDetails.Phones = ContactDetails.Phones
              .Where(x => x.Id != _view.PhoneId)
              .ToList();

            ContactDetails.Phones.Add(GetPhoneFromView());

            ClearTextBox();
        }

        private void OnAddAddress(object sender, EventArgs e)
        {
            ContactDetails.Addresses.Add(GetAddressFromView());

            ClearTextBox();
        }

        private void OnEditAddress(object sender, EventArgs e)
        {
            ContactDetails.Addresses = ContactDetails.Addresses
                .Where(x => x.Id != _view.AddressId)
                .ToList();

            ContactDetails.Addresses.Add(GetAddressFromView());

            ClearTextBox();
        }

        private void OnDeletePhone(object sender, EventArgs e)
        {
            _personData.DeletePhone(_view.PhoneId);

            ContactDetails.Phones = ContactDetails.Phones
              .Where(x => x.Id != _view.PhoneId)
              .ToList();

            ClearTextBox();
        }

        private void OnDeleteAddress(object sender, EventArgs e)
        {
            _personData.DeleteAddress(_view.AddressId);

            ContactDetails.Addresses = ContactDetails.Addresses
              .Where(x => x.Id != _view.AddressId)
              .ToList();

            ClearTextBox();
        }

        private void OnPhoneTypeSelected(object sender, EventArgs e)
        {
            var phone = ContactDetails.Phones
                 .FirstOrDefault(x => x.PhoneNumberTypeID == _view.PhoneNumberTypeID) ?? new PhoneDbModel();

            _view.PhoneId = phone.Id;
            _view.PhoneNumber = phone.PhoneNumber;
        }

        private void OnAddressTypeSelected(object sender, EventArgs e)
        {
            var address = ContactDetails.Addresses
                 .FirstOrDefault(x => x.AddressTypeID == _view.AddressTypeID) ?? new AddressDbModel();

            _view.AddressId = address.Id;
            _view.HomeNumber = address.HomeNumber;
            _view.Street = address.Street;
            _view.City = address.City;
            _view.Country = address.Country;
        }

        public void Contact(string contactId, bool isEditMode)
        {
            _view.SetVisibilityButttons(isEditMode);

            if (!string.IsNullOrEmpty(contactId))
            {
                int.TryParse(contactId, out int id);

                var contactData = _personData.GetContactById(id);

                var contact = contactData.Item1;
                ContactDetails.Phones = contactData.Item2;
                ContactDetails.Addresses = contactData.Item3;

                SetContactToView(contact);
            }

            _view.OpenView();
        }

        public void SetContactToView(PersonDbModel contact)
        {
            _view.ContactId = contact.Id;
            _view.FirstName = contact.FirstName;
            _view.LastName = contact.LastName;
            _view.EmailAddress = contact.EmailAddress;
            _view.Description = contact.Description;
        }

        public PersonDbModel GetContactFromView()
        {
            var contact = new PersonDbModel
            {
                Id = _view.ContactId,
                FirstName = _view.FirstName,
                LastName = _view.LastName,
                EmailAddress = _view.EmailAddress,
                Description = _view.Description
            };
            return contact;
        }

        public PhoneDbModel GetPhoneFromView()
        {
            var phone = new PhoneDbModel
            {
                Id = _view.PhoneId,
                PersonID = _view.ContactId,
                PhoneNumberTypeID = _view.PhoneNumberTypeID,
                PhoneNumber = _view.PhoneNumber
            };
            return phone;
        }

        public AddressDbModel GetAddressFromView()
        {
            var address = new AddressDbModel
            {
                Id = _view.AddressId,
                PersonID = _view.ContactId,
                AddressTypeID = _view.AddressTypeID,
                HomeNumber = _view.HomeNumber,
                Street = _view.Street,
                City = _view.City,
                Country = _view.Country
            };
            return address;
        }

        public void LoadTypes()
        {
            var data = _boxData.LoadComboBox();

            _view.PhoneType = data.Item1;
            _view.AddressType = data.Item2;
        }

        public void ClearTextBox()
        {
            _view.PhoneNumber = string.Empty;
            _view.HomeNumber = string.Empty;
            _view.Street = string.Empty;
            _view.City = string.Empty;
            _view.Country = string.Empty;
        }
    }
}

/// <reference path="../_references.js" />
my.ContactsViewModel = function (apiClient) {
    var self = this;
    self.contacts = ko.observableArray();
    self.selectedContact = ko.observable();
    self.newContact = new my.ContactModel();

    self.refresh = function () {
        $.when(apiClient.get("contacts"))
            .then(function (contacts) {
                self.contacts.removeAll();
                self.update(contacts);
            });
    };

    self.addContact = function (form) {
        if (!self.newContact.isValid()) {
            return;
        }

        $.when(apiClient.post("contacts", ko.toJSON(self.newContact)))
            .then(function (result) {
                self.contacts.push(new my.ContactModel(result));
            });

        // reset newContact
        self.newContact.revert();
    };

    
    self.displayMode = function (contact) {
        return contact === self.selectedContact() ? "tmplEdit" : "tmplView";
    }

    self.updateContact = function (contact) {
        self.selectedContact(contact);
    };

    self.saveContact = function (contact) {
        $.when(apiClient.put("contacts/" + contact.id(), ko.toJSON({ firstName: contact.firstName, lastName: contact.lastName })))
            .then(function () {
                self.selectedContact(undefined);
            });
    };

    self.cancelEdit = function (contact) {
        contact.revert();
        self.selectedContact(undefined);
    };

    self.deleteContact = function (contact) {
        $.when(apiClient.delete("contacts/" + contact.id())) // better to use hypermedia links here
            .then(function () {
                self.contacts.remove(contact);
            });
    }
};

ko.utils.extend(my.ContactsViewModel.prototype, {
    update: function (data) {
        data = data || {};
        var contacts = ko.utils.arrayMap(data, function (c) {
            return new my.ContactModel(c);
        });

        this.contacts.push.apply(this.contacts, contacts);
    }
});

my.ContactModel = function (data) {
    var self = this;
    self.id = ko.observable();
    self.firstName = ko.observable();
    self.lastName = ko.observable();
    self.email = ko.observable();
    self.cache = function () { };

    self.update(data);
};

ko.utils.extend(my.ContactModel.prototype, {
    update: function (data) {
        data = data || {};
        this.id(data.id);
        this.firstName(data.firstName);
        this.lastName(data.lastName);
        this.email(data.email);

        this.commit();
    },
    revert: function () {
        this.update(this.cache.data);
    },
    commit: function () {
        this.cache.data = ko.toJS(this);
    },
    isValid: function () {
        return this.firstName() && this.lastName() && this.email();
    }
});
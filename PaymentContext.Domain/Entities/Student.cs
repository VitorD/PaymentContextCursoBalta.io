using System.Collections.Generic;
using System.Linq;
using Flunt.Validations;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Entities;

namespace PaymentContext.Domain.Entities
{
    public class Student : Entity
    {
        private IList<Subscription> _subscriptions;
        public Student(Name name, Document document, Email email)
        {
           Name =name;
            Document = document;
            Email = email;
            _subscriptions = new List<Subscription>();

            AddNotifications(name,document,email);
        }

        public Name Name {get;set;}
        public string FirstName {get; private set;}
        public string LastName {get; private set;}
        public Document Document {get; private set;}
        public Email Email {get; private set;}

         public Address Address {get; private set;}


        public IReadOnlyCollection<Subscription> Subscriptions {get{return _subscriptions.ToArray();}}

        public void AddSubscription(Subscription subscription)
        {
         var hasSusbscriptionActive = false;
         foreach(var sub in _subscriptions){
             if(sub.Active)
                hasSusbscriptionActive = true;
         }
        AddNotifications(new Contract()
        .Requires()
        .IsFalse(hasSusbscriptionActive,"Student,Subscriptions","Voce já tem uma assinatura ativa")
        .AreNotEquals(0,subscription.Payments.Count,"Student.Subscription.Payments","essa assinatura nao possui pagamento")
        );


        }

    }
}
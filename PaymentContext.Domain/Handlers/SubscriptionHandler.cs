using System;
using Flunt.Notifications;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler :
    Notifiable,
    IHandler<CreateBoletoSubscriptionCommand>
    {
        private readonly IStudentRepository _repository;
        public SubscriptionHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        
        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            //Fail Fast Validation
            command.Validate();
            if(command.Invalid){
                AddNotifications(command);
                return new CommandResult(false,"Nao foi possivel realizar sua assinatura");

            }
           //verificar se doc ja ta cadastrado
            if(_repository.DocumentExists(command.Document))
            {
                AddNotification("Document","Este CPF já está em uso");

            }
           //verificar se email ja ta cadastrado
           if(_repository.EmailExists(command.Email))
           {
                AddNotification("Email","Este email já está em uso");
           }
           //gerar VOs
            var name = new Name(command.FirstName,command.LastName);
            var document = new Document(command.Document,EDocumentType.CPF);
            var email = new Email(command.Email);
           var address = new Address(command.Street,command.Number,command.Neighborhood,command.City,command.State,command.Country,command.Zipcode);
           //gerar entidades
           var student = new Student(name,document,email);
           var subscription = new Subscription(DateTime.Now.AddMonths(1));
           var payment = new BoletoPayment(command.Barcode,
           command.BoletoNumber,
           command.PaidDate,
           command.ExpireDate,
           command.Total,
           command.TotalPaid,
           new Document(command.PayerDocument,command.PayerDocumentType),
           command.Payer,
           address,
           email
           );
            //Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);
           //agrupar as validaçoes
           AddNotifications(name,document,email,address,student,subscription,payment);
           //Checar as notificações
           if(Invalid)
            return new CommandResult(false,"Não foi possivel realizar sua assinatura");
           //salvar as informacoes
           _repository.CreateSubscription(student);
           //enviar email de boas vindas(faltou fazer)
           //retornar informacoes
           return new CommandResult(true,"Assinatura realizada com sucesso");
        }
    }
}
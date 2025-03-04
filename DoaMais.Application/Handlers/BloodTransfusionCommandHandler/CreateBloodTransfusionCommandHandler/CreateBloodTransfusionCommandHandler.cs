using DoaMais.Application.Commands.BloodTransfusionCommands.CreateBloodTransfusionCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.MessageBus.Interface;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace DoaMais.Application.Handlers.BloodTransfusionCommandHandler.CreateBloodTransfusionCommandHandler
{
    public class CreateBloodTransfusionCommandHandler(
        IUnitOfWork unitOfWork,
        IMessageBus messageBus,
        IConfiguration configuration
        ) : IRequestHandler<CreateBloodTransfusionCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMessageBus _messageBus = messageBus;
        private readonly IConfiguration _configuration = configuration;

        public async Task<ResultViewModel<Guid>> Handle(CreateBloodTransfusionCommand request, CancellationToken cancellationToken)
        {
            var hospital = await _unitOfWork.Hospital.GetHospitalByCNPJAsync(request.CNPJ);

            if (hospital == null || !hospital.IsActive)
                return ResultViewModel<Guid>.Error($"Hospital with CNPJ {request.CNPJ} not found.");

            var transfusionRequestEventDTO = new BloodTransfusionRequestedEventDTO(
                hospital.Id,
                request.QuantityML,
                request.BloodType,
                request.RHFactor
            );

            var transfusionQueueName = _configuration["RabbitMQ:TransfusionQueueName"] ?? throw new ArgumentNullException("TransfusionQueueName not found.");
            var stockEventExchangeName = _configuration["RabbitMQ:StockEventsExchangeName"] ?? throw new ArgumentNullException("StockEventsExchangeName not found.");
            var transfusionRoutingKey = _configuration["RabbitMQ:TransfusionRoutingKey"] ?? throw new ArgumentNullException("TransfusionRoutingKey not found.");

            await _messageBus.PublishDirectMessageAsync(stockEventExchangeName, transfusionQueueName, transfusionRoutingKey, transfusionRequestEventDTO);

            return ResultViewModel<Guid>.Success(Guid.Empty, "Pedido de transfusão enviado. Aguarde a confirmação do StockService.");
        }
    }
}

using DoaMais.Application.Commands.BloodTransfusionCommands.CreateBloodTransfusionCommand;
using DoaMais.Application.DTOs;
using DoaMais.Application.Models;
using DoaMais.Domain.Interfaces.IUnitOfWork;
using DoaMais.MessageBus.Interface;
using MediatR;
using Microsoft.Extensions.Configuration;
using VaultService.Interface;

namespace DoaMais.Application.Handlers.BloodTransfusionCommandHandler.CreateBloodTransfusionCommandHandler
{
    public class CreateBloodTransfusionCommandHandler(
        IUnitOfWork unitOfWork,
        IMessageBus messageBus,
        IConfiguration configuration,
        IVaultClient vaultClient
        ) : IRequestHandler<CreateBloodTransfusionCommand, ResultViewModel<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMessageBus _messageBus = messageBus;
        private readonly IConfiguration _configuration = configuration;
        private readonly IVaultClient _vaultClient = vaultClient;

        public async Task<ResultViewModel<Guid>> Handle(CreateBloodTransfusionCommand request, CancellationToken cancellationToken)
        {
            var hospital = await _unitOfWork.Hospital.GetHospitalByCNPJAsync(request.CNPJ);

            if (hospital == null || !hospital.IsActive)
                return ResultViewModel<Guid>.Error($"Hospital with CNPJ {request.CNPJ} not found.");

            var transfusionRequestEventDTO = new BloodTransfusionRequestedEventDTO(
                hospital.Id,
                hospital.Name,
                hospital.Email,
                request.QuantityML,
                request.BloodType,
                request.RHFactor
            );

            var transfusionQueueName = _vaultClient.GetSecret(_configuration["KeyVaultSecrets:RabbitMQ:TransfusionQueue"] ?? throw new ArgumentNullException("TransfusionQueue is missing in Vault"));
            var stockEventExchangeName = _vaultClient.GetSecret(_configuration["KeyVaultSecrets:RabbitMQ:StockEventsExchange"] ?? throw new ArgumentNullException("StockEventsExchange is missing in Vault"));
            var transfusionRoutingKey = _vaultClient.GetSecret(_configuration["KeyVaultSecrets:RabbitMQ:TransfusionRoutingKey"] ?? throw new ArgumentNullException("TransfusionRoutingKey is missing in Vault"));

            await _messageBus.PublishDirectMessageAsync(stockEventExchangeName, transfusionQueueName, transfusionRoutingKey, transfusionRequestEventDTO);

            return ResultViewModel<Guid>.Success(Guid.Empty, "Pedido de transfusão enviado. Aguarde a confirmação do StockService.");
        }
    }
}

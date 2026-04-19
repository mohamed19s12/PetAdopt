using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetAdopt.Application.DTOs.Adoption;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Services
{
    public class AdoptionService : IAdoptionService
    {
        private readonly IAdoptionRequestRepository _AdoptionRepo;
        private readonly IPetRepository _PetRepo;
        private readonly INotificationService _NotificationService;
        private readonly ILogger<AdoptionService> _logger;
        private readonly IMapper _mapper;

        public AdoptionService(IAdoptionRequestRepository adoptionRepo, IPetRepository petRepo, INotificationService notificationService, ILogger<AdoptionService> logger, IMapper mapper)
        {
            _AdoptionRepo = adoptionRepo;
            _PetRepo = petRepo;
            _NotificationService = notificationService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task Acceept(int requestId)
        {
            //First we need to get the request by id
            _logger.LogInformation("Accepting adoption request: {RequestId}", requestId);
            var request =await _AdoptionRepo.GetByIdAsync(requestId);

            if (request == null)
            {
                _logger.LogWarning("Adoption request not found: {RequestId}", requestId);
                throw new Exception("Adoption request not found");
            }
            //if the request approved cannot accept again
            if (request.Status == RequestStatus.Approved)
            {
                _logger.LogWarning("Adoption request already approved: {RequestId}", requestId);
                throw new InvalidOperationException("Adoption request is already approved");
            }
            if (request.Status == RequestStatus.Rejected)
            {
                _logger.LogWarning("Cannot accept rejected request: {RequestId}", requestId);
                throw new InvalidOperationException("Cannot accept a rejected request, adopter must apply again");
            }

            //make Rquest Approved
            request.Status = RequestStatus.Approved;
            //then make the pet adopted
            request.Pet.Status = PetStatus.Adopted;
            await _AdoptionRepo.SaveChangesAsync();


            //Notify the adopter about the approval
            _logger.LogInformation("Adoption request accepted: {RequestId} for Pet: {PetName}",
                requestId, request.Pet.Name);
            await _NotificationService.SendNotificationAsync(
                request.AdoprerId, $"Your adoption request for {request.Pet.Name} has been approved!");
        }

        public async Task Apply(string userId, int petId)
        {
            //First we need to get the pet by id
            _logger.LogInformation("User {UserId} applying for pet {PetId}", userId, petId);
            var pet = await _PetRepo.GetByIdAsync(petId);
            if (pet == null)
            {
                _logger.LogWarning("Pet not found: {PetId}", petId);
                throw new KeyNotFoundException("Pet not found");
            }
            //Check if the pet is available for adoption
            if (pet.Status != PetStatus.Approved)
            {
                _logger.LogWarning("Pet is not available for adoption: {PetId}", petId);
                throw new InvalidOperationException("Pet is not available for adoption");
            }

            var request = new AdoptionRequest
            {
                PetId = petId,
                AdoprerId = userId,
            };

            await _AdoptionRepo.AddAsync(request);
            await _AdoptionRepo.SaveChangesAsync();

            _logger.LogInformation("Adoption request created for Pet: {PetName} by User: {UserId}",
            pet.Name, userId);

            await _NotificationService.SendNotificationAsync(
                pet.OwnerId,
                $"Someone wants to adopt your pet {pet.Name}!");

        }

        public async Task Reject(int requestId)
        {
            _logger.LogInformation("Rejecting adoption request: {RequestId}", requestId);
            var request =await _AdoptionRepo.GetByIdAsync(requestId);

            if (request == null)
            {
                _logger.LogWarning("Adoption request not found: {RequestId}", requestId);
                throw new Exception("Adoption request not found");
            }
            if (request.Status == RequestStatus.Approved)
            {
                _logger.LogWarning("Cannot reject approved request: {RequestId}", requestId);
                throw new InvalidOperationException("Cannot reject an already approved request");
            }
            await _AdoptionRepo.DeleteAsync(request);
            await _AdoptionRepo.SaveChangesAsync();

            _logger.LogInformation("Adoption request rejected: {RequestId}", requestId);

            await _NotificationService.SendNotificationAsync(
                request.AdoprerId,
                $"Your adoption request for {request.Pet.Name} has been rejected.");
        }

        public async Task<List<AdoptionRequestDto>> GetMyRequestsAsync(string adopterId, RequestStatus? status = null)
        {
            var requests = await _AdoptionRepo.GetByAdopterIdAsync(adopterId, status);
            return requests.Select(r => _mapper.Map<AdoptionRequestDto>(r)).ToList();
        }

        public async Task<List<AdoptionRequestDto>> GetOwnerRequestsAsync(string ownerId)
        {
            _logger.LogInformation("Getting adoption requests for owner: {OwnerId}", ownerId);
            var requests = await _AdoptionRepo.GetByOwnerIdAsync(ownerId);

            _logger.LogInformation("Found {RequestCount} adoption requests for owner: {OwnerId}", requests.Count, ownerId);
            return requests.Select(r => _mapper.Map<AdoptionRequestDto>(r)).ToList();
        }
    }
}

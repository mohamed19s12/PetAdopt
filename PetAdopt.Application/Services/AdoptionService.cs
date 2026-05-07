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
            var pet = request.Pet;

            //if the request has ( approved Or rejected )cannot accept again
            if (request.RequestStatus == RequestStatus.Approved)
            {
                _logger.LogWarning("Adoption request already approved: {RequestId}", requestId);
                throw new InvalidOperationException("Adoption request is already approved");
            }
            if (request.RequestStatus == RequestStatus.Rejected)
            {
                _logger.LogWarning("Cannot accept rejected request: {RequestId}", requestId);
                throw new InvalidOperationException("Cannot accept a rejected request, adopter must apply again");
            }

            //if pet is adopted allready
            if (request.Pet.petStatusForAdoption == PetStatusForAdoption.Adopted)
            {
                _logger.LogWarning("Pet is already adopted: {RequestId}", requestId);
                throw new InvalidOperationException("Pet is already adopted");
            }

            //make Rquest Approved
            request.RequestStatus = RequestStatus.Approved;

            //then make the pet adopted
            pet.petStatusForAdoption = PetStatusForAdoption.Adopted;
            pet.requestStatus = RequestStatus.Approved;

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
            if (pet.petStatusForAdoption != PetStatusForAdoption.Available)
            {
                _logger.LogWarning("Pet is not available for adoption: {PetId}", petId);
                throw new InvalidOperationException("Pet is not available for adoption");
            }

            //Check if the user has already applied for this pet ..........pervent duplicates
            var existingRequest = await _AdoptionRepo.GetActiveRequest(userId, petId);

            if (existingRequest != null)
            {
                _logger.LogWarning("User {UserId} already applied for pet {PetId}", userId, petId);
                throw new InvalidOperationException("You already applied for this pet");
            }

            var request = new AdoptionRequest
            {
                PetId = petId,
                AdoprerId = userId,
                RequestStatus = RequestStatus.Pending
            };
            //update pet status
            pet.petStatusForAdoption = PetStatusForAdoption.Requested;
            pet.requestStatus = RequestStatus.Pending;


            await _AdoptionRepo.AddAsync(request);

            await _AdoptionRepo.SaveChangesAsync();

            //test
            _logger.LogInformation(
    "PET STATUS: {status}",
    pet.petStatusForAdoption);

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

            // Cannot reject approved request
            if (request.RequestStatus == RequestStatus.Approved)
            {
                throw new InvalidOperationException("Cannot reject approved request");
            }
            // Already rejected
            if (request.RequestStatus == RequestStatus.Rejected)
            {
                throw new InvalidOperationException("Request already rejected");
            }

            // Reject request
            request.RequestStatus = RequestStatus.Rejected;

            // Return pet to available
            request.Pet.petStatusForAdoption = PetStatusForAdoption.Available;
            request.Pet.requestStatus = RequestStatus.Pending;

            await _AdoptionRepo.SaveChangesAsync();

            _logger.LogInformation("Adoption request rejected: {RequestId}", requestId);

            await _NotificationService.SendNotificationAsync(
                request.AdoprerId,
                $"Your adoption request for {request.Pet.Name} has been rejected.");
        }

        //Getting adopter requests
        public async Task<List<AdoptionRequestDto>> GetMyRequestsAsync(string adopterId, RequestStatus? status = null)
        {
            var requests = await _AdoptionRepo.GetByAdopterIdAsync(adopterId, status);
            return requests.Select(r => _mapper.Map<AdoptionRequestDto>(r)).ToList();
        }

        //Getting owner requests
        public async Task<List<AdoptionRequestDto>> GetOwnerRequestsAsync(string ownerId)
        {
            _logger.LogInformation("Getting adoption requests for owner: {OwnerId}", ownerId);
            var requests = await _AdoptionRepo.GetByOwnerIdAsync(ownerId);

            _logger.LogInformation("Found {RequestCount} adoption requests for owner: {OwnerId}", requests.Count, ownerId);
            return requests.Select(r => _mapper.Map<AdoptionRequestDto>(r)).ToList();
        }

        public async Task<List<AdoptionRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _AdoptionRepo.GetAllRequestsAsync();
            return requests.Select(req => _mapper.Map<AdoptionRequestDto>(req)).ToList();
        }
    }
}

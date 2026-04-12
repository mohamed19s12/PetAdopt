using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        public AdoptionService(IAdoptionRequestRepository adoptionRepo, IPetRepository petRepo, INotificationService notificationService)
        {
            _AdoptionRepo = adoptionRepo;
            _PetRepo = petRepo;
            _NotificationService = notificationService;
        }

        public async Task Acceept(int requestId)
        {
            //First we need to get the request by id
            var request =await _AdoptionRepo.GetByIdAsync(requestId);

            if (request == null)
                throw new Exception("Adoption request not found");

            //if the request approved cannot accept again
            if (request.Status == RequestStatus.Approved)
                throw new InvalidOperationException("Adoption request is already approved");

            if (request.Status == RequestStatus.Rejected)
                throw new InvalidOperationException("Cannot accept a rejected request, adopter must apply again");


            //make Rquest Approved
            request.Status = RequestStatus.Approved;
            //then make the pet adopted
            request.Pet.Status = PetStatus.Adopted;
            await _AdoptionRepo.SaveChangesAsync();


            //Notify the adopter about the approval
            await _NotificationService.SendNotificationAsync(
                request.AdoprerId, $"Your adoption request for {request.Pet.Name} has been approved!");
        }

        public async Task Apply(string userId, int petId)
        {
            //First we need to get the pet by id
            var pet = await _PetRepo.GetByIdAsync(petId);
            if (pet == null)
                throw new KeyNotFoundException("Pet not found");

            //Check if the pet is available for adoption
            if (pet.Status != PetStatus.Approved)
                throw new InvalidOperationException("Pet is not available for adoption");



            var request = new AdoptionRequest
            {
                PetId = petId,
                AdoprerId = userId,
            };

            await _AdoptionRepo.AddAsync(request);
            await _AdoptionRepo.SaveChangesAsync();

            await _NotificationService.SendNotificationAsync(
                pet.OwnerId,
                $"Someone wants to adopt your pet {pet.Name}!");

        }

        public async Task Reject(int requestId)
        {
            var request =await _AdoptionRepo.GetByIdAsync(requestId);

            if (request == null)
                throw new Exception("Adoption request not found");

            if (request.Status == RequestStatus.Approved)
                throw new InvalidOperationException("Cannot reject an already approved request");

            await _AdoptionRepo.DeleteAsync(request);
            await _AdoptionRepo.SaveChangesAsync();

            await _NotificationService.SendNotificationAsync(
                request.AdoprerId,
                $"Your adoption request for {request.Pet.Name} has been rejected.");
        }

        public async Task<List<AdoptionRequestDto>> GetMyRequestsAsync(string adopterId, RequestStatus? status = null)
        {
            var requests = await _AdoptionRepo.GetByAdopterIdAsync(adopterId, status);
            return requests.Select(r => new AdoptionRequestDto
            {
                Id = r.Id,
                PetId = r.PetId,
                PetName = r.Pet.Name,
                Status = r.Status.ToString(),
                RequestedAt = r.RequestedAt
            }).ToList();
        }

        public async Task<List<AdoptionRequestDto>> GetOwnerRequestsAsync(string ownerId)
        {
            var requests = await _AdoptionRepo.GetByOwnerIdAsync(ownerId);
            return requests.Select(r => new AdoptionRequestDto
            {
                Id = r.Id,
                PetId = r.PetId,
                PetName = r.Pet.Name,
                Status = r.Status.ToString(),
                RequestedAt = r.RequestedAt
            }).ToList();
        }
    }
}

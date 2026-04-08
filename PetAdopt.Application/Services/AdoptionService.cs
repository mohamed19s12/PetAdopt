using Microsoft.AspNetCore.SignalR;
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

            //make Rquest Approved
            request.Status = RequestStatus.Approved;

            //then make the pet adopted
            request.Pet.Status = PetStatus.Adopted;


            await _AdoptionRepo.SaveChangesAsync();


            //Notify the adopter about the approval
            await _NotificationService
                .SendNotificationAsync(
                request.AdoprerId, $"Your adoption request for {request.Pet.Name} has been approved!");
        }

        public async Task Apply(string userId, int petId)
        {
            var request = new AdoptionRequest
            {
                PetId = petId,
                AdoprerId = userId,
            };

            await _AdoptionRepo.AddAsync(request);
            await _AdoptionRepo.SaveChangesAsync();

        }

        public async Task Reject(int requestId)
        {
            var request =await _AdoptionRepo.GetByIdAsync(requestId);

            if (request == null)
                throw new Exception("Adoption request not found");

            request.Status = RequestStatus.Rejected;

            await _AdoptionRepo.SaveChangesAsync();
        }
    }
}

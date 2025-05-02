using Domain.Models.SupportMessageHub;
using Domain.Models;
using Domain.ViewModels.SupportViewModels;
using Infrastructure.Repository;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;

namespace Application.Services.SupportServices
{
    public class CustomerSupportService : ICustomerSupportService
    {
        private readonly IRepository<SupportMessage> _supportMessageRepository;
        private readonly IRepository<User> _userRepository;
        private readonly INotificationService _notificationService;
        public CustomerSupportService(
          IRepository<SupportMessage> supportMessageRepository,
          IRepository<User> userRepository,
          INotificationService notificationService)
        {
            _supportMessageRepository = supportMessageRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<SupportMessageViewModel> SaveSupportMessage(SupportMessageCreateViewModel messageViewModel)
        {
            var user = await _userRepository.GetById(messageViewModel.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var supportMessage = new SupportMessage
            {
                Id = Guid.NewGuid(),
                UserId = messageViewModel.UserId,
                Message = messageViewModel.Message,
                CreatedDate = DateTime.UtcNow,
                IsResolved = false
            };

            await _supportMessageRepository.Add(supportMessage);
            await _supportMessageRepository.SaveChangesMethod();

            await _notificationService.NotifyAdminsAsync(new
            {
                messageId = supportMessage.Id,
                userId = user.Id,
                username = user.FullName,
                userType = user.UserType?.Name,
                message = supportMessage.Message,
                timestamp = supportMessage.CreatedDate
            });

            return new SupportMessageViewModel
            {
                Id = supportMessage.Id,
                UserId = supportMessage.UserId,
                UserName = user.FullName,
                Message = supportMessage.Message,
                CreatedDate = supportMessage.CreatedDate,
                IsResolved = supportMessage.IsResolved
            };
        }

        public async Task<IEnumerable<SupportMessageViewModel>> GetUserSupportHistory(Guid userId)
        {
            var messages = await _supportMessageRepository.FindAll(m => m.UserId == userId);
            var user = await _userRepository.GetById(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return messages.Select(m => new SupportMessageViewModel
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = user.FullName,
                Message = m.Message,
                CreatedDate = m.CreatedDate,
                IsResolved = m.IsResolved,
                AdminResponse = m.AdminResponse,
                ResponseDate = m.ResponseDate
            });
        }

        public async Task<IEnumerable<SupportMessageViewModel>> GetAllPendingSupportMessages()
        {
            var pendingMessages = await _supportMessageRepository.FindAll(m => !m.IsResolved);
            var result = new List<SupportMessageViewModel>();

            foreach (var message in pendingMessages)
            {
                var user = await _userRepository.GetById(message.UserId);
                if (user != null)
                {
                    result.Add(new SupportMessageViewModel
                    {
                        Id = message.Id,
                        UserId = message.UserId,
                        UserName = user.FullName,
                        Message = message.Message,
                        CreatedDate = message.CreatedDate,
                        IsResolved = message.IsResolved,
                        AdminResponse = message.AdminResponse,
                        ResponseDate = message.ResponseDate
                    });
                }
            }

            return result;
        }

        public async Task<SupportMessageViewModel> RespondToSupportMessage(SupportResponseViewModel responseViewModel)
        {
            var supportMessage = await _supportMessageRepository.GetById(responseViewModel.MessageId);
            if (supportMessage == null)
            {
                throw new KeyNotFoundException("Support message not found");
            }

            supportMessage.AdminResponse = responseViewModel.Response;
            supportMessage.ResponseDate = DateTime.UtcNow;
            supportMessage.IsResolved = true;
            supportMessage.ModifiedDate = DateTime.UtcNow;

            await _supportMessageRepository.Update(supportMessage);
            await _supportMessageRepository.SaveChangesMethod();

            await _notificationService.NotifyUserAsync(supportMessage.UserId.ToString(), new
            {
                messageId = supportMessage.Id,
                originalMessage = supportMessage.Message,
                adminResponse = supportMessage.AdminResponse,
                responseDate = supportMessage.ResponseDate
            });

            var user = await _userRepository.GetById(supportMessage.UserId);

            return new SupportMessageViewModel
            {
                Id = supportMessage.Id,
                UserId = supportMessage.UserId,
                UserName = user?.FullName,
                Message = supportMessage.Message,
                CreatedDate = supportMessage.CreatedDate,
                IsResolved = supportMessage.IsResolved,
                AdminResponse = supportMessage.AdminResponse,
                ResponseDate = supportMessage.ResponseDate
            };
        }
    }
}

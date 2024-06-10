using InterviewManagement.Dtos;
using InterviewManagement.Models;

namespace InterviewManagement.Services
{
    public interface OfferService
    {
        ICollection<OfferDto> convertToDtoList(ICollection<Offer> offers);
    }
}

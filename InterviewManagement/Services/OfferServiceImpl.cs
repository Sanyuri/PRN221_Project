using InterviewManagement.Dtos;
using InterviewManagement.Models;
using InterviewManagement.Values;

namespace InterviewManagement.Services
{
    public class OfferServiceImpl : OfferService
    {
        public ICollection<OfferDto> convertToDtoList(ICollection<Offer> offers)
        {
            List<OfferDto> offerDtos = new List<OfferDto>();
            foreach (var offer in offers)
            {
                OfferDto offerDto = new OfferDto()
                {CandidateName = offer.Candidate.FullName,
                Email = offer.Candidate.Email,
                Approver = offer.Employees.FirstOrDefault(e => e.Role.RoleName.Equals(RolesValue.MANAGER)).FullName,
                Department = offer.Department.DepertmentName,
                Note = offer.Note,
                Status = offer.Status};
                offerDtos.Add(offerDto);
            }

            return offerDtos;
        }
    }
}

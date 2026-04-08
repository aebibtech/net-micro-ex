using System;
using Wpm.Clinic.Controllers;
using Wpm.Clinic.DataAccess;

namespace Wpm.Clinic.Application;

public class ClinicApplicationService(ClinicDbContext dbContext, ExternalServices.ManagementService managementService)
{
	public async Task<Consultation> Handle(StartConsultationCommand command)
	{
		var petInfo = await managementService.GetPetInfo(command.PatientId);
		var newConsultation = new Consultation(Guid.NewGuid(),
                                         command.PatientId,
                                         petInfo.Name,
                                         petInfo.Age,
                                         DateTime.UtcNow);

		await dbContext.Consultations.AddAsync(newConsultation);
		await dbContext.SaveChangesAsync();

		return newConsultation;
	}
}


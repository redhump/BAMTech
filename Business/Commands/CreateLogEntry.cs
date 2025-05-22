using System;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
	public class CreateLogEntry : IRequest<CreateLogEntryResult>
	{
        public required string LogMessage { get; set; }

        public DateTime LogTimeStamp { get; set; }

        public required Data.LogLevel LogLevel { get; set; }

    }

    public class CreateLogEntryHandler : IRequestHandler<CreateLogEntry, CreateLogEntryResult>
    {
        private readonly StargateContext _context;

        public CreateLogEntryHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<CreateLogEntryResult> Handle(CreateLogEntry request, CancellationToken cancellationToken)
        {
            var newLogEntry= new LogEntry()
            {
                LogMessage = request.LogMessage,
                LogTimeStamp = request.LogTimeStamp.Date,
                LogLevel = request.LogLevel
            };

            await _context.LogEntries.AddAsync(newLogEntry);

            await _context.SaveChangesAsync();

            return new CreateLogEntryResult()
            {
                Id = newLogEntry.Id
            };
        }

    }

    public class CreateLogEntryResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}


using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

public class SubmitToWipoBackgroundJob(
    WipoSubmissionProcessor wipoSubmissionProcessor,
    ILogger<SubmitToWipoBackgroundJob> logger
) : AsyncBackgroundJob<SubmitToWipoBackgroundJobArgs>, ITransientDependency
{
    private readonly WipoSubmissionProcessor _wipoSubmissionProcessor = wipoSubmissionProcessor;
    private readonly ILogger<SubmitToWipoBackgroundJob> _logger = logger;

    [UnitOfWork]
    public override async Task ExecuteAsync(SubmitToWipoBackgroundJobArgs args)
    {
        try
        {
            await _wipoSubmissionProcessor.ProcessAsync(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Queued WIPO submission failed. SourceReferenceId={SourceReferenceId}",
                args.SourceReferenceId
            );
            throw;
        }
    }
}

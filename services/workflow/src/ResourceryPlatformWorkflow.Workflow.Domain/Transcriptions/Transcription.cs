using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.Transcriptions;

public class Transcription : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsPublic { get; private set; }
    public DateTime DateOfTranscription { get; private set; }
    public DateTime? EventDate { get; private set; }
    public string MediaFile { get; private set; }
    public string Language { get; private set; }
    public string InputeFormat { get; private set; }
    public string Status { get; private set; }
    public InputSource InputSource { get; private set; }
    public string ThumbNailImage { get; private set; }
    public string SourceReferenceId { get; private set; }
    public string LinkJson { get; private set; }
    public string LinkSrt { get; private set; }
    public string LinkHtml { get; private set; }
    public string LinkTxt { get; private set; }
    public string LinkDocx { get; private set; }
    public string LinkVerbatimDocx { get; private set; }

    protected Transcription()
    {
        Title = string.Empty;
        Description = string.Empty;
        MediaFile = string.Empty;
        Language = string.Empty;
        InputeFormat = string.Empty;
        Status = string.Empty;
        ThumbNailImage = string.Empty;
        SourceReferenceId = string.Empty;
        LinkJson = string.Empty;
        LinkSrt = string.Empty;
        LinkHtml = string.Empty;
        LinkTxt = string.Empty;
        LinkDocx = string.Empty;
        LinkVerbatimDocx = string.Empty;
    }

    public Transcription(
        Guid id,
        string title,
        string description,
        bool isPublic,
        DateTime dateOfTranscription,
        DateTime? eventDate,
        string mediaFile,
        string language,
        string inputeFormat,
        string status,
        InputSource inputSource,
        string thumbNailImage
    ) : base(id)
    {
        SetTitle(title);
        SetDescription(description);
        SetIsPublic(isPublic);
        SetDateOfTranscription(dateOfTranscription);
        SetEventDate(eventDate);
        SetMediaFile(mediaFile);
        SetLanguage(language);
        SetInputeFormat(inputeFormat);
        SetStatus(status);
        SetInputSource(inputSource);
        SetThumbNailImage(thumbNailImage);
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title)).Trim();
    }

    public void SetDescription(string description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();
    }

    public void SetIsPublic(bool isPublic)
    {
        IsPublic = isPublic;
    }

    public void SetDateOfTranscription(DateTime dateOfTranscription)
    {
        DateOfTranscription = dateOfTranscription;
    }

    public void SetEventDate(DateTime? eventDate)
    {
        EventDate = eventDate;
    }

    public void SetMediaFile(string mediaFile)
    {
        MediaFile = string.IsNullOrWhiteSpace(mediaFile) ? string.Empty : mediaFile.Trim();
    }

    public void SetLanguage(string language)
    {
        Language = Check.NotNullOrWhiteSpace(language, nameof(language)).Trim();
    }

    public void SetInputeFormat(string inputeFormat)
    {
        InputeFormat = Check.NotNullOrWhiteSpace(inputeFormat, nameof(inputeFormat)).Trim();
    }

    public void SetStatus(string status)
    {
        Status = Check.NotNullOrWhiteSpace(status, nameof(status)).Trim();
    }

    public void SetInputSource(InputSource inputSource)
    {
        InputSource = inputSource;
    }

    public void SetThumbNailImage(string thumbNailImage)
    {
        ThumbNailImage = string.IsNullOrWhiteSpace(thumbNailImage) ? string.Empty : thumbNailImage.Trim();
    }

    public void SetSourceReferenceId(string sourceReferenceId)
    {
        SourceReferenceId = string.IsNullOrWhiteSpace(sourceReferenceId) ? string.Empty : sourceReferenceId.Trim();
    }

    public void SetLinkJson(string linkJson)
    {
        LinkJson = string.IsNullOrWhiteSpace(linkJson) ? string.Empty : linkJson.Trim();
    }

    public void SetLinkSrt(string linkSrt)
    {
        LinkSrt = string.IsNullOrWhiteSpace(linkSrt) ? string.Empty : linkSrt.Trim();
    }

    public void SetLinkHtml(string linkHtml)
    {
        LinkHtml = string.IsNullOrWhiteSpace(linkHtml) ? string.Empty : linkHtml.Trim();
    }

    public void SetLinkTxt(string linkTxt)
    {
        LinkTxt = string.IsNullOrWhiteSpace(linkTxt) ? string.Empty : linkTxt.Trim();
    }

    public void SetLinkDocx(string linkDocx)
    {
        LinkDocx = string.IsNullOrWhiteSpace(linkDocx) ? string.Empty : linkDocx.Trim();
    }

    public void SetLinkVerbatimDocx(string linkVerbatimDocx)
    {
        LinkVerbatimDocx = string.IsNullOrWhiteSpace(linkVerbatimDocx) ? string.Empty : linkVerbatimDocx.Trim();
    }
}

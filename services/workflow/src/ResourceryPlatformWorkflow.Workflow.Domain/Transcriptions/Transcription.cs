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
    public bool PublishedToWebCast { get; private set; }
    public DateTime DateOfTranscription { get; private set; }
    public DateTime? EventDate { get; private set; }
    public string MediaFile { get; private set; }
    public string Language { get; private set; }
    public string InputeFormat { get; private set; }
    public string Status { get; private set; }
    public InputSource InputSource { get; private set; }
    public string SourceReferenceId { get; private set; }
    public byte[] DocumentData { get; private set; }
    public string DocumentExtension { get; private set; }
    public string Transcript { get; private set; }
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
        SourceReferenceId = string.Empty;
        DocumentData = Array.Empty<byte>();
        DocumentExtension = string.Empty;
        Transcript = string.Empty;
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
        bool publishedToWebCast,
        DateTime dateOfTranscription,
        DateTime? eventDate,
        string mediaFile,
        string language,
        string inputeFormat,
        string status,
        InputSource inputSource
    ) : base(id)
    {
        SetTitle(title);
        SetDescription(description);
        SetIsPublic(isPublic);
        SetPublishedToWebCast(publishedToWebCast);
        SetDateOfTranscription(dateOfTranscription);
        SetEventDate(eventDate);
        SetMediaFile(mediaFile);
        SetLanguage(language);
        SetInputeFormat(inputeFormat);
        SetStatus(status);
        SetInputSource(inputSource);
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

    public void SetPublishedToWebCast(bool publishedToWebCast)
    {
        PublishedToWebCast = publishedToWebCast;
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

    public void SetSourceReferenceId(string sourceReferenceId)
    {
        SourceReferenceId = string.IsNullOrWhiteSpace(sourceReferenceId) ? string.Empty : sourceReferenceId.Trim();
    }

    public void SetDocumentData(byte[] documentData)
    {
        DocumentData = documentData ?? Array.Empty<byte>();
    }

    public void SetDocumentExtension(string documentExtension)
    {
        DocumentExtension = string.IsNullOrWhiteSpace(documentExtension) ? string.Empty : documentExtension.Trim().ToLowerInvariant();
    }

    public void SetTranscript(string transcript)
    {
        Transcript = string.IsNullOrWhiteSpace(transcript) ? string.Empty : transcript;
    }

    public void SetLinkJson(string linkJson)
    {
        LinkJson = string.IsNullOrWhiteSpace(linkJson) ? string.Empty : linkJson;
    }

    public void SetLinkSrt(string linkSrt)
    {
        LinkSrt = string.IsNullOrWhiteSpace(linkSrt) ? string.Empty : linkSrt;
    }

    public void SetLinkHtml(string linkHtml)
    {
        LinkHtml = string.IsNullOrWhiteSpace(linkHtml) ? string.Empty : linkHtml;
    }

    public void SetLinkTxt(string linkTxt)
    {
        LinkTxt = string.IsNullOrWhiteSpace(linkTxt) ? string.Empty : linkTxt;
    }

    public void SetLinkDocx(string linkDocx)
    {
        LinkDocx = string.IsNullOrWhiteSpace(linkDocx) ? string.Empty : linkDocx;
    }

    public void SetLinkVerbatimDocx(string linkVerbatimDocx)
    {
        LinkVerbatimDocx = string.IsNullOrWhiteSpace(linkVerbatimDocx) ? string.Empty : linkVerbatimDocx;
    }
}

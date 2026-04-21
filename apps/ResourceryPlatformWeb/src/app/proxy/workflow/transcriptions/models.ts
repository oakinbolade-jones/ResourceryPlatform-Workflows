import type { FullAuditedEntityDto } from '@abp/ng.core';
import type { InputSource } from './input-source.enum';

export interface CreateUpdateTranscriptionDto {
  title?: string;
  description?: string;
  isPublic: boolean;
  dateOfTranscription?: string;
  eventDate?: string;
  mediaFile?: string;
  language?: string;
  inputeFormat?: string;
  status?: string;
  inputSource: InputSource;
  thumbNailImage?: string;
  sourceReferenceId?: string;
  linkJson?: string;
  linkSrt?: string;
  linkHtml?: string;
  linkToVideo?: string;
  linkTxt?: string;
  linkDocx?: string;
  linkVerbatimDocx?: string;
}

export interface TranscriptionDto extends FullAuditedEntityDto<string> {
  title?: string;
  description?: string;
  isPublic: boolean;
  dateOfTranscription?: string;
  eventDate?: string;
  mediaFile?: string;
  language?: string;
  inputeFormat?: string;
  status?: string;
  inputSource: InputSource;
  thumbNailImage?: string;
  sourceReferenceId?: string;
  linkJson?: string;
  linkSrt?: string;
  linkHtml?: string;
  linkToVideo?: string;
  linkTxt?: string;
  linkDocx?: string;
  linkVerbatimDocx?: string;
}
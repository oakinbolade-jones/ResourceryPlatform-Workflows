import { mapEnumToOptions } from '@abp/ng.core';

export enum RequestType {
  InternalMemorandum = 1,
  IncommingCorrespondence = 2,
  ContributionToReport = 3,
  ConceptNoteTermOfReference = 4,
  Meeting = 5,
  Interpretion = 6,
  Translation = 7,
  GroundTransportation = 8,
  VisaProcurement = 9,
  AccreditationOfHeadsOfMission = 10,
  DiplomaticCocktail = 11,
  ReceptionLodgingOfNewStaff = 12,
  CustomsDutiesWaivers = 13,
  ElectionObservationMission = 14,
}

export const requestTypeOptions = mapEnumToOptions(RequestType);

import { mapEnumToOptions } from '@abp/ng.core';

export enum RequestStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
  Cancelled = 3,
  Completed = 4,
}

export const requestStatusOptions = mapEnumToOptions(RequestStatus);

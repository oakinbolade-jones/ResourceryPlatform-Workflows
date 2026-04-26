using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace ResourceryPlatformWorkflow.Workflow.Meetings
{
    public class MeetingItem : Entity<Guid>, IMultiTenant
{
    
          public Guid? TenantId { get; private set; }
 public Guid MeetingId { get; set; }
        public virtual Meeting Meeting { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
         public string Category { get; set; }
        public string ServiceCenterCode { get; set; }

        public int QuantityNo { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public decimal Budget { get; set; }
        public string RemarkObservation { get; set; }
      

        protected MeetingItem()
        {
        }

        public MeetingItem(
            Guid id,
            Guid meetingId,          
            string itemName,
            string itemCode,
            string category,
            string serviceCenterCode,
            int quantityNo,
            DateTime periodFrom,
            DateTime periodTo,
            decimal budget,
            string remarkObservation
        ) : base(id)
        {
            MeetingId = meetingId;
            ItemName = itemName;
            ItemCode = itemCode;
            Category = category;
            ServiceCenterCode = serviceCenterCode;
            QuantityNo = quantityNo;
            PeriodFrom = periodFrom;
            PeriodTo = periodTo;
            Budget = budget;
            RemarkObservation = remarkObservation;
        }
    }
}
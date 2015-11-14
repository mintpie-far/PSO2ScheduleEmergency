using System;

namespace Pso2.Schedule.Emergency
{
	public class EmergencyDataFormat : ICloneable
	{
        /// <summary>
        /// �C�x���g�J�n����
        /// </summary>
		public DateTime StartTime {
            get;
            set;
        }
        /// <summary>
        /// �C�x���g�I������
        /// </summary>
		public DateTime EndTime {
            get;
            set;
        }
        /// <summary>
        /// �C�x���g���e
        /// </summary>
		public string EventInformation {
            get;
            set;
        }
        /// <summary>
        /// �C�x���g���
        /// </summary>
		public EmergencyEventType EventType {
            get;
            set;
        }

		public void SetDay(DateTime Date)
		{
			this.StartTime = new DateTime(Date.Year, Date.Month, Date.Day, this.StartTime.Hour, this.StartTime.Minute, this.StartTime.Second);
			this.EndTime = new DateTime(Date.Year, Date.Month, Date.Day, this.StartTime.Hour, this.StartTime.Minute, this.StartTime.Second);
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}

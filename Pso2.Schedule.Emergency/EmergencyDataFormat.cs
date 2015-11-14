using System;

namespace Pso2.Schedule.Emergency
{
	public class EmergencyDataFormat : ICloneable
	{
        /// <summary>
        /// イベント開始時間
        /// </summary>
		public DateTime StartTime {
            get;
            set;
        }
        /// <summary>
        /// イベント終了時間
        /// </summary>
		public DateTime EndTime {
            get;
            set;
        }
        /// <summary>
        /// イベント内容
        /// </summary>
		public string EventInformation {
            get;
            set;
        }
        /// <summary>
        /// イベント種類
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

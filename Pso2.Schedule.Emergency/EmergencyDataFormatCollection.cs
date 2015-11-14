using System;
using System.Collections;
using System.Collections.Generic;

namespace Pso2.Schedule.Emergency
{
    public class EmergencyDataFormatCollection :BaseMessage, IEnumerable<EmergencyDataFormat>, IEnumerable {
        internal List<EmergencyDataFormat> emerygencyDataList {
            get;
            set;
        }

        public EmergencyDataFormatCollection() {
            this.emerygencyDataList = new List<EmergencyDataFormat>();
        }

        public IEnumerator<EmergencyDataFormat> GetEnumerator() {
            return (this.emerygencyDataList == null) ? new List<EmergencyDataFormat>().GetEnumerator() : this.emerygencyDataList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        internal void Add(EmergencyDataFormat data) {
            this.emerygencyDataList.Add(data);
        }
    }
}

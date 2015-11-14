using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2.Schedule.Emergency {
    public enum ResponceStatus {
        OK,
        NetworkError,
        FormatChanged,
        UnknownError
    }
    public class BaseMessage : IError {
        internal Exception _exception { get; set; }

        internal string _errorMessage { get; set; }

        public ResponceStatus Status { get; set; }
        public dynamic GetData { get; set; }
        public BaseMessage() {
            this.Status = 0;
        }
        public string GetErrorMessage() {
            return _errorMessage;
        }
        public int GetErrorNumber() {
            throw new NotImplementedException();
        }

        public Exception GetExeception() {
            return _exception;
        }
        public void SetResponceData(dynamic MainData, ResponceStatus Status = ResponceStatus.OK, Exception ex = null, string ErrorMessage = null) {
            this.GetData = MainData;
            this.Status = Status;
            this._exception = ex;
            this._errorMessage = ErrorMessage;
        }
        public void SetResponceData(BaseMessage Data) {
            this.SetResponceData(Data.GetData, Data.Status, Data._exception, Data._errorMessage);
        }
    }
}

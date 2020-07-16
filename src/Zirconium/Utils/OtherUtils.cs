using System;
using System.Collections.Generic;
using Zirconium.Core.Models;

namespace Zirconium.Utils
{
    public static class OtherUtils
    {
        public static BaseMessage GenerateProtocolError(BaseMessage parentMessage, string errCode, string errText, IDictionary<string, object> errPayload)
        {
            ProtocolError err = new ProtocolError();
            err.ErrCode = errCode;
            err.ErrText = errText;
            err.ErrPayload = errPayload;

            BaseMessage msg = new BaseMessage(parentMessage, true);
            if (parentMessage == null) {
                msg.ID = Guid.NewGuid().ToString();
            }
            msg.Ok = false;
            msg.Payload = err.ToDictionary();
            return msg;
        }
    }
}
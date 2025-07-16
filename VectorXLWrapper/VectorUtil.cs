using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vxlapi_NET;

namespace HardwareDriverLayer.Wrapper
{
    internal class VectorUtil
    {

        /// <summary>
        /// Gets the actual CAN-FD DLC based on the payload 
        /// </summary>
        /// <param name="newpayload"></param>
        /// <returns></returns>
        internal static XLDefine.XL_CANFD_DLC GET_XL_CANFD_DLC(uint payload)
        {
            XLDefine.XL_CANFD_DLC xl_canfd_dlc;
            switch (payload)
            {
                case 0: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_0_BYTES; break; //DLC = 0
                case 1: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_1_BYTES; break; //DLC = 1
                case 2: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_2_BYTES; break; //DLC = 2
                case 3: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_3_BYTES; break; //DLC = 3
                case 4: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_4_BYTES; break; //DLC = 4
                case 5: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_5_BYTES; break; //DLC = 5
                case 6: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_6_BYTES; break; //DLC = 6
                case 7: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_7_BYTES; break; //DLC = 7
                case 8: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES; break; //DLC = 8
                case 12: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_12_BYTES; break; //DLC = 9
                case 16: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_16_BYTES; break; //DLC = 10
                case 20: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_20_BYTES; break; //DLC = 11
                case 24: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_24_BYTES; break; //DLC = 12
                case 32: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_32_BYTES; break; //DLC = 13
                case 48: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_48_BYTES; break; //DLC = 14
                case 64: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_64_BYTES; break; //DLC = 15
                default: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES; break; //DLC = 8
            }
            return xl_canfd_dlc;
        }

        /// <summary>
        /// Gets the actual payload of the CAN FD frame based on the DLC flag
        /// </summary>
        /// <param name="dlc"></param>
        /// <returns></returns>
        internal static uint GET_CANFD_PAYLOAD(XLDefine.XL_CANFD_DLC dlc)
        {
            uint payload = 8;
            switch (dlc)
            {
                case XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_0_BYTES: payload = 0; break; //DLC = 0
                case XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_1_BYTES: payload = 1; break; //DLC = 1
                case XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_2_BYTES: payload = 2; break; //DLC = 2
                case XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_3_BYTES: payload = 3; break; //DLC = 3
                case XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_4_BYTES: payload = 4; break; //DLC = 4
                case XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_5_BYTES: payload = 5; break; //DLC = 5
                case XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_6_BYTES: payload = 6; break; //DLC = 6
                case XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_7_BYTES: payload = 7; break; //DLC = 7
                case XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES: payload = 8; break; //DLC = 8
                case XLDefine.XL_CANFD_DLC.DLC_CANFD_12_BYTES: payload = 12; break; //DLC = 9
                case XLDefine.XL_CANFD_DLC.DLC_CANFD_16_BYTES: payload = 16; break; //DLC = 10
                case XLDefine.XL_CANFD_DLC.DLC_CANFD_20_BYTES: payload = 20; break; //DLC = 11
                case XLDefine.XL_CANFD_DLC.DLC_CANFD_24_BYTES: payload = 24; break; //DLC = 12
                case XLDefine.XL_CANFD_DLC.DLC_CANFD_32_BYTES: payload = 32; break; //DLC = 13
                case XLDefine.XL_CANFD_DLC.DLC_CANFD_48_BYTES: payload = 48; break; //DLC = 14
                case XLDefine.XL_CANFD_DLC.DLC_CANFD_64_BYTES: payload = 64; break; //DLC = 15
                default: payload = 8; break; //DLC = 8
            }
            return payload;
        }

        /// <summary>
        /// Calculates the final DLC based on the length of the data to be transmitted
        /// </summary>
        /// <param name="dataLength">length of the data to be transmitted on the BUS</param>
        /// <returns></returns>
        internal static XLDefine.XL_CANFD_DLC GET_TRANSMIT_XL_CANFD_DLC(byte dataLength)
        {
            XLDefine.XL_CANFD_DLC xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES;
            if (dataLength <= 8)
            {
                switch (dataLength)
                {
                    case 0: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_0_BYTES; break; //DLC = 0
                    case 1: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_1_BYTES; break; //DLC = 1
                    case 2: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_2_BYTES; break; //DLC = 2
                    case 3: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_3_BYTES; break; //DLC = 3
                    case 4: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_4_BYTES; break; //DLC = 4
                    case 5: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_5_BYTES; break; //DLC = 5
                    case 6: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_6_BYTES; break; //DLC = 6
                    case 7: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_7_BYTES; break; //DLC = 7
                    case 8: xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CAN_CANFD_8_BYTES; break; //DLC = 8
                }
            }
            else
            {
                if (dataLength > 8 && dataLength <= 12)
                {
                    xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_12_BYTES; //DLC = 9
                }
                else if (dataLength > 12 && dataLength <= 16)
                {
                    xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_16_BYTES; //DLC = 10
                }
                else if (dataLength > 16 && dataLength <= 20)
                {
                    xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_20_BYTES; //DLC = 11
                }
                else if (dataLength > 20 && dataLength <= 24)
                {
                    xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_24_BYTES; //DLC = 12
                }
                else if (dataLength > 24 && dataLength <= 32)
                {
                    xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_32_BYTES; //DLC = 13
                }
                else if (dataLength > 32 && dataLength <= 48)
                {
                    xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_48_BYTES; //DLC = 14
                }
                else if (dataLength > 48) //64 can be the maximum dlc for any length of data
                {
                    xl_canfd_dlc = XLDefine.XL_CANFD_DLC.DLC_CANFD_64_BYTES; //DLC = 15
                }
            }
            return xl_canfd_dlc;
        }
    }
}
